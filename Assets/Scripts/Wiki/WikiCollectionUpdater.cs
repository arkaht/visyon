
using SimpleJSON;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Visyon.Wiki
{
	public static class WikiCollectionUpdater
	{
		public static string DirectoryPath => Application.streamingAssetsPath + "/Collections/Official/";

		private static readonly RequestClient client = new( "http://virt10.itu.chalmers.se/api.php" );

		private const string REG_REF = @"\[\[([^\]]+)\]\]";
		private const string REG_MARK_CATEGORY = @"\[\[Category:([^\]]+)\]\]";
		private const string REG_DEFINITION = @"''([^']+)''";
		private const string REG_HEADER = @"=+\s([^=]*)\s=+";

		public static async void AsyncUpdateAll() 
		{
		}

		public static void Update( string pattern_name ) => AsyncUpdate( pattern_name );  //  TODO: remove
		public static async void AsyncUpdate( string page_name )
		{
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

			StringBuilder bb_text = new();

			//  parse wiki text
			string current_header = "Description";
			foreach ( string line in text.Split( "\n" ) )
			{
				if ( line.Length == 0 || line == "-" ) continue;  //  skip empty lines

				Match match;

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
					bb_text.AppendLine( "<style=\"Definition\">" + definition + "</style>" + "\n" );
					//Debug.Log( "Definition: " + definition );
					//Debug.Log( line );
					continue;
				}

				//  search: header
				if ( ( match = Regex.Match( line, REG_HEADER ) ).Success )
				{
					current_header = match.Groups[1].ToString();
					bb_text.AppendLine( "<style=\"h2\">" + current_header + "</style>" );
					//Debug.Log( "Current Header: " + current_header );
					continue;
				}

				Match relation_match;
				string relation_pattern_id = "";
				if ( ( relation_match = Regex.Match( line, REG_REF ) ).Success )
					relation_pattern_id = PatternRegistery.SafePatternID( relation_match.Groups[1].ToString() );

				//  fill data
				switch ( current_header )
				{
					//  texts
					case "Description":
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
						break;
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
					case "History":
					case "References":
					case "Acknowledgements":
						break;
					//  un-supported header
					default:
						Debug.LogWarning( $"WikiCollectionUpdater: header '{current_header}' for '{page_name}' is not supported!" );
						break;
				}

				bb_text.AppendLine( line + "\n" );
			}

			//  combine data
			string pattern_id = PatternRegistery.SafePatternID( page_name );
			PatternTexts data_texts = new( 
				definition, 
				description.ToArray(),
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

			Debug.Log( bb_text.ToString() );
		}
	}
}
