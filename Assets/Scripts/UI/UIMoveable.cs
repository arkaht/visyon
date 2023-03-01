using UnityEngine;
using UnityEngine.EventSystems;

public class UIMoveable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public RectTransform RectTransform { get; private set; }

	[SerializeField]
	private UIGrid grid;
	[SerializeField]
	private bool snapOnStart = true;

	private Vector2 draggedPos;

	void Awake()
	{
		RectTransform = GetComponent<RectTransform>();

		//  set default grid
		if ( grid == null )
			grid = UIGrid.Instance;
	}

	void Start()
	{
		if ( snapOnStart )
			RectTransform.anchoredPosition = grid.SnapPosition( RectTransform.anchoredPosition );
	}

	public void OnBeginDrag( PointerEventData data )
	{
		draggedPos = RectTransform.anchoredPosition;
	}

	public void OnDrag( PointerEventData data )
	{
		draggedPos += data.delta / grid.Canvas.scaleFactor;
		RectTransform.anchoredPosition = grid.SnapPosition( draggedPos );
	}

	public void OnEndDrag( PointerEventData data )
	{
		print( "EndDrag: snap" );
	}
}
