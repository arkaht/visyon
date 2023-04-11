using System;
using TMPro;
using UnityEditor;
using UnityEngine;

public class UIPatternViewer : MonoBehaviour
{
	public PatternData Data { get; private set; }

	[Header( "References" )]
	[SerializeField]
	private Transform scrollView;
	[SerializeField]
	private Transform informationTransform, 
					  usageTransform, 
					  consequencesTransform,
					  relationsTransform;
	[SerializeField]
	private TextMeshProUGUI tmpName, 
							tmpDefinition;
	[SerializeField]
	private GameObject textPrefab;

	public void Clear()
	{
		TransformUtils.Clear( informationTransform );
		TransformUtils.Clear( usageTransform );
		TransformUtils.Clear( consequencesTransform );
		TransformUtils.Clear( relationsTransform );
	}

	public void ApplyPatternData( PatternData data )
	{
		gameObject.SetActive( true );

		if ( data == Data ) return;
		Data = data;
		
		Clear();

		//  basics
		tmpName.text = data.Name;
		tmpDefinition.text = data.Texts.Definition;

		//  more content
		AddTextsTo( data.Texts.Description, informationTransform );
		AddTextsTo( data.Texts.Examples, informationTransform, ( text ) => "<b>Example: </b>" + text );
		AddTextsTo( data.Texts.Usage, usageTransform );
		AddTextsTo( data.Texts.Consequences, consequencesTransform );

		//TMP_TextUtilities.FindNearestLink;
		
		//  relations
		AddTextTo( "<b>Instantiates: </b>" + PatternRegistery.ConcatPatterns( data.Relations.Instantiates, true ), relationsTransform );
		AddTextTo( "<b>Modulates: </b>" + PatternRegistery.ConcatPatterns( data.Relations.Modulates, true ), relationsTransform );
		AddTextTo( "<b>Instantiated By: </b>" + PatternRegistery.ConcatPatterns( data.Relations.InstantiatedBy, true ), relationsTransform );
		AddTextTo( "<b>Modulated By: </b>" + PatternRegistery.ConcatPatterns( data.Relations.ModulatedBy, true ), relationsTransform );
		AddTextTo( "<b>Potentially Conflicting with: </b>" + PatternRegistery.ConcatPatterns( data.Relations.Conflicts, true ), relationsTransform );
	}
	
	public void Reset()
	{
		Data = null;
		gameObject.SetActive( false );
	}

	private void AddTextsTo( string[] texts, Transform parent, Func<string, string> modifier = null )
	{
		foreach ( string text in texts )
			AddTextTo( modifier == null ? text : modifier( text ), parent );
	}

	private void AddTextTo( string text, Transform parent )
	{
		GameObject obj = Instantiate( textPrefab, parent );
		var viewer_text = obj.GetComponent<UIPatternViewerText>();
		viewer_text.Viewer = this;
		viewer_text.Text = text;
	}

	void Awake()
	{
		Reset();
	}
}