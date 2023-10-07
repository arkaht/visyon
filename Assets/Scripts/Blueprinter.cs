using SimpleJSON;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Visyon.Wiki;

public class Blueprinter : MonoBehaviour, 
						   IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler, IPointerClickHandler
{
	public const string FILE_EXTENSION = "vsy";

	public static Blueprinter Instance
	{
		get {
			if ( instance == null )
				instance = FindObjectOfType<Blueprinter>();
			return instance;
		}
	}
	private static Blueprinter instance;

	public string FilePath { get; set; } = "";
	public bool IsDragging { get; private set; }
	public bool IsSelecting { get; private set; }
	public bool IsMoving { get; private set; }
	public RectTransform ContentTransform => contentTransform;
	public RectTransform OverlayTransform => overlayTransform;
	public RectTransform ConnectionsTransform => connectionsTransform;
	public RectTransform PatternsTransform => patternsTransform;
	public UIPatternSearcher Searcher => searcher;
	public UIPatternViewer Viewer => viewer;
	
	public Camera Camera => camera;
	public Vector2 CameraSize => camera.pixelRect.size;
	public float ScreenRatio => 1920.0f / camera.pixelWidth;
	public float PixelRatio => ScreenRatio * 1.0f / canvas.scaleFactor * GetCurrentZoomSize() / GetDefaultZoomSize();

	public IReadOnlyCollection<UISelectable> Selection => selection;

	[Header( "References" )]
	[SerializeField]
	private Canvas canvas;
	[SerializeField]
	private new Camera camera;
	[SerializeField]
	private RectTransform contentTransform, overlayTransform,
						  connectionsTransform, patternsTransform;
	[SerializeField]
	private UISelectionRect selectionRect;
	[SerializeField]
	private UIPatternSearcher searcher;
	[SerializeField]
	private UIPatternViewer viewer;

	[Header( "Settings" )]
	[SerializeField, Tooltip( "Button to drag the camera around" )]
	private PointerEventData.InputButton dragButton;
	[SerializeField, Tooltip( "Button to create the search menu" )]
	private PointerEventData.InputButton searchButton;
	[SerializeField, Tooltip( "Button to create a selection & to move selectable objects around" )]
	private PointerEventData.InputButton selectButton;
	[SerializeField]
	private float moveMultiplier = 1.0f, zoomMultiplier = 1.0f;
	[SerializeField]
	private float[] zoomLevels = new float[] { 350.0f, 500.0f, 750.0f, 1080.0f };
	[SerializeField]
	private int defaultZoomLevelID = 1;

	private float zoomLevel = 1.0f;
	private bool shouldSpawnSearcher = false;
	private readonly HashSet<UISelectable> selection = new();
	private readonly HashSet<IJSONSerializable> serializables = new();

	private const KeyCode ADDITIVE_SELECTION_KEY = KeyCode.LeftShift;

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
	public Vector2 ScreenToWorld( Vector2 screen_pos ) => camera.ScreenToWorldPoint( screen_pos );

	public void ResetZoomToDefault()
	{
		zoomLevel = defaultZoomLevelID;
		camera.orthographicSize = GetDefaultZoomSize();
	}
	public float GetCurrentZoomSize() => GetZoomSize( Mathf.FloorToInt( zoomLevel ) );
	public float GetDefaultZoomSize() => zoomLevels[defaultZoomLevelID];
	public float GetZoomSize( int level ) => zoomLevels[level];
	
	public bool AddToSelection( UISelectable selected )
	{
		if ( selected == null ) return false;
		if ( !selection.Add( selected ) ) return false;

		selected.SetSelected( true );
		return true;
	}
	public void AddInterconnectionsToSelection( UIPattern pattern, HashSet<UIPattern> close_list = null )
	{
		//  init close list
		close_list ??= new();
		if ( close_list.Contains( pattern ) ) return;  //  prevent stack overflow

		//  add self
		AddToSelection( pattern.Selectable );
		close_list.Add( pattern );

		//  add all connections
		foreach ( UIPatternPin pin in pattern.RelationPins.Values )
		{
			foreach ( UIPatternConnection connection in pin.Connections.Values )
			{
				UIPatternPin next_pin = connection.PinStart == pin ? connection.PinEnd : connection.PinStart;
				AddInterconnectionsToSelection( next_pin.UIPattern, close_list );
			}
		}
	}
	public bool RemoveFromSelection( UISelectable selected )
	{
		if ( selected == null ) return false;
		if ( !selection.Remove( selected ) ) return false;

		selected.SetSelected( false );
		return true;
	}
	public void ClearSelection()
	{
		foreach ( UISelectable selected in selection.ToList() )
			RemoveFromSelection( selected );
	}
	public void DeleteSelection()
	{
		foreach ( UISelectable selected in selection )
			Destroy( selected.gameObject );

		selection.Clear();
	}

	public UIPatternSearcher ShowSearcherAtMousePosition()
	{
		searcher.Clear();
		searcher.Show();

		//  update position
		Vector2 mouse_pos = GetScreenMousePosition();
		searcher.SpawnPosition = mouse_pos;
		searcher.transform.position = mouse_pos;

		//  auto-focus
		searcher.FocusSearchField();

		//  offset position to prevent off-screens
		RectTransform rect_transform = (RectTransform) searcher.transform;
		Vector2 top_right = mouse_pos + rect_transform.sizeDelta / ScreenRatio;
		if ( top_right.x > camera.pixelWidth )
			rect_transform.anchoredPosition -= new Vector2( rect_transform.sizeDelta.x, 0.0f );
		if ( top_right.y > camera.pixelHeight )
			rect_transform.anchoredPosition -= new Vector2( 0.0f, rect_transform.sizeDelta.y );

		return searcher;
	}

	public bool AddSerializable( IJSONSerializable serializable ) => serializables.Add( serializable );
	public bool RemoveSerializable( IJSONSerializable serializable ) => serializables.Remove( serializable );
	public void Save( string path )
	{
		JSONArray array = new();
		foreach ( IJSONSerializable serializable in serializables )
		{
			JSONNode node = serializable.Serialize();
			array.Add( node );
		}

		File.WriteAllText( path, array.ToString( 4 ) );
		FilePath = path;

		print( $"Blueprinter: saved to '{path}'!" );
	}
	public void Load( string path )
	{
		Clear();

		//  init variables
		Vector3Average pos_average = new();

		//  load file
		string json = File.ReadAllText( path );
		JSONArray array = JSON.Parse( json ).AsArray;
		foreach ( JSONObject obj in array )
		{
			int num_type = obj["type"].AsInt;
			switch ( (SerializableType)num_type )
			{
				case SerializableType.Pattern:
					UIPattern pattern = UIPattern.Load( obj );
					pos_average.Add( pattern.transform.position );
					break;
				default:
					Debug.LogError( $"Blueprinter::Load: unknown type '{num_type}'!" );
					break;
			}
		}

		//  center camera on average
		Vector3 camera_pos = pos_average.Result;
		camera_pos.z = camera.transform.position.z;
		camera.transform.position = camera_pos;

		FilePath = path;
	}
	public void Clear()
	{
		UniqueID.ResetNextID();

		TransformUtils.Clear( patternsTransform );
		TransformUtils.Clear( connectionsTransform );

		selection.Clear();
	}

	void Awake()
	{
		PatternRegistery.RegisterCollection( WikiCollectionUpdater.CollectionName );

		instance = this;
	}

	void Start()
	{
		ResetZoomToDefault();
	}

	void Update()
	{
		//  spawn searcher
		if ( shouldSpawnSearcher )
		{
			ShowSearcherAtMousePosition().SetActivePatterns( PatternRegistery.AllKeys );
			shouldSpawnSearcher = false;
		}

		//  delete selection
		if ( !searcher.isActiveAndEnabled )
		{
			if ( Input.GetKeyDown( KeyCode.Backspace ) || Input.GetKeyDown( KeyCode.Delete ) )
				DeleteSelection();
		}
	}

	public void OnBeginDrag( PointerEventData data )
	{
		if ( data.button != selectButton ) return;

		Vector2 pos = ScreenToWorld( data.position );
		UISelectable hovered = selectionRect.FindAtPosition<UISelectable>( pos );

		//  do selection
		if ( hovered == null )
		{
			selectionRect.StartPos = ScreenToWorld( data.position );
			IsSelecting = true;
			return;
		}

		//  begin moving
		if ( !hovered.IsSelected )  //  TODO
			ClearSelection();

		//  select hovered if selection is empty
		if ( selection.Count == 0 )
			AddToSelection( hovered );

		//  drag selection
		foreach ( UISelectable selectable in selection )
			selectable.Moveable.OnBeginDrag( data );

		IsMoving = true;
	}
	public void OnDrag( PointerEventData data )
	{
		searcher.Hide();

		if ( data.button == dragButton ) 
		{
			camera.transform.position -= PixelRatio * moveMultiplier * (Vector3) data.delta;
			IsDragging = true;
		}
		else if ( data.button == selectButton )
		{
			if ( IsSelecting )
				selectionRect.EndPos = ScreenToWorld( data.position );
			else if ( !IsDragging )
				foreach ( UISelectable selectable in selection )
					selectable.Moveable.OnDrag( data );
		}
	}
	public void OnEndDrag( PointerEventData data )
	{
		if ( data.button == dragButton )
		{
			if ( dragButton == searchButton )
				shouldSpawnSearcher = false;

			IsDragging = false;
		}
		else if ( data.button == selectButton )
		{
			if ( IsSelecting )
			{
				bool is_additive = Input.GetKey( ADDITIVE_SELECTION_KEY );
				if ( !is_additive )
					ClearSelection();

				foreach( UISelectable selectable in selectionRect.Find<UISelectable>() )
					AddToSelection( selectable );

				selectionRect.Reset();
				IsSelecting = false;
			}
			else
				foreach ( UISelectable selectable in selection )
					selectable.Moveable.OnEndDrag( data );
		}
	}

	private void HandleSelectionAtClick( PointerEventData data )
	{
		bool is_additive = Input.GetKey( ADDITIVE_SELECTION_KEY );
		if ( !is_additive )
			ClearSelection();

		//  try to select at mouse position
		Vector2 pos = ScreenToWorld( data.position );
		UISelectable hovered = selectionRect.FindAtPosition<UISelectable>( pos );
		if ( hovered != null )
		{
			//  additive control
			if ( is_additive )
			{
				//  select inter-connections
				if ( data.clickCount == 2 )
				{
					if ( hovered.TryGetComponent( out UIPattern pattern ) )
					{
						AddInterconnectionsToSelection( pattern );
					}
				}
				//  invert selection
				else if ( hovered.IsSelected )
				{
					RemoveFromSelection( hovered );
					return;
				}
			}
			//  double click
			else if ( data.clickCount == 2 )
			{
				hovered.OnDoubleClick.Invoke();
			}

			AddToSelection( hovered );
		}
	}
	public void OnPointerClick( PointerEventData data )
	{
		//  stop moving selection
		if ( IsMoving )
			IsMoving = false;
		//  select node at position
		else if ( !IsSelecting && data.button == selectButton )
		{
			HandleSelectionAtClick( data );
		}

		//  populate searcher
		if ( data.button != searchButton )
			searcher.Hide();
		else
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