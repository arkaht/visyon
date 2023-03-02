using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Blueprinter : MonoBehaviour, 
						   IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
	public static Blueprinter Instance { get; private set; }

	public RectTransform ContentTransform => contentTransform;
	
	public float Width => canvas.pixelRect.width;
	public float Height => canvas.pixelRect.height;
	public float UnscaledWidth => canvas.pixelRect.width / canvas.scaleFactor;
	public float UnscaledHeight => canvas.pixelRect.height / canvas.scaleFactor;
	public float PixelRatio => 1920.0f / camera.pixelWidth * 1.0f / canvas.scaleFactor * GetCurrentZoomSize() / GetDefaultZoomSize();

	[Header( "References" )]
	[SerializeField]
	private Canvas canvas;
	[SerializeField]
	private Camera camera;
	[SerializeField]
	private RectTransform contentTransform;

	[Header( "Settings" )]
	[SerializeField]
	private PointerEventData.InputButton inputButton;
	[SerializeField]
	private float moveMultiplier = 1.0f, zoomMultiplier = 1.0f;
	[SerializeField]
	private float[] zoomLevels = new float[] { 350.0f, 500.0f, 750.0f, 1080.0f };
	[SerializeField]
	private int defaultZoomLevelID = 1;

	private float zoomLevel = 1.0f;

	private UINodeSearcher currentSearcher;

	public Vector2 GetMousePosition( bool is_canvas_relative = true, bool is_absolute = false )
	{
		Vector2 pos = Input.mousePosition;

		if ( is_canvas_relative )
			pos = (Vector2)transform.InverseTransformVector( camera.ScreenToWorldPoint( pos ) );

		if ( is_absolute )
			pos -= contentTransform.offsetMin;

		return pos;
	}

	public Vector2 ToScreenPosition( Vector2 world_pos )
	{
		return camera.WorldToScreenPoint( world_pos );
	}

	public float GetCurrentZoomSize() => GetZoomSize( Mathf.FloorToInt( zoomLevel ) );
	public float GetDefaultZoomSize() => zoomLevels[defaultZoomLevelID];
	public float GetZoomSize( int level ) => zoomLevels[level];

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		PatternRegistery.LoadAll();

		camera.orthographicSize = GetDefaultZoomSize();
	}

	void Update()
	{
		//  auto-focus when typing, may not be useful 
		/*if ( currentSearcher != null && !currentSearcher.IsSearchFieldFocused )
		{
			for ( int i = 97; i <= 122; i++ )
			{
				if ( Input.GetKeyDown( (KeyCode) i ) )
				{
					currentSearcher.FocusSearchField();
					break;
				}
			}
		}*/
		//print( camera.pixelRect );
		//  spawn node searcher on right click
		if ( Input.GetMouseButtonDown( 1 ) )
		{
			Vector2 mouse_pos = GetMousePosition();

			//  create it..
			if ( currentSearcher == null )
			{
				currentSearcher = UINodeSearcher.Spawn( mouse_pos );
				currentSearcher.AddAllPatterns();
			}
			//  ..or update position
			else
			{
				currentSearcher.SpawnPosition = mouse_pos;
				currentSearcher.transform.position = mouse_pos;
			}

			//  auto-focus
			currentSearcher.FocusSearchField();

			//  offset Y position to prevent off-screens
			RectTransform rect_transform = (RectTransform) currentSearcher.transform;
			float y_diff = UnscaledHeight - ( rect_transform.anchoredPosition.y + rect_transform.sizeDelta.y );
			if ( y_diff <= 0.0f )
				rect_transform.anchoredPosition += new Vector2( 0.0f, -rect_transform.sizeDelta.y );
		}
	}

	public void OnBeginDrag( PointerEventData data )
	{
	}

	public void OnDrag( PointerEventData data )
	{
		if ( data.button != inputButton ) return;

		camera.transform.position -= PixelRatio * moveMultiplier * (Vector3) data.delta;
	}

	public void OnEndDrag( PointerEventData data )
	{
	}

	public void OnScroll( PointerEventData data )
	{
		float old_zoom = camera.orthographicSize;
		Vector3 zoom_pos = camera.ScreenToWorldPoint( Input.mousePosition );

		//  zoom
		zoomLevel = Mathf.Clamp( zoomLevel - data.scrollDelta.y * zoomMultiplier, 0.0f, zoomLevels.Length - 1.0f );
		camera.orthographicSize = GetCurrentZoomSize();

		//  move relative to cursor
		if ( camera.orthographicSize != old_zoom )
		{
			float zoom_ratio = old_zoom / camera.orthographicSize;

			//  get cursor to camera' center offset
			Vector3 offset = zoom_pos - camera.transform.position;
			offset.z = 0.0f;

			//  zoom in
			if ( data.scrollDelta.y > 0.0f )
			{
				Vector3 diff_offset = offset / zoom_ratio;
				camera.transform.position += offset - diff_offset;
			}
			//  zoom out
			else
			{
				Vector3 diff_offset = offset * zoom_ratio;
				camera.transform.position -= offset - diff_offset;
			}
		}
	}
}