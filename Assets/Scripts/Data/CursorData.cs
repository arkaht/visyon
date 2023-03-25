using UnityEngine;

[CreateAssetMenu( menuName = "Data/CursorData" )]
public class CursorData : ScriptableObject
{
	public Sprite Sprite;
	public Vector2 Hotspot = new( 16.0f, 16.0f );
}