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

	void Start()
	{
		if ( uiPattern.PatternData == null || uiPattern.PatternData.Relations == null )
		{
			Destroy( gameObject );
			return;
		}

		//  get relations count
		int count = 0;
		PatternRelations relations = uiPattern.PatternData.Relations;
		if ( relationsFilter.HasFlag( PatternRelationType.Instantiates ) )
			count += relations.Instantiates.Length;
		if ( relationsFilter.HasFlag( PatternRelationType.Conflicts ) )
			count += relations.Conflicts.Length;
		if ( relationsFilter.HasFlag( PatternRelationType.Modulates ) )
			count += relations.Modulates.Length;
		if ( relationsFilter.HasFlag( PatternRelationType.InstantiatedBy ) )
			count += relations.InstantiatedBy.Length;
		if ( relationsFilter.HasFlag( PatternRelationType.ModulatedBy ) )
			count += relations.ModulatedBy.Length;

		//  prevent using this pin if empty
		if ( count == 0 )
			Destroy( gameObject );
	}
}