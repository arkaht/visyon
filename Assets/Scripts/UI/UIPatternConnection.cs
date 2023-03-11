using System.Collections;
using UnityEngine;

public class UIPatternConnection : UILineConnection
{
	public UIPatternPin PinStart, PinEnd;
	public Axis2D PreferredAxis;

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

	void Update()
	{
		Connect( PinStart.transform.position, PinEnd.transform.position, PreferredAxis );
	}
}