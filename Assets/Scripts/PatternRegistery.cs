using System.Collections.Generic;
using System.IO;
using UnityEngine;

using SimpleJSON;

public record PatternTexts( string Definition, string[] Description, string[] Examples, string[] Usage, string[] Consequences )
{
	public override string ToString()
	{
		return $"{GetType().Name}[Definition={Definition}; Description({Description.Length}); Examples({Examples.Length}); Usage({Usage.Length}); Consequences({Consequences.Length})]";
	}
}

public record PatternRelationData( string[] Instantiates, string[] Modulates, string[] InstantiatedBy, string[] ModulatedBy, string[] Conflicts )
{
	public override string ToString()
	{
		return $"{GetType().Name}[Instantiates({Instantiates.Length}; Modulates({Modulates.Length}); InstantiatedBy({InstantiatedBy.Length}); ModulatedBy({ModulatedBy.Length}); Conflicts({Conflicts.Length})]";
	}
}

public class PatternData
{
	public string Name { get; private set; }
	public PatternTexts Texts { get; private set; }
	public PatternRelationData Relations { get; private set; }

	public PatternData( string name, PatternTexts texts, PatternRelationData relations )
	{
		Name = name;
		Texts = texts;
		Relations = relations;
	}

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

		return LoadFromJSON( json );
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

	public static PatternData LoadFromJSON( string json )
	{
		JSONNode data = JSON.Parse( json );

		return new PatternData( 
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

	public static PatternRelationData LoadRelationsFromJSONNode( JSONNode data )
	{
		return new PatternRelationData(
			data["instantiates"].AsArray.ToStringArray(),
			data["modulates"].AsArray.ToStringArray(),
			data["instantiated_by"].AsArray.ToStringArray(),
			data["modulated_by"].AsArray.ToStringArray(),
			data["conflicts"].AsArray.ToStringArray()
		);
	}
}
