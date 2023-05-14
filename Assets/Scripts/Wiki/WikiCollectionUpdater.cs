
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Visyon.Core;

namespace Visyon.Wiki
{
	public enum TaskResponse
	{
		Success,
		Critical,
	}

	public static class WikiCollectionUpdater
	{
		public const string CollectionName = "Official";
		public static string DirectoryPath => Application.streamingAssetsPath + "/Collections/" + CollectionName + "/";

		public const string BaseLink = "http://virt10.itu.chalmers.se/";
		public static string IndexLink => BaseLink + "index.php";
		public static string APILink => BaseLink + "api.php";

		private static readonly RequestClient client = new( APILink );
		private static readonly HashSet<string> patternsIDs = new();
		private static readonly List<string> patternsPages = new();

		internal const string REG_REF = @"\[\[([^\]]+)\]\]";
		internal const string REG_SPECIAL_REF = @"\[\[([^|\]]*)\|?([^\]]*)\]\]";
		internal const string REG_MARK_CATEGORY = @"\[\[Category:([^\]]+)\]\]";
		internal const string REG_DEFINITION = @"^''(.+)''$";
		internal const string REG_HEADER = @"=+\s([^=]*)\s=+";
		internal const string REG_SMALL_REF = @"<ref[^>]*\/>";
		internal const string REG_ITALIC = @"''([^']*)''";
		internal const string REG_PATTERN_CATEGORY = @"^([^|]*Patterns)\|?";

		public static async void ScheduleUpdateAll() => await ScheduleUpdate( "Wiki Collection Update", () => patternsPages.ToArray() );
		public static async Task ScheduleUpdate( string title, Func<string[]> get_patterns )
		{
			using Tasker tasker = UITaskViewer.Instance.Use( title );
			if ( tasker == null )
			{
				Debug.LogWarning( "WikiCollectionUpdater: a tasker is already reserved, unable to update!" );
				return;
			}

			CancellationTokenSource token = tasker.UseCancelToken();

			//  retrieving
			bool should_retrieve = patternsPages.Count == 0;
			if ( should_retrieve )
			{
				await tasker.Task( "Retrieving Patterns IDs", AsyncRetrievePatternsIDs() );
				tasker.AddProgress();
			}

			string[] patterns = get_patterns();
			tasker.SetMaximumProgress( patterns.Length + ( should_retrieve ? 1 : 0 ) );

			//  downloading
			int successes = 0;
			bool has_error = false;
			foreach ( string page in patterns )
			{
				if ( token.IsCancellationRequested )
				{
					tasker.State = "User canceled";
					Debug.Log( $"WikiCollectionUpdater: cancelled by token, successfully updated {successes}/{patterns.Length}" );
					return;
				}

				await tasker.Task( 
					"Downloading: " + page, 
					Task.Run( 
						async () =>
						{
							try
							{
								await AsyncUpdate( Uri.EscapeDataString( page ) );
								successes++;
							}
							catch ( TaskCanceledException e )
							{
								has_error = true;
								Debug.LogException( e );
							}
							catch ( Exception e )
							{
								//has_error = true;
								Debug.LogException( e );
							}
						} 
					)
				);
				tasker.AddProgress();

				//  delay next request to prevent service outage
				await Task.Delay( 250 );

				if ( has_error ) break;
			}

			if ( !has_error )
			{
				//  success
				tasker.State = $"Succesfully updated {successes}/{patterns.Length} patterns";
				Debug.Log( $"WikiCollectionUpdater: successfully updated {successes}/{patterns.Length}" );

				//  reload registery
				PatternRegistery.RegisterCollection( CollectionName );
			}
			else
			{
				tasker.State = $"An error has occured!";
				Debug.Log( $"WikiCollectionUpdater: cancelled due to error, successfully updated {successes}/{patterns.Length}" );
			}
		}

