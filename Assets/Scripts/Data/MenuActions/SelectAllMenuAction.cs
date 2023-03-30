using UnityEngine;

[CreateAssetMenu( menuName = "Data/MenuAction/SelectAll" )]
public class SelectAllMenuAction : MenuAction
{
	public override void Execute( Blueprinter blueprint ) 
	{
		foreach ( UISelectable selectable in FindObjectsOfType<UISelectable>() )
			blueprint.AddToSelection( selectable );
	}
}