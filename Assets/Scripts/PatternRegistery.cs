using System.Collections.Generic;
using System.IO;
using UnityEngine;

using SimpleJSON;

public static class PatternRegistery
{
	private static Dictionary<string, PatternData> patterns = new();

	public static string PATH => Application.streamingAssetsPath + "/PatternsDB/";

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

	public static bool TryGet( string id, out PatternData pattern )
	{
		//  retrieve from cache
		if ( !patterns.ContainsKey( id ) )
		{
			pattern = Load( id );
			if ( pattern == null )
				return false;

			patterns[id] = pattern;
		}
		else
		{
			pattern = patterns[id];
		}

		return true;
	}

	public static PatternData Load( string id )
	{
		string path = PATH + id + ".json";
		string json = File.ReadAllText( path );
		if ( json == null ) return null;

		return LoadFromJSON( id, json );
	}
	
	public static void LoadAll()
	{
		string[] files = Directory.GetFiles( PATH, "*.json" );

		int load_successes = 0;
		foreach ( string file in files )
		{
			//  load from file
			string id = Path.GetFileNameWithoutExtension( file );
			PatternData pattern = Load( id );
			if ( pattern == null )
			{
				Debug.LogError( "PatternRegistery: failed to load pattern '" + id + "'" );
				continue;
			}

			//  register it
			if ( patterns.ContainsKey( id ) )
				patterns[id] = pattern;
			else
				patterns.Add( id, pattern );

			load_successes++;
			//Debug.Log( "PatternRegistery: loaded pattern '" + id + "'" );
		}

		Debug.Log( "PatternRegistery: loaded successfully " + load_successes + "/" + files.Length + " files" );
	}

	public static PatternData LoadFromJSON( string id, string json )
	{
		JSONNode data = JSON.Parse( json );

		return new PatternData( 
			id,
			data["name"].Value,
			LoadTextsFromJSONNode( data["texts"] ),
			LoadRelationsFromJSONNode( data["relations"] )
		);
	}

	public static PatternTexts LoadTextsFromJSONNode( JSONNode data )
	{
		return new PatternTexts( 
			data["definition"].Value, 
			data["description"].AsArray.ToStringArray(),
			data["examples"].AsArray.ToStringArray(),
			data["usage"].AsArray.ToStringArray(),
			data["consequences"].AsArray.ToStringArray()
		);
	}

	public static PatternRelations LoadRelationsFromJSONNode( JSONNode data )
	{
		return new PatternRelations(
			data["instantiates"].AsArray.ToStringArray(),
			data["modulates"].AsArray.ToStringArray(),
			data["instantiated_by"].AsArray.ToStringArray(),
			data["modulated_by"].AsArray.ToStringArray(),
			data["conflicts"].AsArray.ToStringArray()
		);
	}
}
