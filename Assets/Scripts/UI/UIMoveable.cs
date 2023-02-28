using UnityEngine;
using UnityEngine.EventSystems;

public class UIMoveable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[SerializeField]
	private UIGrid grid;
	[SerializeField]
	private bool snapOnAwake = true;

	private Vector2 draggedPos;
	private RectTransform rectTransform;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();

		if ( snapOnAwake )
			rectTransform.anchoredPosition = grid.SnapPosition( rectTransform.anchoredPosition );
	}

	public void OnBeginDrag( PointerEventData data )
	{
		draggedPos = rectTransform.anchoredPosition;
	}

	public void OnDrag( PointerEventData data )
	{
		draggedPos += data.delta / grid.Canvas.scaleFactor;
		rectTransform.anchoredPosition = grid.SnapPosition( draggedPos );
	}

	public void OnEndDrag( PointerEventData data )
	{
		print( "EndDrag: snap" );
	}
}
