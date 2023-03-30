using UnityEngine;

public static class CursorWrapper
{
	public static CursorData Data
	{
		get => data;
		set {
			if ( data == value ) return;
			data = value;

			UICursor.Instance.Data = data;
		}
	}
	private static CursorData data;

	public static bool IsHardwareVisible
	{
		get => Cursor.visible;
		set => Cursor.visible = value;
	}
}