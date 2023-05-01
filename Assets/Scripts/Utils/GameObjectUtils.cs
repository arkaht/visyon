using UnityEngine;

public static class GameObjectUtils
{
	public static void Hide( this MonoBehaviour behaviour ) => behaviour.gameObject.SetActive( false );
	public static void Show( this MonoBehaviour behaviour ) => behaviour.gameObject.SetActive( true );	
}