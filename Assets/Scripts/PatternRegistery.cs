using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using SimpleJSON;

public struct PatternTexts
{
	public string Definition;
	public string[] Description;
	public string[] Examples;
	public string[] Usage;
	public string[] Consequences;

	public override string ToString()
	{
		return $"{GetType().Name}[Definition={Definition}; Description({Description.Length}); Examples({Examples.Length}); Usage({Usage.Length}); Consequences({Consequences.Length})]";
	}
}

public struct PatternRelationData
{
	public string[] Instantiates;
	public string[] Modulates;
	public string[] InstantiatedBy;
	public string[] ModulatedBy;
	public string[] Conflicts;

	public override string ToString()
	{
		return $"{GetType().Name}[Instantiates({Instantiates.Length}; Modulates({Modulates.Length}); InstantiatedBy({InstantiatedBy.Length}); ModulatedBy({ModulatedBy.Length}); Conflicts({Conflicts.Length})]";
	}
}

public struct PatternData
{
	public bool IsNull => Name == null;

	public string Name;
	public PatternTexts Texts;
	public PatternRelationData Relations;

	public override string ToString()
	{
		return $"{GetType().Name}[Name={Name}; Description={Texts}; Relations={Relations}]";
	}
}

public static class PatternRegistery
{
	private static Dictionary<string, PatternData> patterns = new();

	public const string PATH = "Assets/Resources/PatternsDB/";

	public static bool TryGet( string id, out PatternData pattern )
	{
		//  retrieve from cache
		if ( !patterns.ContainsKey( id ) )
		{
			if ( !Load( id, out pattern ) )
				return false;

			patterns[id] = pattern;
		}
		else
		{
			pattern = patterns[id];
		}

		return true;
	}

	public static bool Load( string id, out PatternData pattern )
	{
		string path = PATH + id + ".json";
		string json = File.ReadAllText( path );
		if ( json == null )
		{
			pattern = new PatternData();
			return false;
		}

		pattern = LoadFromJSON( json );
		return true;
	}
	
	public static void LoadAll()
	{
		string[] files = Directory.GetFiles( PATH, "*.json" );

		int load_successes = 0;
		foreach ( string file in files )
		{
			//  load from file
			string id = Path.GetFileNameWithoutExtension( file );
			if ( !Load( id, out PatternData pattern ) )
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

	public static PatternData LoadFromJSON( string json )
	{
		JSONNode data = JSON.Parse( json );

		return new PatternData
		{
			Name		= data["name"].Value,
			Texts		= LoadTextsFromJSONNode( data["texts"] ),
			Relations	= LoadRelationsFromJSONNode( data["relations"] ),
		};
	}

	public static PatternTexts LoadTextsFromJSONNode( JSONNode data )
	{
		return new PatternTexts
		{
			Definition		= data["definition"].Value,
			Description		= data["description"].AsArray.ToStringArray(),
			Examples		= data["examples"].AsArray.ToStringArray(),
			Usage			= data["usage"].AsArray.ToStringArray(),
			Consequences	= data["consequences"].AsArray.ToStringArray(),
		};
	}

	public static PatternRelationData LoadRelationsFromJSONNode( JSONNode data )
	{
		return new PatternRelationData
		{
			Instantiates	= data["instantiates"].AsArray.ToStringArray(),
			Modulates		= data["modulates"].AsArray.ToStringArray(),
			InstantiatedBy	= data["instantiated_by"].AsArray.ToStringArray(),
			ModulatedBy		= data["modulated_by"].AsArray.ToStringArray(),
			Conflicts		= data["conflicts"].AsArray.ToStringArray(),
		};
	}
}
