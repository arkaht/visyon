using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPatternConnection : UILineConnection,
								   IPointerClickHandler
{
	public override ConnectionData Data
	{
		get => base.Data;
		set
		{
			base.Data = value;

			if ( cursorChanger != null )
			{
				cursorChanger.Data = Data.Cursor;
				cursorChanger.AdditionalHoldKey = ADDITIONAL_HOLD_KEY;
			}
		}
	}

	public UIPatternPin PinStart, PinEnd;
	public Axis2D PreferredAxis;

	private UICursorChanger cursorChanger;

	private const PointerEventData.InputButton DELETE_BUTTON = PointerEventData.InputButton.Left;
	private const KeyCode ADDITIONAL_HOLD_KEY = KeyCode.LeftShift;

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

		//  mouse cursor
		UICursorChanger cursor_changer = connection.AddComponent<UICursorChanger>();
		connection.cursorChanger = cursor_changer;

		connection.Data = GetConnectionData( start.Relation );
		return connection;
	}

	public void OnPointerClick( PointerEventData data )
	{
		if ( data.button == DELETE_BUTTON && Input.GetKey( ADDITIONAL_HOLD_KEY ) )
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