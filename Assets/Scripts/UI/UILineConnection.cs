using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

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
	public virtual ConnectionData Data
	{
		get => data;
		set
		{
			data = value;

			renderer.Thickness = data.Thickness;
			renderer.color = data.Color;
			arrowImage.color = data.Color;
			arrowImage.sprite = data.ArrowSprite;
			arrowImage.rectTransform.sizeDelta = data.ArrowSize;
		}
	}

	private ConnectionData data;

	protected new UILineRenderer renderer;
	protected Image arrowImage;

	void Awake()
	{
		renderer = GetComponent<UILineRenderer>();
	}

	public void Connect( Vector2 start, Vector2 end, Axis2D axis )
	{
		//  find prefered axis
		if ( axis == Axis2D.None )
			axis = GetPreferredAxis( start, end );

		//  initialize points
		List<Vector2> points = new()
		{
			start
		};

		//  middle average
		Vector3 average = UIGrid.Instance.SnapPosition( ( start + end ) * 0.5f );
		switch ( axis )
		{
			case Axis2D.X:
				if ( start.y != end.y )
				{
					points.Add( new( average.x, start.y ) );
					points.Add( new( average.x, end.y ) );
				}
				break;
			case Axis2D.Y:
				if ( start.x != end.x )
				{
					points.Add( new( start.x, average.y ) );
					points.Add( new( end.x, average.y ) );
				}
				break;
		}
		
		//  add end point
		points.Add( end );

		//  schedule update
		renderer.Points = points;
		renderer.UpdateRenderer();

		//  place arrow
		Vector2 back_dir = ( points[^2] - points[^1] ).normalized;
		arrowImage.rectTransform.position = end + back_dir * GetArrowOffset();
		arrowImage.rectTransform.eulerAngles = new( 0.0f, 0.0f, MathUtils.DirectionalAngle( -back_dir ) );
	}
	public void Connect( Vector2 start, Vector2 end )
	{
		Connect(
			start,
			end,
			GetPreferredAxis( start, end )
		);
	}

	protected virtual Vector2 GetArrowOffset() => Vector2.zero;

	private Axis2D GetPreferredAxis( Vector2 start, Vector2 end )
	{
		return Mathf.Abs( start.x - end.x ) > Mathf.Abs( start.y - end.y ) ? Axis2D.X : Axis2D.Y;
	}

	protected static T Spawn<T>( string name, PatternRelationType relation ) where T : UILineConnection
	{
		GameObject obj = new( name ); 
		obj.transform.SetParent( Blueprinter.Instance.ConnectionsTransform );

		T connection = obj.AddComponent<T>();

		GameObject arrow = new( "Arrow" );
		arrow.transform.SetParent( obj.transform );
		Image arrow_image = arrow.AddComponent<Image>();
		connection.arrowImage = arrow_image;

		return connection;
	}
	public static UILineConnection Spawn( PatternRelationType relation )
	{
		UILineConnection connection = Spawn<UILineConnection>( "Line Connection", relation );
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
