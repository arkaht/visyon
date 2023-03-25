using UnityEngine;

[CreateAssetMenu( menuName = "Data/CursorData" )]
public class CursorData : ScriptableObject
{
	public Texture2D Texture;
	public Vector2 Hotspot = new( 16.0f, 16.0f );
}