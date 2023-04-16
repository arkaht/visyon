using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent( typeof( UIMoveable ), typeof( UniqueID ), typeof( UISelectable ) )]
public class UIPattern : MonoBehaviour,
						 IJSONSerializable
{
	public PatternData PatternData { get; private set; }
	public UIMoveable Moveable { get; private set; }
	public UniqueID UID { get; private set; }
	public Dictionary<PatternRelationType, UIPatternPin> RelationPins { get; private set; } = new();
	public UISelectable Selectable { get; private set; }

	public string ID = "ability-losses";

	[SerializeField]
	private TextMeshProUGUI tmpName;
	[SerializeField]
	private TextMeshProUGUI tmpDefinition;
	[SerializeField]
	private UIPatternPin[] pins;

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
		if ( RelationPins.Count == 0 )
			RetrieveRelationPins();

		if ( RelationPins.TryGetValue( relation, out UIPatternPin pin ) )
			return pin;

		return null;
	}

	public void ApplyPatternData()
	{
		gameObject.name = $"Pattern '{PatternData.Name}'";

		tmpName.text = PatternData.Name;
		tmpDefinition.text = PatternData.Texts.Definition;
	}

	public JSONNode Serialize()
	{
		JSONObject obj = new();
		obj["type"] = (int)SerializableType.Pattern;
		obj["position"] = transform.position;
		obj["p-id"] = ID;
		obj["u-id"] = UID.ID;

		JSONObject pins = new();
		foreach ( PatternRelationType relation in RelationPins.Keys )
		{
			UIPatternPin pin = RelationPins[relation];
			pins[relation.ToString()] = pin.Serialize();
		}
		obj["pins"] = pins;

		return obj;
	}

	public static UIPattern Load( JSONObject obj )
	{
		string pid = obj["p-id"];
		int uid = obj["u-id"];

		UIPattern pattern = Spawn( pid );
		pattern.transform.position = obj["position"];
		pattern.UID.ID = uid;

		//  differ pattern linking
		pattern.StartCoroutine( pattern.CoroutineLoad( obj ) );

		return pattern;
	}
	private IEnumerator CoroutineLoad( JSONObject obj )
	{
		yield return new WaitForEndOfFrame();

		//  link to patterns
		JSONObject pins = obj["pins"].AsObject;
		foreach ( PatternRelationType relation in RelationPins.Keys )
		{
			UIPatternPin pin = GetRelationPin( relation );
			if ( pin == null ) continue;

			JSONArray connections = pins[relation.ToString()].AsArray;
			foreach ( JSONNode connection in connections )
			{
				if ( !UniqueID.TryGet( connection.AsInt, out UniqueID unique_id ) ) continue;
				if ( !unique_id.TryGetComponent( out UIPattern target_pattern ) ) continue;

				pin.Connect( target_pattern );
			}
		}
	}
	public static UIPattern Spawn( string id )
	{
		if ( selfPrefab == null )
		{
			selfPrefab = Resources.Load<GameObject>( "Prefabs/UI/Pattern" );
		}

		GameObject obj = Instantiate( selfPrefab, Blueprinter.Instance.PatternsTransform );
		UIPattern pattern = obj.GetComponent<UIPattern>();

		if ( id != null )
			pattern.SetPattern( id );

		return pattern;
	}

	private void RetrieveRelationPins()
	{
		RelationPins.Clear();

		foreach ( UIPatternPin pin in pins )
			RelationPins.Add( pin.Relation, pin );
	}

	public void OnDoubleClick()
	{
		Blueprinter.Instance.Viewer.ApplyPatternData( PatternData );
	}

	/*public void OnPointerClick( PointerEventData data )
	{
		if ( data.clickCount != 2 ) 
		{
			Blueprinter.Instance.OnPointerClick( data );
			return;
		}
		Blueprinter.Instance.Viewer.ApplyPatternData( PatternData );
	}*/

	void Awake()
	{
		Moveable = GetComponent<UIMoveable>();
		UID = GetComponent<UniqueID>();
		Selectable = GetComponent<UISelectable>();

		RetrieveRelationPins();
	}

	void Start()
	{
		Blueprinter.Instance.AddSerializable( this );
	}

	void OnDestroy()
	{
		Blueprinter.Instance.RemoveSerializable( this );
	}
}