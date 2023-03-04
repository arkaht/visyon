using UnityEngine;
using UnityEngine.EventSystems;

public class UIPatternPin : MonoBehaviour,
							IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[SerializeField]
	private UIPattern uiPattern;

	[SerializeField]
	private PointerEventData.InputButton inputButton;

	public void OnBeginDrag( PointerEventData data )
	{
	}

	public void OnDrag( PointerEventData data )
	{
		if ( data.button != inputButton )
		{
			Blueprinter.Instance.OnDrag( data );
			return;
		}

	}

	public void OnEndDrag( PointerEventData data )
	{
		if ( data.button != inputButton )
		{
			Blueprinter.Instance.OnEndDrag( data );
			return;
		}

		PatternRelations relations = uiPattern.PatternData.Relations;

		UINodeSearcher searcher = Blueprinter.Instance.SpawnNodeSearcherAtMousePosition();
		searcher.AddPatterns( relations.Instantiates, "Instantiates" );
		searcher.AddPatterns( relations.Conflicts, "Potential Conflicts" );
		searcher.AddPatterns( relations.Modulates, "Modulates" );
		searcher.AddPatterns( relations.InstantiatedBy, "Instantiated By" );
		searcher.AddPatterns( relations.ModulatedBy, "Modulated By" );
	}
}