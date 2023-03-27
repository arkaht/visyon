using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIMenu : MonoBehaviour,
					  IPointerEnterHandler, IPointerExitHandler
{
	public UIMenuTab CurrentTab { get; private set; }

	[HideInInspector]
	public UnityEvent<UIMenuTab> OnShow, OnHide;

	[Header( "References" )]
	[SerializeField]
	private GameObject actionPrefab;

	private Coroutine hideCoroutine;

	public void Clear()
	{
		TransformUtils.Clear( transform );
	}

	public UIMenuAction AddAction( string name, string shortcut, Action<Blueprinter> callback )
	{
		GameObject obj = Instantiate( actionPrefab, transform );

		UIMenuAction action = obj.GetComponent<UIMenuAction>();
		action.Name = name;
		action.Shortcut = shortcut;
		action.Callback = callback;
		action.Menu = this;

		return action;
	}

	public void Show( UIMenuTab tab )
	{
		//  show
		CancelHide();
		gameObject.SetActive( true );

		//  set position
		transform.position = tab.SpawnPoint;

		//  populate actions
		Clear();
		foreach ( MenuAction action in tab.Actions )
			AddAction( action.Name, action.StringifyShortcut(), action.Execute );

		CurrentTab = tab;
		OnShow.Invoke( CurrentTab );
	}
	public void Hide()
	{
		if ( !gameObject.activeSelf ) return;
		if ( hideCoroutine != null ) return;

		//  start new coroutine
		IEnumerator ScheduleHide()
		{
			yield return new WaitForSeconds( 0.1f );

			gameObject.SetActive( false );
			hideCoroutine = null;

			OnHide.Invoke( CurrentTab );
			CurrentTab = null;
		}
		hideCoroutine = StartCoroutine( ScheduleHide() );
	}
	public void CancelHide()
	{
		if ( hideCoroutine == null ) return;

		StopCoroutine( hideCoroutine );
		hideCoroutine = null;
	}

	public void OnPointerEnter( PointerEventData data ) => CancelHide();
	public void OnPointerExit( PointerEventData data ) 
	{
		if ( !data.fullyExited ) return;
		Hide();
	}
}
