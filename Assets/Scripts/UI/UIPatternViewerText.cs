using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPatternViewerText : MonoBehaviour,
								   IPointerClickHandler
{
	private readonly static Regex REGEX_LINK = new( @"\[([^\]]*)\]\(([^\)]*)\)" );

	public string Text
	{
		get => textTMP.text;
		set 
		{
			string text = REGEX_LINK.Replace( 
				value, 
				match =>
				{
					string id = match.Groups[2].ToString();
					string name = match.Groups[1].ToString();

					//  style to current pattern
					if ( id == Viewer.Data.ID )
						return $"<style=\"CurrentPatternLink\">{name}</style>";

					//  style & link to pattern
					return $"<style=\"PatternLink\"><link=\"{id}\">{name}</link></style>";
				} 
			);
			textTMP.text = text;
		}
	}
	public UIPatternViewer Viewer;

	[SerializeField]
	private TMP_Text textTMP;

	public void OnPointerClick( PointerEventData data )
	{
		int id = TMP_TextUtilities.FindIntersectingLink( textTMP, data.position, null );
		if ( id == -1 ) return;

		//  get pattern ID
		string link = textTMP.textInfo.linkInfo[id].GetLinkID();
		if ( !PatternRegistery.TryGet( link, out PatternData pattern_data ) )
		{
			Debug.LogError( $"UIPatternViewerText: link to unknown pattern '{link}'" );
			return;
		}

		//  show pattern
		Viewer.ApplyPatternData( pattern_data );
	}
}