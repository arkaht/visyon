using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Blueprinter : MonoBehaviour, 
						   IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler, IPointerClickHandler
{
	public static Blueprinter Instance { get; private set; }

	public RectTransform ContentTransform => contentTransform;
	public RectTransform OverlayTransform => overlayTransform;
	
	public float Width => canvas.pixelRect.width;
	public float Height => canvas.pixelRect.height;
	public float UnscaledWidth => canvas.pixelRect.width / canvas.scaleFactor;
	public float UnscaledHeight => canvas.pixelRect.height / canvas.scaleFactor;
	public float PixelRatio => 1920.0f / camera.pixelWidth * 1.0f / canvas.scaleFactor * GetCurrentZoomSize() / GetDefaultZoomSize();

	[Header( "References" )]
	[SerializeField]
	private Canvas canvas;
	[SerializeField]
	private new Camera camera;
	[SerializeField]
	private RectTransform contentTransform, overlayTransform;

	[Header( "Settings" )]
	[SerializeField]
	private PointerEventData.InputButton dragButton;
	[SerializeField]
	private PointerEventData.InputButton searchButton;
	[SerializeField]
	private float moveMultiplier = 1.0f, zoomMultiplier = 1.0f;
	[SerializeField]
	private float[] zoomLevels = new float[] { 350.0f, 500.0f, 750.0f, 1080.0f };
	[SerializeField]
	private int defaultZoomLevelID = 1;

	private float zoomLevel = 1.0f;
	private UINodeSearcher currentSearcher;
	private bool shouldSpawnSearcher = false;

	public Vector2 GetScreenMousePosition() => Input.mousePosition;
	public Vector2 GetWorldMousePosition( bool is_canvas_relative = false, bool is_absolute = false )
	{
		Vector2 pos = ScreenToWorld( GetScreenMousePosition() );

		if ( is_canvas_relative )
			pos = (Vector2) transform.InverseTransformVector( pos );

		if ( is_absolute )
			pos -= contentTransform.offsetMin;

		return pos;
	}

	public Vector2 WorldToScreen( Vector2 world_pos ) => camera.WorldToScreenPoint( world_pos );
	public Vector2 ScreenToWorld( Vector2 world_pos ) => camera.ScreenToWorldPoint( world_pos );

	public void ResetZoomToDefault()
	{
		zoomLevel = defaultZoomLevelID;
		camera.orthographicSize = GetDefaultZoomSize();
	}
	public float GetCurrentZoomSize() => GetZoomSize( Mathf.FloorToInt( zoomLevel ) );
	public float GetDefaultZoomSize() => zoomLevels[defaultZoomLevelID];
	public float GetZoomSize( int level ) => zoomLevels[level];

	public void SpawnNodeSearcherAtMousePosition()
	{
		Vector2 mouse_pos = GetScreenMousePosition();

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
	public void DestroySearcher()
	{
		if ( currentSearcher == null ) return;
		Destroy( currentSearcher.gameObject );
	}

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		PatternRegistery.LoadAll();

		ResetZoomToDefault();
	}

	void Update()
	{
		if ( shouldSpawnSearcher )
		{
			SpawnNodeSearcherAtMousePosition();
			shouldSpawnSearcher = false;
		}
	}

	public void OnBeginDrag( PointerEventData data ) {}
	public void OnDrag( PointerEventData data )
	{
		DestroySearcher();

		if ( data.button != dragButton ) 
		{
			return;
		}

		camera.transform.position -= PixelRatio * moveMultiplier * (Vector3) data.delta;
	}
	public void OnEndDrag( PointerEventData data )
	{
		if ( data.button != dragButton ) return;

		if ( dragButton == searchButton )
			shouldSpawnSearcher = false;
	}

	public void OnPointerClick( PointerEventData data )
	{
		if ( data.button != searchButton )
		{
			DestroySearcher();
			return;
		}

		shouldSpawnSearcher = true;
	}

	public void OnScroll( PointerEventData data )
	{
		float old_zoom = camera.orthographicSize;
		Vector3 zoom_pos = GetWorldMousePosition();

		//  zoom
		zoomLevel = Mathf.Clamp( zoomLevel - data.scrollDelta.y * zoomMultiplier, 0.0f, zoomLevels.Length - 1.0f );
		camera.orthographicSize = GetCurrentZoomSize();

		//  move relative to cursor
		if ( camera.orthographicSize != old_zoom )
		{
			//  get cursor to camera' center offset
			Vector3 offset = zoom_pos - camera.transform.position;
			offset.z = 0.0f;

			//  zoom in
			float zoom_ratio;
			if ( data.scrollDelta.y > 0.0f )
				zoom_ratio = 1.0f / ( old_zoom / camera.orthographicSize );
			//  zoom out
			else
				zoom_ratio = camera.orthographicSize / old_zoom;

			//  apply offset
			camera.transform.position += offset - offset * zoom_ratio;
		}
	}
}