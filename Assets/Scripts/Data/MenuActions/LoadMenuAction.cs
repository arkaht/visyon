using UnityEngine;

[CreateAssetMenu( menuName = "Data/MenuAction/Load" )]
public class LoadMenuAction : MenuAction
{
	public override void Execute( Blueprinter blueprint ) 
	{
		blueprint.Load( "save-test-00.json" );
	}
}