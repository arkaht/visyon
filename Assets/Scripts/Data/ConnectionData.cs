using UnityEngine;

[CreateAssetMenu( menuName = "Data/ConnectionData" )]
public class ConnectionData : ScriptableObject
{
	[Header( "Definition" )]
	public float Thickness = 5.0f;
	public Color Color = Color.white;
	public CursorData Cursor;

	[Header( "Arrow" )]
	public Sprite ArrowSprite;
	public Vector2 ArrowSize = new( 32.0f, 32.0f );
}