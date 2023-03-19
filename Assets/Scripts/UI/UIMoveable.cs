using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIMoveable : MonoBehaviour
{
	public RectTransform RectTransform { get; private set; }
	public bool IsDragging { get; private set; }
	public bool IsHovered { get; private set; }

	public UnityEvent OnMove;

	[SerializeField]
	private UIGrid grid;
	[SerializeField]
	private bool snapOnMove = true;
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
		IsDragging = true;
	}

	public void OnDrag( PointerEventData data )
	{
		draggedPos += data.delta * Blueprinter.Instance.PixelRatio;
		RectTransform.anchoredPosition = snapOnMove ? grid.SnapPosition( draggedPos ) : draggedPos;

		OnMove.Invoke();
	}

	public void OnEndDrag( PointerEventData data )
	{
		IsDragging = false;
	}
}
