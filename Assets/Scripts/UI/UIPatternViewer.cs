using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Visyon.Wiki;

public class PatternViewerData
{
	public PatternData Data;
	public float ScrollY;
}

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
	private Button previousButton, nextButton, updateButton;
	[SerializeField]
	private string patternID;

	private readonly History<PatternViewerData> history = new();

	public void Clear()
	{
		TransformUtils.Clear( contentTransform );
	}

	public void NavigateHistory( int offset )
	{
		UpdateCurrentInHistory();

		PatternViewerData data = history.Navigate( offset );

		//  update buttons
		UpdateHistoryButtons();

		if ( data == null )
			return;

		//  apply data
		ApplyPatternData( data.Data, true, data.ScrollY );
	}

	public void PlacePattern()
	{
		if ( Data == null )
			return;

		//  spawn
		UIPattern pattern = UIPattern.Spawn( Data.ID );

		//  center it on camera
		Vector3 position = Blueprinter.Instance.Camera.transform.position;
		position.z = 0.0f;
		pattern.transform.position = position;
	}

	public void UpdatePattern()
	{
		if ( Data == null )
			return;
		WikiCollectionUpdater.ScheduleUpdate( $"'{Data.Name}' Update", () => new string[] { Data.Name } );
	}

	public void ApplyPatternData( PatternData data, bool no_history = false, float scroll_y = 0.0f )
	{
		gameObject.SetActive( true );

		if ( !no_history )
		{
			UpdateCurrentInHistory();

			//  insert previous into history
			history.Add(
				new()
				{
					Data = data,
					ScrollY = 0.0f
				}
			);
			UpdateHistoryButtons();
		}

		//  set new data
		Data = data;
		patternID = data == null ? string.Empty : data.ID;

		Clear();

		//  set content
		tmpName.text = data.Name;
		AddTextsTo( data.Texts.Markups, contentTransform );

		StartCoroutine( DoScrollVertically( scroll_y ) );  //  delay scroll to let transform be updated correctly
														   //  and avoid clamping to previous height
	}

	private void UpdateCurrentInHistory()
	{
		PatternViewerData vdata = history.Current;
		if ( vdata == null ) return;

		vdata.ScrollY = contentTransform.position.y;
	}

	private IEnumerator DoScrollVertically( float y )
	{
		yield return new WaitForEndOfFrame();
		ScrollVertically( y );
	}

	public void ScrollVertically( float y )
	{
		Vector3 pos = contentTransform.position;
		pos.y = y;
		contentTransform.position = pos;
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

	void Start()
	{
		if ( patternID == string.Empty )
			Reset();
		else
			Reload();

		//  bind to events
		UITaskViewer.Instance.OnBegin.AddListener( () => updateButton.interactable = false );
		UITaskViewer.Instance.OnEnd.AddListener( () => updateButton.interactable = true );
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
		if ( patternID == string.Empty )
			return;
		if ( !PatternRegistery.TryGet( patternID, out PatternData new_data ) )
			return;

		Debug.Log( "UIPatternViewer: reloading.." );

		history.Remove( history.Current );
		ApplyPatternData( new_data );
	}
}