using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using SimpleJSON;

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
	public string Description;
	public PatternRelationData Relations;

	public override string ToString()
	{
		return $"{GetType().Name}[Name={Name}; Description={Description}; Relations={Relations}]";
	}
}

public static class PatternRegistery
{
	private static Dictionary<string, PatternData> patterns = new();

	public static string DirectoryPath => "Assets/Resources/PatternsDB/";

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
		string path = DirectoryPath + id + ".json";
		string json = File.ReadAllText( path );
		if ( json == null )
		{
			pattern = new PatternData();
			return false;
		}

		pattern = LoadFromJSON( json );
		return true;
	}

	public static PatternData LoadFromJSON( string json )
	{
		JSONNode data = JSON.Parse( json );

		return new PatternData
		{
			Name = data["name"].Value,
			Description = data["description"].Value,
			Relations = LoadRelationsFromJSONNode( data["relations"] ),
		};
	}

	public static PatternRelationData LoadRelationsFromJSONNode( JSONNode data )
	{
		return new PatternRelationData
		{
			Instantiates   = data["instantiates"].AsArray.ToStringArray(),
			Modulates      = data["modulates"].AsArray.ToStringArray(),
			InstantiatedBy = data["instantiated_by"].AsArray.ToStringArray(),
			ModulatedBy    = data["modulated_by"].AsArray.ToStringArray(),
			Conflicts      = data["conflicts"].AsArray.ToStringArray(),
		};
	}
}
