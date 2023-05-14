using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class GameObjectUtils
{
	public static void Hide( this MonoBehaviour behaviour ) => behaviour.gameObject.SetActive( false );
	public static void Show( this MonoBehaviour behaviour ) => behaviour.gameObject.SetActive( true );

	public static IEnumerator DoInvalidate( LayoutGroup layout_group, CanvasGroup canvas_group = null )
	{
		if ( canvas_group != null )
			canvas_group.alpha = 0.0f;

		//  hide a frame
		layout_group.enabled = false;
		yield return new WaitForEndOfFrame();

		//  show and wait for update
		layout_group.enabled = true;
		yield return new WaitForEndOfFrame();

		if ( canvas_group != null )
			canvas_group.alpha = 1.0f;
	}
	public static void Invalidate( this MonoBehaviour behaviour, LayoutGroup layout_group, CanvasGroup canvas_group = null )
		=> behaviour.StartCoroutine( DoInvalidate( layout_group, canvas_group ) );
}