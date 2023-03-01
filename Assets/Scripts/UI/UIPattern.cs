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

	private static GameObject selfPrefab;

	void Awake()
	{
		Moveable = GetComponent<UIMoveable>();
	}

	public bool SetPattern( string id )
	{
		if ( !PatternRegistery.TryGet( id, out PatternData pattern ) ) return false;

		ID = id;
		PatternData = pattern;

		//print( "data: " + PatternData );

		/*foreach ( string pattern_id in PatternData.Relations.Instantiates )
		{
			print( pattern_id );
		}*/

		ApplyPatternData();
		return true;
	}

	public void ApplyPatternData()
	{
		tmpName.text = PatternData.Name;
	}

	public static UIPattern Spawn( string id )
	{
		if ( selfPrefab == null )
		{
			selfPrefab = Resources.Load<GameObject>( "Prefabs/UI/Pattern" );
		}

		GameObject obj = Instantiate( selfPrefab, Blueprinter.Instance.transform );
		UIPattern pattern = obj.GetComponent<UIPattern>();
		pattern.SetPattern( id );

		return pattern;
	}
}