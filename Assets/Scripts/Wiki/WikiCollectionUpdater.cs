
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Visyon.Core;

namespace Visyon.Wiki
{
	public static class WikiCollectionUpdater
	{
		public static string DirectoryPath => Application.streamingAssetsPath + "/Collections/Official/";

		public const string BaseLink = "http://virt10.itu.chalmers.se/";
		public static string IndexLink => BaseLink + "index.php";
		public static string APILink => BaseLink + "api.php";

		private static readonly RequestClient client = new( APILink );
		private static readonly HashSet<string> patternsIDs = new();

		internal const string REG_REF = @"\[\[([^\]]+)\]\]";
		internal const string REG_SPECIAL_REF = @"\[\[([^|\]]*)\|?([^\]]*)\]\]";
		internal const string REG_MARK_CATEGORY = @"\[\[Category:([^\]]+)\]\]";
		internal const string REG_DEFINITION = @"''([^']+)''";
		internal const string REG_HEADER = @"=+\s([^=]*)\s=+";

		public static async void AsyncUpdateAll() 
		{
			await AsyncUpdate( "Abilities" );
			await AsyncUpdate( "Aim_%26_Shoot" );
		}

		public static async Task AsyncRetrievePatternsIDs()
		{
			patternsIDs.Clear();

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

					//Debug.Log( pattern_id );
					if ( !patternsIDs.Add( pattern_id ) )
						Debug.LogWarning( $"WikiCollectionUpdater: {pattern_name} conflicts with the ID {pattern_id}!" );
				}

				//  update continue
				if ( !root.HasKey( "continue" ) )
					break;
				args.Update( root["continue"].AsObject );
			}
			while ( true );

			Debug.Log( $"WikiCollectionUpdater: retrieved {patternsIDs.Count} patterns IDs" );
		}

		public static void Update( string pattern_name ) => AsyncUpdate( pattern_name );  //  TODO: remove
		public static async Task AsyncUpdate( string page_name )
		{
			//  ensure patterns IDs are retrieved
			if ( patternsIDs.Count == 0 )
				await AsyncRetrievePatternsIDs();

			//  request content
			RequestArguments args = WikiArgumentsBuilder.GetPage( page_name );
			string content = await client.Get( args );
			Debug.Log( content );

			//  parse to json
			JSONNode root = JSON.Parse( content );
			string text = root["parse"]["wikitext"]["*"];
			string pattern_name = root["parse"]["title"];
			Debug.Log( pattern_name + ": " + text );

			//  init
			List<string> categories = new();
			string definition = null;

			List<string> description = new(),
						 examples = new(),
						 usage = new(),
						 consequences = new();

			List<string> instantiates = new(),
						 modulates = new(),
						 instantiated_by = new(),
						 modulated_by = new(),
						 conflicts = new();

			string pattern_id = PatternRegistery.SafePatternID( pattern_name );
			List<string> markups = new();

			//  parse wiki text
			Match match;
			string current_header = "Description";
			foreach ( string line in text.Split( "\n" ) )
			{
				if ( line == string.Empty || line == "-" ) continue;  //  skip empty lines

				//  search: category marks
				if ( ( match = Regex.Match( line, REG_MARK_CATEGORY ) ).Success )
				{
					string category = match.Groups[1].ToString();
					categories.Add( category );
					continue;
				}

				//  search: definition
				if ( definition == null && ( match = Regex.Match( line, REG_DEFINITION ) ).Success )
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
					if ( header_type < 4 )
						current_header = header;

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
				switch ( current_header )
				{
					//  texts
					/*case "Description":
						description.Add( line );
						break;
					case "Examples":
						examples.Add( line );
						break;
					case "Using the pattern":
						usage.Add( line );
						break;
					case "Consequences":
						consequences.Add( line );
						break;*/
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
					//  ignore these
					/*case "History":
					case "References":
					case "Acknowledgements":
						break;
					//  un-supported header
					default:
						Debug.LogWarning( $"WikiCollectionUpdater: header '{current_header}' for '{page_name}' is not supported!" );
						break;*/
				}

				markups.Add( ReplaceReferences( line, pattern_id ) );
			}

			//  combine data
			PatternTexts data_texts = new( 
				definition, 
				markups.ToArray(),
				examples.ToArray(), 
				usage.ToArray(), 
				consequences.ToArray() 
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
			File.WriteAllText( DirectoryPath + pattern_id + ".json", data_pattern.Serialize().ToString( 4 ) );
		}

		public static string ReplaceReferences( string input, string current_pattern_id = "" )
		{
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
