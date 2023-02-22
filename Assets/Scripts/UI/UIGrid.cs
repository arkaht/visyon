using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIGrid : MonoBehaviour
{
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
    private int lineSize = 3;
    [SerializeField]
    private Color bigColor, smallColor;

    private bool shouldUpdate = true;

    private RectTransform rectTransform;

    void Awake()
    {
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
        for ( int id = 0; id <= transform.childCount; id++ )
        {
            Transform child = transform.GetChild( 0 );
            if ( Application.isPlaying )
                Destroy( child.gameObject );
            else
                DestroyImmediate( child.gameObject );
        }

        int small_gap = bigGap / bigLineCount;

        //  generate horizontals
        int i = 0;
        Vector2 size = new( lineSize, canvas.pixelRect.height / canvas.scaleFactor );
        for ( int x = 0; x < canvas.pixelRect.width / canvas.scaleFactor; x += small_gap )
		{
			CreateLine( new( x, 0 ), size, i % bigLineCount == 0 ? bigColor : smallColor );
            i++;
		}

        //  generate verticals
        i = 0;
        size = new( canvas.pixelRect.width / canvas.scaleFactor, lineSize );
        for ( int y = 0; y < canvas.pixelRect.height / canvas.scaleFactor; y += small_gap )
		{
			CreateLine( new( 0, -y ), size, i % bigLineCount == 0 ? bigColor : smallColor );
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
