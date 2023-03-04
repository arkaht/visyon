using UnityEngine;
using UnityEngine.EventSystems;

public class UIMoveable : MonoBehaviour, 
						  IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public RectTransform RectTransform { get; private set; }
	public bool IsDragging { get; private set; }
	public bool IsHovered { get; private set; }

	[SerializeField]
	private UIGrid grid;
	[SerializeField]
	private bool snapOnMove = true;
	[SerializeField]
	private bool snapOnStart = true;
	[SerializeField]
	private PointerEventData.InputButton inputButton;

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
		if ( data.button != inputButton ) return;

		draggedPos = RectTransform.anchoredPosition;
		IsDragging = true;
	}

	public void OnDrag( PointerEventData data )
	{
		if ( data.button != inputButton ) 
		{
			Blueprinter.Instance.OnDrag( data );
			return;
		}

		if ( Blueprinter.Instance.IsDragging ) return;

		draggedPos += data.delta * Blueprinter.Instance.PixelRatio;
		RectTransform.anchoredPosition = snapOnMove ? grid.SnapPosition( draggedPos ) : draggedPos;
	}

	public void OnEndDrag( PointerEventData data )
	{
		if ( data.button != inputButton ) 
		{
			Blueprinter.Instance.OnEndDrag( data );
			return;
		}

		IsDragging = false;
	}
}
