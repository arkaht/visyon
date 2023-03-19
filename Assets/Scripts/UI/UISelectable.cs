using System.Collections;
using UnityEngine;

public class UISelectable : MonoBehaviour
{
	public UIMoveable Moveable => moveable;

	public bool IsSelected { get; private set; }

	[SerializeField]
	private UIMoveable moveable;
	[SerializeField]
	private Transform SelectionFeedback;

	void Start()
	{
		OnUnSelected();
	}

	public void SetSelected( bool is_selected )
	{
		if ( IsSelected == is_selected ) return;

		IsSelected = is_selected;

		if ( IsSelected )
			OnSelected();
		else
			OnUnSelected();
	} 

	public virtual void OnSelected() 
	{
		if ( SelectionFeedback != null )
			SelectionFeedback.gameObject.SetActive( true );
	}
	public virtual void OnUnSelected() 
	{
		if ( SelectionFeedback != null )
			SelectionFeedback.gameObject.SetActive( false );
	}
}