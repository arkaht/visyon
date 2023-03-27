using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMenuTab : MonoBehaviour,
						 IPointerEnterHandler, IPointerExitHandler
{
	public Vector3 SpawnPoint => spawn.position;
	public List<MenuAction> Actions => actions;

	[Header( "References" )]
	[SerializeField]
	private UIMenu menu;
	[SerializeField]
	private Transform spawn;
	[SerializeField]
	private UIHoverStyle hoverStyle;

	[Header( "Settings" )]
	[SerializeField]
	private List<MenuAction> actions;

	public void OnPointerEnter( PointerEventData data )
	{
		menu.Show( this );
	}
	public void OnPointerExit( PointerEventData data ) 
	{
		menu.Hide();
	}

	void Start()
	{
		hoverStyle.enabled = false;
		menu.OnShow.AddListener( 
			( tab ) => {
				hoverStyle.IsHovered = tab == this;
			} 
		);
		menu.OnHide.AddListener( 
			( tab ) => {
				if ( tab != this ) return; 
				hoverStyle.IsHovered = false;
			} 
		);
	}
}