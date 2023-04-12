﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Utils;

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
	[SerializeField]
	private Button previousButton, nextButton;

	private readonly History<PatternData> history = new();

	public void Clear()
	{
		TransformUtils.Clear( informationTransform );
		TransformUtils.Clear( usageTransform );
		TransformUtils.Clear( consequencesTransform );
		TransformUtils.Clear( relationsTransform );
	}

	public void NavigateHistory( int offset )
	{
		PatternData data = history.Navigate( offset );

		//  update buttons
		UpdateHistoryButtons();

		//  apply data
		if ( data == null ) return;
		ApplyPatternData( data, true );
	}

	public void PlacePattern()
	{
		if ( Data == null ) return;

		//  spawn
		UIPattern pattern = UIPattern.Spawn( Data.ID );

		//  center it on camera
		Vector3 position = Blueprinter.Instance.Camera.transform.position;
		position.z = 0.0f;
		pattern.transform.position = position;
	}

	public void ApplyPatternData( PatternData data, bool no_history = false )
	{
		gameObject.SetActive( true );

		//  don't update if equal
		if ( data == Data ) return;

		//  insert previous into history
		if ( !no_history )
		{
			history.Add( data );
			UpdateHistoryButtons();
		}

		//  set new data
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
		history.Clear();
	}

	private void UpdateHistoryButtons()
	{
		previousButton.interactable = history.Previous != null;
		nextButton.interactable = history.Next != null;
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

	void OnEnable()
	{
		NavigateHistory( 0 );
	}
}