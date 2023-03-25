using UnityEngine;

public static class CursorWrapper
{
	public static CursorData Data
	{
		get => data;
		set {
			if ( data == value ) return;
			data = value;

			if ( data == null )
				Cursor.SetCursor( null, Vector2.zero, CursorMode.ForceSoftware );
			else
				Cursor.SetCursor( data.Texture, data.Hotspot, CursorMode.ForceSoftware );
		}
	}
	private static CursorData data;
}