using UnityEngine;
using UnityEngine.EventSystems;

public class UIPatternPin : MonoBehaviour,
							IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[SerializeField]
	private UIPattern uiPattern;

	public void OnBeginDrag( PointerEventData data )
	{
	}

	public void OnDrag( PointerEventData data )
	{
	}

	public void OnEndDrag( PointerEventData data )
	{
		/*print( "end, spawning searcher" );
		data.hovered.ForEach( print );*/

		PatternRelations relations = uiPattern.PatternData.Relations;

		UINodeSearcher searcher = Blueprinter.Instance.SpawnNodeSearcherAtMousePosition();
		searcher.AddPatterns( relations.Instantiates, "Instantiates" );
		searcher.AddPatterns( relations.Conflicts, "Potential Conflicts" );
		searcher.AddPatterns( relations.Modulates, "Modulates" );
		searcher.AddPatterns( relations.InstantiatedBy, "Instantiated By" );
		searcher.AddPatterns( relations.ModulatedBy, "Modulated By" );
	}
}