using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UIPatternPin : MonoBehaviour,
							IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public UIPattern UIPattern => uiPattern;
	public PatternRelationType Relation => relationOut;

	[SerializeField]
	private UIPattern uiPattern;

	[SerializeField]
	private PointerEventData.InputButton inputButton;
	[SerializeField]
	private PatternRelationType relationOut, relationIn;

	private Dictionary<UIPatternPin, UIPatternConnection> connections = new();
	private UILineConnection preview_connection;

	public bool Connect( UIPatternPin pin )
	{
		if ( pin == null ) return false;  //  check null-reference
		if ( pin.uiPattern.PatternData == uiPattern.PatternData ) return false;  //  check same patterns
		if ( connections.ContainsKey( pin ) ) return false;  //  check existing connection
		
		UIPatternConnection connection = UIPatternConnection.Spawn( this, pin );
		connections.Add( pin, connection );
		
		print( "connect " + uiPattern.ID + " to " + pin.uiPattern.ID );
		return true;
	}

	public string[] GetRelations()
	{
		PatternRelations relations = uiPattern.PatternData.Relations;
		return relationOut switch
		{
			PatternRelationType.Instantiates => relations.Instantiates,
			PatternRelationType.InstantiatedBy => relations.InstantiatedBy,
			PatternRelationType.Modulates => relations.Modulates,
			PatternRelationType.ModulatedBy => relations.ModulatedBy,
			PatternRelationType.Conflicts => relations.Conflicts,
			_ => null,
		};
	}

	public string GetRelationName()
	{
		return relationOut switch
		{
			PatternRelationType.Instantiates => "Instantiates",
			PatternRelationType.InstantiatedBy => "Instantiated By",
			PatternRelationType.Modulates => "Modulates",
			PatternRelationType.ModulatedBy => "Modulated By",
			PatternRelationType.Conflicts => "Potential Conflicts",
			_ => "None",
		};
	}

	public void OnBeginDrag( PointerEventData data )
	{
		if ( data.button != inputButton ) return;

		preview_connection = UILineConnection.Spawn( relationOut );
	}

	public void OnDrag( PointerEventData data )
	{
		if ( data.button != inputButton )
		{
			Blueprinter.Instance.OnDrag( data );
			return;
		}

		preview_connection.Connect( transform.position, Blueprinter.Instance.ScreenToWorld( data.position ) );
	}

	public void OnEndDrag( PointerEventData data )
	{
		if ( data.button != inputButton )
		{
			Blueprinter.Instance.OnEndDrag( data );
			return;
		}

		//  spawn searcher
		UINodeSearcher searcher = Blueprinter.Instance.SpawnNodeSearcherAtMousePosition();

		//  add related patterns
		searcher.AddPatterns( GetRelations(), GetRelationName() );

		//  listen to events
		searcher.OnSpawnPattern.AddListener( pattern => Connect( pattern.GetRelationPin( relationIn ) ) );
		searcher.OnRemove.AddListener( () => Destroy( preview_connection.gameObject ) );
	}

	void Start()
	{
		if ( uiPattern.PatternData == null || uiPattern.PatternData.Relations == null )
		{
			Destroy( gameObject );
			return;
		}

		//  prevent using this pin if empty
		if ( GetRelations().Length == 0 )
			Destroy( gameObject );
	}
}