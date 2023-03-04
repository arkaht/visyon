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
		searcher.AddPatterns( relations.Instantiates );
		searcher.AddPatterns( relations.Conflicts );
		searcher.AddPatterns( relations.Modulates );
		searcher.AddPatterns( relations.InstantiatedBy );
		searcher.AddPatterns( relations.ModulatedBy );
	}
}