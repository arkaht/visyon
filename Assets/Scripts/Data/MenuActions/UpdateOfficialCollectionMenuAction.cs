using UnityEngine;
using Visyon.Wiki;

[CreateAssetMenu( menuName = "Data/MenuAction/Update Official Collection" )]
public class UpdateOfficialCollectionMenuAction : MenuAction
{
	public override void Execute( Blueprinter blueprint ) 
	{
		WikiCollectionUpdater.AsyncUpdate( "Abilities" );
		WikiCollectionUpdater.AsyncUpdate( "Aim_%26_Shoot" );
		//wiki.GetPagesFromCategory( "Patterns", 20 );
	}
}