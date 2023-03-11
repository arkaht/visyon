using UnityEngine;
using UnityEngine.EventSystems;

public class UIPatternPin : MonoBehaviour,
							IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[SerializeField]
	private UIPattern uiPattern;

	[SerializeField]
	private PointerEventData.InputButton inputButton;
	[SerializeField]
	private PatternRelationType relationsFilter = PatternRelationType.None;

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
		if ( relationsFilter.HasFlag( PatternRelationType.Instantiates ) )
			searcher.AddPatterns( relations.Instantiates, "Instantiates" );
		if ( relationsFilter.HasFlag( PatternRelationType.Conflicts ) )
			searcher.AddPatterns( relations.Conflicts, "Potential Conflicts" );
		if ( relationsFilter.HasFlag( PatternRelationType.Modulates ) )
			searcher.AddPatterns( relations.Modulates, "Modulates" );
		if ( relationsFilter.HasFlag( PatternRelationType.InstantiatedBy ) )
			searcher.AddPatterns( relations.InstantiatedBy, "Instantiated By" );
		if ( relationsFilter.HasFlag( PatternRelationType.ModulatedBy ) )
			searcher.AddPatterns( relations.ModulatedBy, "Modulated By" );
	}
}