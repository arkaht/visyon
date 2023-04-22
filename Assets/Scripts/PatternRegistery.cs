using System.Collections.Generic;
using System.IO;
using UnityEngine;

using SimpleJSON;
using System.Text.RegularExpressions;
using UnityEngine.Events;

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
		//  retrieve from cache
		if ( !patterns.ContainsKey( id ) )
		{
			/*pattern = Load( id );
			if ( pattern == null )
				return false;

			patterns[id] = pattern;*/
			pattern = null;
			return false;
		}
		else
		{
			pattern = patterns[id];
		}

		return true;
	}

	/// <summary>
	/// Inform every registered events to reload themselves potentially due to a registery update
	/// </summary>
	public static void Reload() => OnReload.Invoke();

	public static PatternData LoadFromCollection( string collection, string id )
	{
		string path = $"{DirectoryPath}{collection}/{id}.json";
		string json = File.ReadAllText( path );
		if ( json == null ) return null;

		return LoadFromJSON( id, json );
	}
	
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
}
