using Unity.VisualScripting;
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

    public Vector2 SnapPosition( Vector2 pos )
    {
        return new( 
            Mathf.Round( pos.x / SmallGap ) * SmallGap,
            Mathf.Round( pos.y / SmallGap ) * SmallGap
        );
    }

	public void UpdateGrid()
    {
        if ( bigGap <= 50 ) return;
        
        //  clear previous lines
        if ( Application.isPlaying )
            for ( int id = 0; id < transform.childCount; id++ )
            {
                Transform child = transform.GetChild( 0 );
                Destroy( child.gameObject );
            }
        else
            while ( transform.childCount > 0 )
            {
                Transform child = transform.GetChild( 0 );
                DestroyImmediate( child.gameObject );
            }

        int small_gap = SmallGap;
        Vector2 grid_size = rectTransform.rect.size;

        //  generate verticals
        int i = 0;
        Vector2 size = new( lineWide, grid_size.y );
        for ( int x = 0; x < grid_size.x; x += small_gap )
		{
			CreateLine( new( x, 0 ), size, i % bigLineCount == 0 ? bigColor : smallColor );
            i++;
		}

        //  generate horizontals
        i = 0;
        size = new( grid_size.x, lineWide );
        for ( int y = 0; y < grid_size.y; y += small_gap )
		{
			CreateLine( new( 0, y ), size, i % bigLineCount == 0 ? bigColor : smallColor );
            i++;
		}

        print( "Grid::UpdateGrid()" );
        shouldUpdate = false;
	}

	private void CreateLine( Vector2 pos, Vector2 size, Color color )
	{
		GameObject line_obj = Instantiate( linePrefab, transform );
        if ( line_obj.TryGetComponent( out Image line_image ) )
		{
            line_image.color = color;
			line_image.rectTransform.anchoredPosition = pos;
			line_image.rectTransform.sizeDelta = size;
		}
	}
}
