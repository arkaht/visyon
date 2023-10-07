using System.Collections.Generic;
using System.IO;
using UnityEngine;

using SimpleJSON;
using System.Text.RegularExpressions;
using UnityEngine.Events;
using System;
using Visyon.Wiki;

public static class PatternRegistery
{
	public static UnityEvent OnReload = new();

	private static Dictionary<string, PatternData> patterns = new();

	public static string DirectoryPath => Application.streamingAssetsPath + "/Collections/";

	public static Dictionary<string, PatternData>.KeyCollection AllKeys => patterns.Keys;

	public static string ConcatPatterns( string[] ids, bool in_markdown = false, string separator = ", " )
	{
		string text = "";

		int length = ids.Length;
		for ( int i = 0; i < length; i++ )
		{
			string id = ids[i];

			//  join name
			if ( TryGet( id, out PatternData pattern ) )
			{
				text += in_markdown ? $"[{pattern.Name}]({id})" : pattern.Name;
			}
			else
				text += id;

			//  join separator
			if ( i + 1 < length )
				text += separator;
		}

		return text;
	}

	public static string SafePatternID( string name )
	{
		string id = "";

		var matches = Regex.Matches( name, @"[A-Z][a-zA-Z]+" );
		foreach ( Match match in matches )
		{
			if ( id != "" )
				id += "-";

			id += match.Value;
		}

		return id.ToLower();
	}

	public static bool TryGet( string id, out PatternData pattern )
	{
		//  check cache
		if ( !patterns.ContainsKey( id ) )
		{
			pattern = null;
			return false;
		}

		pattern = patterns[id];
		return true;
	}

	/// <summary>
	/// Inform every registered events to reload themselves potentially due to a registery update
	/// </summary>
	public static void Reload() => OnReload.Invoke();

	public static PatternData LoadFromCollection( string collection, string id )
	{
		string path = GetFilePathTo( collection, id );
		string json = File.ReadAllText( path );
		if ( json == null ) return null;

		return LoadFromJSON( id, json );
	}

	public static string GetFilePathTo( string collection, string id ) => $"{DirectoryPath}{collection}/{id}.json";

	public static void RegisterCollection( string collection, bool should_reload = true )
	{
		string[] files = Directory.GetFiles( $"{DirectoryPath}{collection}/", "*.json" );

		int load_successes = 0;
		foreach ( string file in files )
		{
			//  load from file
			string id = Path.GetFileNameWithoutExtension( file );
			PatternData pattern = LoadFromCollection( collection, id );
			if ( pattern == null )
			{
				Debug.LogError( "PatternRegistery: failed to load pattern '" + id + "'" );
				continue;
			}

			//  register it
			patterns[id] = pattern;

			load_successes++;
			//Debug.Log( "PatternRegistery: loaded pattern '" + id + "'" );
		}

		Debug.Log( "PatternRegistery: loaded successfully " + load_successes + "/" + files.Length + " files" );

		if ( should_reload )
			Reload();
	}

	public static PatternData LoadFromJSON( string id, string json )
	{
		JSONNode data = JSON.Parse( json );
		return PatternData.FromJSON( id, data );
	}

	const char CSV_SEPARATOR = ';';
	private static string[] SplitCSV( string line )
	{
		List<string> columns = new();

		bool in_quote = true;
		string text = "";

		for ( int i = 0; i < line.Length; i++ )
		{
			char letter = line[i];
			bool is_last_iteration = i == line.Length - 1;

			//  include double-quote characters
			//Debug.Log( i + "] " + text + " : " + letter );
			if ( letter == '\"' )
			{
				in_quote = !in_quote;
				if ( !is_last_iteration && line[i + 1] != '\"' ) continue;
			}

			//  split texts
			if ( is_last_iteration || letter == CSV_SEPARATOR && in_quote )
			{
				columns.Add( text );
				//Debug.Log( text );
				text = "";
				continue;
			}

			text += letter;
		}

		//Debug.Log( columns.Count );
		return columns.ToArray();
	}

