using System.Collections;
using UnityEngine;

public class UIPatternConnection : UILineConnection
{
	public UIPatternPin PinStart, PinEnd;

	public static UIPatternConnection Spawn( UIPatternPin start, UIPatternPin end )
	{
		GameObject obj = new( "Pattern Connection" ); 
		obj.transform.SetParent( Blueprinter.Instance.ConnectionsTransform );
		obj.AddComponent<CanvasRenderer>();

		UIPatternConnection connection = obj.AddComponent<UIPatternConnection>();
		connection.PinStart = start;
		connection.PinEnd = end;
        connection.Data = GetConnectionData( start.Relation );

		return connection;
	}

	void Update()
	{
		Connect( PinStart.transform.position, PinEnd.transform.position );
	}
}