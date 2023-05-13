using SimpleJSON;

public record PatternData( string ID, string Name, string[] Categories, PatternTexts Texts, PatternRelations Relations ) 
	: IJSONSerializable
{
	public static PatternData FromJSON( string id, JSONNode data )
	{
		return 
			new PatternData( 
				id,
				data["name"].Value,
				data["categories"].AsArray.ToStringArray(),
				PatternTexts.FromJSON( data["texts"] ),
				PatternRelations.FromJSON( data["relations"] )
			);
	}

	public JSONNode Serialize()
	{
		JSONObject data = new();
		data["name"] = Name;
		data["categories"] = JSONArray.FromStringArray( Categories );
		data["texts"] = Texts.Serialize();
		data["relations"] = Relations.Serialize();
		return data;
	}

	public override string ToString()
	{
		return $"{GetType().Name}[Name={Name}; Texts={Texts}; Relations={Relations}]";
	}
}

public record PatternTexts( string Definition, string[] Markups )
	: IJSONSerializable
{
	public static PatternTexts FromJSON( JSONNode data )
	{
		return 
			new PatternTexts( 
				data["definition"].Value, 
				data["markups"].AsArray.ToStringArray()
			);
	}

	public JSONNode Serialize()
	{
		JSONObject data = new();
		data["definition"] = Definition;
		data["markups"] = JSONArray.FromStringArray( Markups );
		return data;
	}

	public override string ToString()
	{
		return $"{GetType().Name}[Definition={Definition}; Markups({Markups.Length});]";
	}
}

public record PatternRelations( string[] Instantiates, string[] Modulates, string[] InstantiatedBy, string[] ModulatedBy, string[] Conflicts )
	: IJSONSerializable
{
	public static PatternRelations FromJSON( JSONNode data )
	{
		return 
			new PatternRelations(
				data["instantiates"].AsArray.ToStringArray(),
				data["modulates"].AsArray.ToStringArray(),
				data["instantiated_by"].AsArray.ToStringArray(),
				data["modulated_by"].AsArray.ToStringArray(),
				data["conflicts"].AsArray.ToStringArray()
			);
	}

	public JSONNode Serialize()
	{
		JSONObject data = new();
		data["instantiates"] = JSONArray.FromStringArray( Instantiates );
		data["modulates"] = JSONArray.FromStringArray( Modulates );
		data["instantiated_by"] = JSONArray.FromStringArray( InstantiatedBy );
		data["modulated_by"] = JSONArray.FromStringArray( ModulatedBy );
		data["conflicts"] = JSONArray.FromStringArray( Conflicts );
		return data;
	}

	public override string ToString()
	{
		return $"{GetType().Name}[Instantiates({Instantiates.Length}; Modulates({Modulates.Length}); InstantiatedBy({InstantiatedBy.Length}); ModulatedBy({ModulatedBy.Length}); Conflicts({Conflicts.Length})]";
	}
}

public enum PatternRelationType
{
	None,
	Instantiates,
	InstantiatedBy,
	Modulates,
	ModulatedBy,
	Conflicts,
}