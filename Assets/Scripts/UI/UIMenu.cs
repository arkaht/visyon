using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMenu : MonoBehaviour,
					  IPointerEnterHandler, IPointerExitHandler
{
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

		return action;
	}

	public void Show( Vector3 pos, List<MenuAction> actions )
	{
		//  show
		CancelHide();
		gameObject.SetActive( true );

		//  set position
		transform.position = pos;

		//  populate actions
		Clear();
		foreach ( MenuAction action in actions )
			AddAction( action.Name, action.StringifyShortcut(), action.Execute );
	}
	public void Hide()
	{
		//  remove previous coroutine
		CancelHide();

		//  start new coroutine
		IEnumerator ScheduleHide()
		{
			yield return new WaitForSeconds( 0.1f );

			gameObject.SetActive( false );
			hideCoroutine = null;
		}
		hideCoroutine = StartCoroutine( ScheduleHide() );
	}
	public void CancelHide()
	{
		if ( hideCoroutine == null ) return;
		StopCoroutine( hideCoroutine );
	}

	public void OnPointerEnter( PointerEventData data ) => CancelHide();
	public void OnPointerExit( PointerEventData data ) => Hide();
}
