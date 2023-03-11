using UnityEngine;
using UnityEngine.EventSystems;

public class UIPatternConnection : UILineConnection,
								   IPointerClickHandler
{
	public UIPatternPin PinStart, PinEnd;
	public Axis2D PreferredAxis;

	private PointerEventData.InputButton deleteButton = PointerEventData.InputButton.Left;

	public static UIPatternConnection Spawn( UIPatternPin start, UIPatternPin end, Axis2D preferred_axis )
	{
		GameObject obj = new( $"Pattern Connection '{start.UIPattern.PatternData.Name}'=>'{end.UIPattern.PatternData.Name}" ); 
		obj.transform.SetParent( Blueprinter.Instance.ConnectionsTransform );
		obj.AddComponent<CanvasRenderer>();

		UIPatternConnection connection = obj.AddComponent<UIPatternConnection>();
		connection.PinStart = start;
		connection.PinEnd = end;
		connection.PreferredAxis = preferred_axis;
        connection.Data = GetConnectionData( start.Relation );

		return connection;
	}

	//  TODO: fix graphic raycasting shape when unity website is up again
	//  https://answers.unity.com/questions/1901509/raycasting-on-ui-graphic-with-custom-shape.html
	public void OnPointerClick( PointerEventData data )
	{
		print( "pointer click " + data );
		if ( data.button == deleteButton && Input.GetKey( KeyCode.LeftShift ) )
		{
			PinStart.Disconnect( PinEnd );
			print( "unconnect");
		}
	}

	void Update()
	{
		Connect( PinStart.transform.position, PinEnd.transform.position, PreferredAxis );
	}
}