using UnityEngine;
using UnityEngine.EventSystems;

public class UICursorChanger : MonoBehaviour,
							   IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private Texture2D hoveredCursor;

	public void OnPointerEnter( PointerEventData data )
	{
		Cursor.SetCursor( hoveredCursor, Vector2.zero, CursorMode.ForceSoftware );
	}

	public void OnPointerExit( PointerEventData data )
	{
		Cursor.SetCursor( null, Vector2.zero, CursorMode.ForceSoftware );
	}
}