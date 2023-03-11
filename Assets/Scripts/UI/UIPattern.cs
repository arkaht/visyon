using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UIPattern : MonoBehaviour
{
	public PatternData PatternData { get; private set; }
	public UIMoveable Moveable { get; private set; }

	public string ID = "ability-losses";

	[SerializeField]
	private TextMeshProUGUI tmpName;
	[SerializeField]
	private TextMeshProUGUI tmpDefinition;
	[SerializeField]
	private UIPatternPin[] pins;

	private Dictionary<PatternRelationType, UIPatternPin> relationPins;

	private static GameObject selfPrefab;

	public bool SetPattern( string id )
	{
		if ( !PatternRegistery.TryGet( id, out PatternData pattern ) ) return false;

		ID = id;
		PatternData = pattern;

		ApplyPatternData();
		return true;
	}

	public UIPatternPin GetRelationPin( PatternRelationType relation )
	{
		if ( relationPins == null )
			RetrieveRelationPins();

		if ( relationPins.TryGetValue( relation, out UIPatternPin pin ) )
			return pin;

		return null;
	}

	public void ApplyPatternData()
	{
		tmpName.text = PatternData.Name;
		tmpDefinition.text = PatternData.Texts.Definition;
	}

	public static UIPattern Spawn( string id )
	{
		if ( selfPrefab == null )
		{
			selfPrefab = Resources.Load<GameObject>( "Prefabs/UI/Pattern" );
		}

		GameObject obj = Instantiate( selfPrefab, Blueprinter.Instance.PatternsTransform );
		UIPattern pattern = obj.GetComponent<UIPattern>();
		pattern.SetPattern( id );

		return pattern;
	}

	private void RetrieveRelationPins()
	{
		relationPins = new();

		foreach ( UIPatternPin pin in pins )
			relationPins.Add( pin.Relation, pin );
	}

	void Awake()
	{
		Moveable = GetComponent<UIMoveable>();
	}
}