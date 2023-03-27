using UnityEngine;

[CreateAssetMenu( menuName = "Data/MenuAction/Save" )]
public class SaveMenuAction : MenuAction
{
	public override void Execute( Blueprinter blueprint ) 
	{
		blueprint.Save( "save-test-00.json" );
	}
}