	public static void ImportCSV( string path )
	{
		//  read from disk
		string[] lines = File.ReadAllLines( path );
		if ( lines.Length == 0 ) return;

		int id_key = -1;
		int name_key = -1;
		int definition_key = -1;
		int categories_start_key = -1, categories_end_key = -1;

		for ( int i = 0; i < lines.Length; i++ )
		{
			string line = lines[i];
			string[] columns = SplitCSV( line );

			//  find column keys
			if ( i == 0 )
			{
				int key = 0;
				foreach ( string column in columns )
				{
					switch ( column )
					{
						case "ID":
							id_key = key;
							break;
						case "Name":
							name_key = key;
							break;
						case "Definition":
							definition_key = key;
							break;
						default:
							if ( column.StartsWith( "Categories" ) )
							{
								if ( categories_start_key == -1 )
								{
									categories_start_key = key;
								}

								categories_end_key = key;
							}
							break;
					}

					key++;
				}

				//Debug.Log( id_key + ";" + name_key + ";" + definition_key + ";" + categories_start_key + ":" + categories_end_key );

				if ( id_key == -1 )
				{
					Debug.LogError( "PatternRegistery.ImportCSV: couldn't find key for 'ID', aborting.." );
					return;
				}
				continue;
			}

			string id = columns[id_key];
			if ( !TryGet( id, out PatternData pattern ) )
			{
				Debug.LogWarning( "PatternRegistery.ImportCSV: pattern '" + id + "' couldn't be found, next.." );
				continue;
			}

			//  set name
			string name = pattern.Name;
			if ( name_key != -1 )
			{
				name = columns[name_key];
			}

			//  set definition
			string definition = pattern.Texts.Definition;
			if ( definition_key != -1 )
			{
				definition = columns[definition_key];
			}

			//  set categories
			string[] categories = pattern.Categories;
			if ( categories_start_key != -1 )
			{
				List<string> new_categories = new();

				for ( int key = categories_start_key; key < categories_end_key; key++ )
				{
					string category = columns[key];
					if ( category.Length == 0 ) continue;

					new_categories.Add( category );
				}

				categories = new_categories.ToArray();
			}

			//  create new pattern
			PatternData new_pattern = new( 
				id, 
				name, 
				categories,
				new PatternTexts( definition, pattern.Texts.Markups ), 
				pattern.Relations 
			);
			patterns[id] = new_pattern;

			//  save to disk
			var json = new_pattern.Serialize();
			File.WriteAllText( WikiCollectionUpdater.GetFilePathTo( id ), json.ToString( 4 ) );

			//Debug.Log( json );
		}

		Reload();
	}

	public static void ExportCSV( string path, ICollection<string> ids )
	{
		string csv = "ID;Name;Definition;";
		
		//  build categories columns
		const int NUMBER_OF_CATEGORIES = 10;
		for ( int i = 0; i < NUMBER_OF_CATEGORIES; i++ )
		{
			csv += "Categories (" + ( i + 1 ) + ")";

			if ( i + 1 < NUMBER_OF_CATEGORIES )
			{
				csv += CSV_SEPARATOR;
			}
		}
		csv += "\n";

		//  fill patterns data
		foreach ( string id in ids )
		{
			if ( !TryGet( id, out PatternData pattern ) )
			{
				Debug.LogWarning( "PatternRegistery.ExportCSV: pattern '" + id + "' couldn't be found, next.." );
				continue;
			}

			csv += id + CSV_SEPARATOR + pattern.Name + CSV_SEPARATOR + "\"" + pattern.Texts.Definition.Replace( "\"", "\"\"" ) + "\"" + CSV_SEPARATOR;

			//  fill categories
			for ( int i = 0; i < NUMBER_OF_CATEGORIES; i++ )
			{
				string category = "";
				if ( i < pattern.Categories.Length )
				{
					category = pattern.Categories[i];
				}
				csv += category;

				if ( i + 1 < NUMBER_OF_CATEGORIES )
				{
					csv += CSV_SEPARATOR;
				}
			}

			csv += "\n";
		}

		//  save to disk
		File.WriteAllText( path, csv );
	}
}
