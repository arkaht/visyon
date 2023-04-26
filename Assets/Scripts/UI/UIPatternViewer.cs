using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;
using Visyon.Wiki;

public class UIPatternViewer : MonoBehaviour
{
	public PatternData Data { get; private set; }

	[Header( "References" )]
	[SerializeField]
	private Transform scrollView;
	[SerializeField]
	private Transform contentTransform;
	[SerializeField]
	private TextMeshProUGUI tmpName;
	[SerializeField]
	private GameObject textPrefab;
	[SerializeField]
	private Button previousButton, nextButton;
	[SerializeField]
	private string patternID;

	private readonly History<PatternData> history = new();

	public void Clear()
	{
		TransformUtils.Clear( contentTransform );
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

	public void UpdatePattern()
	{
		if ( Data == null ) return;
		WikiCollectionUpdater.AsyncUpdate( Uri.EscapeDataString( Data.Name ) );
	}

	public void ApplyPatternData( PatternData data, bool no_history = false )
	{
		gameObject.SetActive( true );

		//  insert previous into history
		if ( !no_history )
		{
			history.Add( data );
			UpdateHistoryButtons();
		}

		//  set new data
		Data = data;
		patternID = data == null ? string.Empty : data.ID;
		
		Clear();

		//  set content
		tmpName.text = data.Name;
		AddTextsTo( data.Texts.Markups, contentTransform );
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
		if ( patternID == string.Empty )
			Reset();
		else if ( PatternRegistery.TryGet( patternID, out PatternData data ) )
			ApplyPatternData( data );
	}

	void OnEnable()
	{
		NavigateHistory( 0 );

		//  bind to event
		PatternRegistery.OnReload.AddListener( Reload );
	}
	void OnDisable()
	{
		//  unbind to event
		PatternRegistery.OnReload.RemoveListener( Reload );
	}

	private void Reload()
	{
		if ( patternID == string.Empty ) return;
		if ( !PatternRegistery.TryGet( patternID, out PatternData new_data ) ) return;

		Debug.Log( "UIPatternViewer: reloading.." );
		ApplyPatternData( new_data, true );
	}
}