using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UINodeSearcher : MonoBehaviour
{
	public bool IsSearchFieldFocused => searchField.isFocused;
	public TMP_InputField SearchField => searchField;

	public Vector2 SpawnPosition { get; set; }

	[Header( "References" )]
	[SerializeField]
	private RectTransform content;
	[SerializeField]
	private TMP_InputField searchField;

	[Header( "Prefabs" )]
	[SerializeField]
	private GameObject choicePrefab;
	private static GameObject selfPrefab;

	private List<UINodeSearcherChoice> choices = new();

	public void FocusSearchField()
	{
		searchField.Select();
		searchField.ActivateInputField();
	}

	public void AddAllPatterns()
	{
		foreach ( string id in PatternRegistery.AllKeys )
			AddPattern( id );
	}

	public void AddPattern( string id )
	{
		if ( !PatternRegistery.TryGet( id, out PatternData pattern ) )
		{
			Debug.LogError( "UINodeSearcher: failed to find pattern '" + id + "'" );
			return;
		}

		AddChoice( pattern.Name, id );
	}

	public void AddChoice( string name, string id )
	{
		GameObject obj = Instantiate( choicePrefab, content );

		UINodeSearcherChoice choice = obj.GetComponent<UINodeSearcherChoice>();
		choice.Name = name;
		choice.Button.onClick.AddListener( OnChoiceClicked );
		choice.ID = id;

		choices.Add( choice );
	}

	public void UpdateSearchFilter()
	{
		string search_text = searchField.text.ToLower();
		foreach ( UINodeSearcherChoice choice in choices )
		{
			string lower_name = choice.Name.ToLower();
			choice.gameObject.SetActive( lower_name.Contains( search_text ) );
		}
	}

	private void OnChoiceClicked()
	{
		GameObject selected = EventSystem.current.currentSelectedGameObject;
		if ( !selected.TryGetComponent( out UINodeSearcherChoice choice ) )
			return;

		UIPattern pattern = UIPattern.Spawn( choice.ID );
		pattern.transform.position = SpawnPosition;

		Destroy( gameObject );
	}

	public static UINodeSearcher Spawn( Vector2 pos )
	{
		if ( selfPrefab == null )
		{
			selfPrefab = Resources.Load<GameObject>( "Prefabs/UI/NodeSearcher" );
		}

		GameObject obj = Instantiate( selfPrefab, Blueprinter.Instance.ContentTransform );
		UINodeSearcher searcher = obj.GetComponent<UINodeSearcher>();
		searcher.transform.position = pos;
		searcher.SpawnPosition = pos;

		return searcher;
	}
}
