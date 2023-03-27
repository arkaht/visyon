using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMenuTab : MonoBehaviour,
						 IPointerEnterHandler, IPointerExitHandler
{
	[Header( "References" )]
	[SerializeField]
	private UIMenu menu;
	[SerializeField]
	private Transform spawn;

	[Header( "Settings" )]
	[SerializeField]
	private List<MenuAction> actions;

	public void OnPointerEnter( PointerEventData data )
	{
		menu.Show( spawn.position, actions );
	}
	public void OnPointerExit( PointerEventData data ) => menu.Hide();
}