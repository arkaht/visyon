using System.Collections;
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
		//  reverse start & end pins
		switch ( end.Relation )
		{
			case PatternRelationType.Instantiates:
			case PatternRelationType.Modulates:
				( start, end ) = ( end, start );
				break;
		}

		//  spawn connection
		UIPatternConnection connection = Spawn<UIPatternConnection>( $"Pattern Connection '{start.UIPattern.PatternData.Name}'=>'{end.UIPattern.PatternData.Name}", start.Relation );
		connection.PinStart = start;
		connection.PinEnd = end;
		connection.PreferredAxis = preferred_axis;

		//  update connection
		start.UIPattern.Moveable.OnMove.AddListener( connection.UpdateConnection );
		end.UIPattern.Moveable.OnMove.AddListener( connection.UpdateConnection );
		connection.ScheduleUpdate();

		return connection;
	}

	//  TODO: fix graphic raycasting shape when unity website is up again
	//  https://answers.unity.com/questions/1901509/raycasting-on-ui-graphic-with-custom-shape.html
	public void OnPointerClick( PointerEventData data )
	{
		if ( data.button == deleteButton && Input.GetKey( KeyCode.LeftShift ) )
			PinStart.Disconnect( PinEnd );
	}

	public void UpdateConnection()
	{
		Connect( PinStart.transform.position, PinEnd.transform.position, PreferredAxis );
	}

	private void ScheduleUpdate()
	{
		IEnumerator DoUpdate() {
			yield return new WaitForSeconds( 0.0f );
			UpdateConnection();
		}

		StartCoroutine( DoUpdate() );
	}

	protected override Vector2 GetArrowOffset() => ( PinEnd.transform as RectTransform ).sizeDelta * 0.75f;
}