		public static async Task AsyncRetrievePatternsIDs()
		{
			patternsIDs.Clear();
			patternsPages.Clear();

			//  build params
			RequestArguments args = WikiArgumentsBuilder.GetPagesFromCategory( "Patterns", 300 );

			//  fetch'em
			int iter = 0;
			do
			{
				if ( ++iter > 50 )
				{
					Debug.LogWarning( "WikiCollectionUpdater: canceling patterns retrieving, exceed maximum iterations!" );
					break;
				}	

				//  do request
				string content = await client.Get( args );

				//  parse json
				JSONObject root = JSON.Parse( content ).AsObject;
				JSONArray array = root["query"]["categorymembers"].AsArray;

				//  fill IDs
				foreach ( JSONObject obj in array )
				{
					string pattern_name = obj["title"];
					string pattern_id = PatternRegistery.SafePatternID( pattern_name );

					//Debug.Log( pattern_name + " | " + pattern_id );
					if ( !patternsIDs.Add( pattern_id ) )
						Debug.LogWarning( $"WikiCollectionUpdater: {pattern_name} conflicts with the ID {pattern_id}!" );

					patternsPages.Add( pattern_name );
				}

				//  update continue
				if ( !root.HasKey( "continue" ) )
					break;
				args.Update( root["continue"].AsObject );
			}
			while ( true );

			Debug.Log( $"WikiCollectionUpdater: retrieved {patternsIDs.Count} patterns IDs" );
		}

		public static async Task AsyncUpdate( string page_name )
		{
			//  ensure patterns IDs are retrieved
			if ( patternsIDs.Count == 0 )
				await AsyncRetrievePatternsIDs();

			//  request content
			RequestArguments args = WikiArgumentsBuilder.GetPage( page_name );
			string content = await client.Get( args );
			//Debug.Log( content );

			//  parse to json
			JSONNode root = JSON.Parse( content );
			if ( root.HasKey( "error" ) )
			{
				string code = root["error"]["code"];
				if ( code.StartsWith( "internal" ) )
					//case "internal_api_error_DBConnectionError":
					throw new TaskCanceledException( "WikiCollectionUpdater: server internal error: " + code );

				throw new Exception( $"WikiCollectionUpdater: failed to get page of '{page_name}', result body: '{content}'" );
			}

			string text = root["parse"]["wikitext"]["*"];
			string pattern_name = root["parse"]["title"];
			//Debug.Log( pattern_name + ": " + text );

			//  init
			List<string> categories = new();
			string definition = null;

			List<string> instantiates = new(),
						 modulates = new(),
						 instantiated_by = new(),
						 modulated_by = new(),
						 conflicts = new();

			string pattern_id = PatternRegistery.SafePatternID( pattern_name );
			List<string> markups = new();
			List<string> relation_buffer = new();

			//  parse wiki text
			Match match;
			bool in_relation = false;
			string current_header = "Description";
			foreach ( string line in text.Split( "\n" ) )
			{
				if ( line == string.Empty )  //  skip empty lines
				{
					//  append relation buffer
					if ( relation_buffer.Count > 0 )
					{
						markups.Add( string.Join( ", ", relation_buffer ) );
						relation_buffer.Clear();
					}
					continue;
				}
				if ( line == "-" )
				{
					if ( in_relation )
						markups.Add( "N/A" );

					continue;
				}

				//  search: category marks
				if ( ( match = Regex.Match( line, REG_MARK_CATEGORY ) ).Success )
				{
					string category = match.Groups[1].ToString();

					//  filter category
					if ( !( match = Regex.Match( category, REG_PATTERN_CATEGORY ) ).Success ) continue;
					category = match.Groups[1].ToString();

					//  more elegant category names
					category = category.Replace( "_", " " )  //  specially for "Aesthetic_Patterns"
									   .Replace( "pattern", "Patterns" );  //  specially for "Long pattern"

					//  avoid duplicates
					if ( categories.Contains( category ) ) continue;

					categories.Add( category );
					continue;
				}

				//  search: definition
				if ( definition == null && ( match = Regex.Match( line.Trim(), REG_DEFINITION ) ).Success )
				{
					definition = match.Groups[1].ToString();
					markups.Add( TextMarkup.AsDefinition( definition ) );
					//Debug.Log( "Definition: " + definition );
					continue;
				}

				//  search: header
				if ( ( match = Regex.Match( line, REG_HEADER ) ).Success )
				{
					string header = match.Groups[1].ToString();
					if ( header == "History" ) break;  //  stop parsing from history

					//  get header type
					int header_type = ( line.Split( "=" ).Length - 1 ) / 2;
					if ( header_type < 4 )  //  ignore h4+
						current_header = header;

					//  handle relation buffer
					switch ( header )
					{
						case "Can Instantiate":
						case "Can Modulate":
						case "Can Be Instantiated By":
						case "Can Be Modulated By":
						case "Possible Closure Effects":
						case "Potentially Conflicting With":
							in_relation = true;
							break;
						default:
							if ( header_type < 4 )
								in_relation = false;

							if ( relation_buffer.Count > 0 )
								relation_buffer.Clear();
							break;
					}

					//  append
					header = ReplaceReferences( header, pattern_id );
					markups.Add( TextMarkup.AsHeader( header_type, header ) );
					//Debug.Log( "Current Header: " + current_header );
					continue;
				}

				//  match pattern relations
				string relation_pattern_id = "";
				if ( ( match = Regex.Match( line, REG_REF ) ).Success )
					relation_pattern_id = PatternRegistery.SafePatternID( match.Groups[1].ToString() );

				//  fill data
				string marked_text = ReplaceReferences( line, pattern_id );
				switch ( current_header )
				{
					//  relations
					case "Can Instantiate":
						instantiates.Add( relation_pattern_id );
						break;
					case "Can Modulate":
						modulates.Add( relation_pattern_id );
						break;
					case "Can Be Instantiated By":
						instantiated_by.Add( relation_pattern_id );
						break;
					case "Can Be Modulated By":
						modulated_by.Add( relation_pattern_id );
						break;
					case "Possible Closure Effects":
						//  TODO: potentially think of support this
						break;
					case "Potentially Conflicting With":
						conflicts.Add( relation_pattern_id );
						break;
					//  un-supported header
					default:
						//Debug.LogWarning( $"WikiCollectionUpdater: header '{current_header}' for '{page_name}' is not supported!" );
						break;
				}

				if ( in_relation )
				{
					marked_text = Regex.Replace( marked_text, @"(,)\s*$", "" ).Trim();
					relation_buffer.Add( marked_text );
				}
				else
					markups.Add( marked_text );
			}

			//  combine data
			PatternTexts data_texts = new(
				definition, 
				markups.ToArray()
			);
			PatternRelations data_relations = new( 
				instantiates.ToArray(), 
				modulates.ToArray(), 
				instantiated_by.ToArray(), 
				modulated_by.ToArray(), 
				conflicts.ToArray()
			);
			PatternData data_pattern = new( 
				pattern_id, 
				pattern_name, 
				categories.ToArray(), 
				data_texts,
				data_relations 
			);

			//  write to file
			Directory.CreateDirectory( DirectoryPath );
			await File.WriteAllTextAsync( DirectoryPath + pattern_id + ".json", data_pattern.Serialize().ToString( 4 ) );
		}

