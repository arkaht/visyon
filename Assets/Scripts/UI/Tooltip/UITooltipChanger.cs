using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITooltipChanger : MonoBehaviour,
								IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
{
	public string Name = "Name";
	public string Text = "Tooltip";

	[SerializeField]
	private Button button;

	private UITooltip currentTooltip;

	public bool IsInteractable()
	{
		if ( button != null )
			return button.interactable;

		return true;
	}

	public void OnPointerEnter( PointerEventData data )
	{
		currentTooltip = UITooltip.Instance;
		if ( currentTooltip == null ) return;

		currentTooltip.Show( data.position, Name, Text );
	}

	public void OnPointerMove( PointerEventData data )
	{
		if ( currentTooltip == null ) return;
		currentTooltip.Move( data.position );
	}

	public void OnPointerExit( PointerEventData data )
	{
		if ( currentTooltip == null ) return;
		currentTooltip.Hide();
		currentTooltip = null;
	}

	void Update()
	{
		if ( currentTooltip == null ) return;
		currentTooltip.SetHighlight( IsInteractable() );
	}

	void OnDisable()
	{
		OnPointerExit( null );
	}
}