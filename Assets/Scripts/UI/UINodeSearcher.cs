using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UINodeSearcher : MonoBehaviour
{
	[SerializeField]
	private RectTransform content;

	[Header( "Prefabs" )]
	[SerializeField]
	private GameObject choicePrefab;
	[SerializeField]
	private GameObject patternPrefab;
	private static GameObject selfPrefab;

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
		choice.Text.text = name;
		choice.Button.onClick.AddListener( OnChoiceClicked );
		choice.ID = id;
	}

	private void OnChoiceClicked()
	{
		GameObject selected = EventSystem.current.currentSelectedGameObject;
		if ( !selected.TryGetComponent( out UINodeSearcherChoice choice ) )
			return;

		UIPattern pattern = UIPattern.Spawn( choice.ID );
		pattern.transform.position = transform.position;

		Destroy( gameObject );
	}

	public static UINodeSearcher Spawn()
	{
		if ( selfPrefab == null )
		{
			selfPrefab = Resources.Load<GameObject>( "Prefabs/UI/NodeSearcher" );
		}

		GameObject obj = Instantiate( selfPrefab, Blueprinter.Instance.transform );
		UINodeSearcher searcher = obj.GetComponent<UINodeSearcher>();

		return searcher;
	}
}
