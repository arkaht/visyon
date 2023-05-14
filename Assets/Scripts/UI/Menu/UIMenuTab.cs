using System;
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

	private MenuAction[] sortedByShortcutsActions;

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
		//  sort actions by shortcuts:
		//  allow us to avoid prioritizing wrong actions having similar keys
		//  (e.g. 'Ctrl + S' for 'Save' and 'Ctrl + Shift + S' for 'Save As')
		sortedByShortcutsActions = actions.ToArray();
		Array.Sort( sortedByShortcutsActions, new MenuActionShortcutsComparer() );

		//  appearance
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

	void Update()
	{
		//  handle actions shortcut
		foreach ( MenuAction action in sortedByShortcutsActions )
		{
			if ( action.ShortcutKeys.Count == 0 ) continue;

			//  check pressing
			bool is_user_pressing = true;
			foreach ( ShortcutKey shortcut in action.ShortcutKeys )
			{
				//  check input
				if ( shortcut.ShouldHold ? Input.GetKey( shortcut.Key ) : Input.GetKeyDown( shortcut.Key ) ) 
					continue;

				is_user_pressing = false;
				break;
			}

			//  execute action
			if ( is_user_pressing )
			{
				action.Execute( Blueprinter.Instance );
				break;
			}
		}
	}
}