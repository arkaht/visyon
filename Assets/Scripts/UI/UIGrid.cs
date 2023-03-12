using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIGrid : MonoBehaviour
{
    public static UIGrid Instance { get; private set; }

    public Canvas Canvas => canvas;
    public int BigGap => bigGap;
    public int SmallGap => bigGap / bigLineCount;

    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private int bigGap = 250;
    [SerializeField]
    private int bigLineCount = 10;
    [SerializeField]
    private GameObject linePrefab;
    [SerializeField]
    private int lineWide = 2;
    [SerializeField]
    private Color bigColor, smallColor;

    private bool shouldUpdate = true;
    private int currentLineID = -1;

    private RectTransform rectTransform;

    void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();
    }

	void OnValidate()
	{
		shouldUpdate = true;
	}

    void Update()
    {
        if ( shouldUpdate )
            UpdateGrid();
    }

	private void OnRectTransformDimensionsChange()
	{
        shouldUpdate = true;
	}

	public Vector2Int WorldToTile( Vector2 pos )
    {
        float small_gap = SmallGap;
        return new(
          Mathf.RoundToInt( pos.x / small_gap ),
          Mathf.RoundToInt( pos.y / small_gap )
        );
    }

    public Vector2 SnapPosition( Vector2 pos )
    {
        float small_gap = SmallGap;
        return new( 
            Mathf.Round( pos.x / small_gap ) * small_gap,
            Mathf.Round( pos.y / small_gap ) * small_gap
        );
    }

	public void UpdateGrid()
    {
        if ( bigGap <= 50 ) return;

        currentLineID = 0;

        int small_gap = SmallGap;
        Vector2 grid_pos = rectTransform.position;
        Vector2 grid_size = rectTransform.rect.size;

        Vector2Int min_tile_pos = WorldToTile( grid_pos );
        Vector2Int max_tile_pos = WorldToTile( grid_pos + grid_size );

        //  generate verticals
        Vector2 size = new( lineWide, grid_size.y );
        for ( int i = min_tile_pos.x; i < max_tile_pos.x; i++ )
        {
            CreateLine( 
                new Vector3( i * small_gap - lineWide / 2.0f, rectTransform.position.y ), 
                size, 
                i % bigLineCount == 0 ? bigColor : smallColor
            );
            currentLineID++;
        }

        //  generate horizontals
        size = new( grid_size.x, lineWide );
        for ( int i = min_tile_pos.y; i < max_tile_pos.y; i++ )
        {
            CreateLine( 
                new Vector3( rectTransform.position.x, i * small_gap - lineWide / 2.0f ), 
                size, 
                i % bigLineCount == 0 ? bigColor : smallColor
            );
            currentLineID++;
        }

        //  clear previous lines
        if ( Application.isPlaying )
            for ( int id = currentLineID; id < transform.childCount; id++ )
            {
                Transform child = transform.GetChild( currentLineID );
                Destroy( child.gameObject );
            }
        else
            while ( transform.childCount > currentLineID )
            {
                Transform child = transform.GetChild( currentLineID );
                DestroyImmediate( child.gameObject );
            }

        print( "Grid::UpdateGrid()" );
        shouldUpdate = false;
	}

	private void CreateLine( Vector2 pos, Vector2 size, Color color )
	{
		Transform line_transform = currentLineID >= 0 && transform.childCount > currentLineID
                                 ? transform.GetChild( currentLineID ) 
                                 : Instantiate( linePrefab, transform ).transform;

        if ( line_transform.TryGetComponent( out Image line_image ) )
		{
            line_image.color = color;
			line_image.rectTransform.position = pos;
			line_image.rectTransform.sizeDelta = size;
		}
	}
}
