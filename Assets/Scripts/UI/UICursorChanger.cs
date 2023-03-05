using UnityEngine;
using UnityEngine.EventSystems;

public class UICursorChanger : MonoBehaviour,
							   IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private Texture2D hoveredCursor;
	[SerializeField]
	private Vector2 hotspot;

	public void OnPointerEnter( PointerEventData data )
	{
		Cursor.SetCursor( hoveredCursor, hotspot, CursorMode.ForceSoftware );
	}

	public void OnPointerExit( PointerEventData data )
	{
		Cursor.SetCursor( null, Vector2.zero, CursorMode.ForceSoftware );
	}
}