		public static string ReplaceReferences( string input, string current_pattern_id = "" )
		{
			//  remove "small" references
			input = Regex.Replace( input, REG_SMALL_REF, "" );

			//  italic
			input = Regex.Replace( input, REG_ITALIC, match => {
				return TextMarkup.AsItalic( match.Groups[1].Value );
			} );

			//  replace "special" references
			return 
				Regex.Replace( input, REG_SPECIAL_REF, match => {
					string reference = match.Groups[1].Value;
					string nickname = match.Groups[2].Value == string.Empty ? reference : match.Groups[2].Value;

					//Debug.Log( reference + " | " + nickname );

					//  category reference
					if ( reference.StartsWith( ":Category" ) )
					{
						return TextMarkup.AsLink( 
							Uri.EscapeUriString( IndexLink + "/" + reference.Remove( 0, 1 ) ), 
							nickname 
						);
					}

					//  pattern reference
					string id = PatternRegistery.SafePatternID( reference );
					if ( patternsIDs.Contains( id ) )
					{
						//  current link
						if ( id == current_pattern_id )
							return TextMarkup.AsCurrentLink( nickname );

						//  link to pattern
						return TextMarkup.AsLink( id, nickname );
					}

					//  game or unknown references..
					return TextMarkup.AsLink( 
						Uri.EscapeUriString( IndexLink + "/" + reference ), 
						nickname 
					);
				} );
		}
	}
}
