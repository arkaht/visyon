using UnityEngine;

public static class TransformUtils
{
	public static void Clear( Transform transform )
	{
		foreach ( Transform child in transform )
			GameObject.Destroy( child.gameObject );
	}
}