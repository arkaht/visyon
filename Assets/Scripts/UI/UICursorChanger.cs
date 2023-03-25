using UnityEngine;
using UnityEngine.EventSystems;

public class UICursorChanger : MonoBehaviour,
							   IPointerEnterHandler, IPointerExitHandler
{
	public bool IsPointerIn { get; set; }

	public CursorData Data;
	public KeyCode AdditionalHoldKey = KeyCode.None;

	public void OnPointerEnter( PointerEventData data )
	{
		IsPointerIn = true;
	}

	public void OnPointerExit( PointerEventData data )
	{
		if ( !data.fullyExited ) return;

		CursorWrapper.Data = null;
		IsPointerIn = false;
	}

	void Update()
	{
		if ( !IsPointerIn ) return;

		//  check additional hold key
		if ( AdditionalHoldKey != KeyCode.None && !Input.GetKey( AdditionalHoldKey ) )
		{
			CursorWrapper.Data = null;
			return;
		}

		CursorWrapper.Data = Data;
	}

	void OnDestroy()
	{
		if ( IsPointerIn )
			CursorWrapper.Data = null;
	}
}