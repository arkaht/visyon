using UnityEngine;

public enum Axis2D
{
    None,
    X,
    Y,
}

[RequireComponent( typeof( UILineRenderer ) )]
public class UILineConnection : MonoBehaviour
{
    public UILineRenderer Renderer => renderer;
    public ConnectionData Data
    {
        get => data;
        set
        {
            data = value;

            renderer.Thickness = data.Thickness;
            renderer.color = data.Color;
        }
    }

    private ConnectionData data;

    protected new UILineRenderer renderer;

	void Awake()
    {
        renderer = GetComponent<UILineRenderer>();
    }

    public void Connect( Vector2 start, Vector2 end, Axis2D axis )
    {
        //  find prefered axis
        if ( axis == Axis2D.None )
        {
            Connect( start, end );
            return;
        }

        transform.position = start;

        renderer.Points.Clear();
        renderer.Points.Add( Vector3.zero );

        //  middle average
        Vector3 average = ( start + end ) * 0.5f;
		switch ( axis )
		{
			case Axis2D.X:
                if ( start.y != end.y )
                {
                    renderer.Points.Add( transform.InverseTransformPoint( new( average.x, start.y ) ) );
                    renderer.Points.Add( transform.InverseTransformPoint( new( average.x, end.y ) ) );
                }
				break;
			case Axis2D.Y:
                if ( start.x != end.x )
                {
                    renderer.Points.Add( transform.InverseTransformPoint( new( start.x, average.y ) ) );
                    renderer.Points.Add( transform.InverseTransformPoint( new( end.x, average.y ) ) );
                }
				break;
		}
		
        renderer.Points.Add( transform.InverseTransformPoint( end ) );

        //  schedule update
        renderer.SetVerticesDirty();
	}
    public void Connect( Vector2 start, Vector2 end )
    {
        Connect( 
            start, 
            end, 
            Mathf.Abs( start.x - end.x ) > Mathf.Abs( start.y - end.y ) ? Axis2D.X : Axis2D.Y 
        );
    }

    public static UILineConnection Spawn( PatternRelationType relation )
    {
		GameObject obj = new( "Line Connection" ); 
		obj.transform.SetParent( Blueprinter.Instance.ConnectionsTransform );
		obj.AddComponent<CanvasRenderer>();

		UILineConnection connection = obj.AddComponent<UILineConnection>();
        connection.Data = GetConnectionData( relation );

		return connection;
    }

    public static ConnectionData GetConnectionData( PatternRelationType relation )
    {
		return relation switch
		{
			PatternRelationType.None 
         or PatternRelationType.Conflicts => Resources.Load<ConnectionData>( "Data/Connections/DefaultConnectionData" ),
			PatternRelationType.Instantiates 
         or PatternRelationType.InstantiatedBy => Resources.Load<ConnectionData>( "Data/Connections/InstantiateConnectionData" ),
			PatternRelationType.Modulates 
         or PatternRelationType.ModulatedBy => Resources.Load<ConnectionData>( "Data/Connections/ModulateConnectionData" ),
			_ => null,
		};
	}

}
