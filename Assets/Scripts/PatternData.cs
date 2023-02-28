
public record PatternData( string Name, PatternTexts Texts, PatternRelations Relations )
{
	public override string ToString()
	{
		return $"{GetType().Name}[Name={Name}; Texts={Texts}; Relations={Relations}]";
	}
}

public record PatternTexts( string Definition, string[] Description, string[] Examples, string[] Usage, string[] Consequences )
{
	public override string ToString()
	{
		return $"{GetType().Name}[Definition={Definition}; Description({Description.Length}); Examples({Examples.Length}); Usage({Usage.Length}); Consequences({Consequences.Length})]";
	}
}

public record PatternRelations( string[] Instantiates, string[] Modulates, string[] InstantiatedBy, string[] ModulatedBy, string[] Conflicts )
{
	public override string ToString()
	{
		return $"{GetType().Name}[Instantiates({Instantiates.Length}; Modulates({Modulates.Length}); InstantiatedBy({InstantiatedBy.Length}); ModulatedBy({ModulatedBy.Length}); Conflicts({Conflicts.Length})]";
	}
}
