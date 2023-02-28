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

	public string ID = "ability-losses";

	[SerializeField]
	private TextMeshProUGUI tmpName;


	void Awake()
	{
		SetPattern( ID );
	}

	public bool SetPattern( string id )
	{
		if ( !PatternRegistery.TryGet( id, out PatternData pattern ) ) return false;

		ID = id;
		PatternData = pattern;

		print( "data: " + PatternData );

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
}