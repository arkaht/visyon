using UnityEngine;
using Visyon.Wiki;

[CreateAssetMenu( menuName = "Data/MenuAction/Update Official Collection" )]
public class UpdateOfficialCollectionMenuAction : MenuAction
{
	public override void Execute( Blueprinter blueprint ) 
	{
		//WikiCollectionUpdater.AsyncRetrievePatternsIDs();
		WikiCollectionUpdater.AsyncUpdateAll();
		//wiki.GetPagesFromCategory( "Patterns", 20 );
	}
}