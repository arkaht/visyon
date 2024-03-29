﻿using SimpleJSON;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPatternPin : MonoBehaviour,
							IBeginDragHandler, IDragHandler, IEndDragHandler,
							IJSONSerializable
{
	public UIPattern UIPattern => uiPattern;
	public PatternRelationType Relation => relationOut;
	public Dictionary<UIPatternPin, UIPatternConnection> Connections { get; private set; } = new();

	[SerializeField]
	private UIPattern uiPattern;

	[SerializeField]
	private PointerEventData.InputButton inputButton;
	[SerializeField]
	private PatternRelationType relationOut, relationIn;
	[SerializeField]
	private Axis2D connectionAxisPreference = Axis2D.None;

	private UILineConnection preview_connection;

	private void DoConnect( UIPatternPin pin, UIPatternConnection connection )
	{
		Connections.Add( pin, connection );
		//print( $"add connection from {uiPattern.PatternData.Name} to {pin.uiPattern.PatternData.Name}" );
	}
	public bool Connect( UIPatternPin pin )
	{
		if ( pin == null ) return false;  //  check null-reference
		if ( pin.uiPattern.PatternData == uiPattern.PatternData ) return false;  //  check same patterns
		if ( Connections.ContainsKey( pin ) ) return false;  //  check existing connection
		
		//  spawn connection
		UIPatternConnection connection = UIPatternConnection.Spawn( this, pin, connectionAxisPreference );

		//  connect both
		DoConnect( pin, connection );
		pin.DoConnect( this, connection );
		
		return true;
	}
	public bool Connect( UIPattern pattern ) => Connect( pattern.GetRelationPin( relationIn ) );

	private void DoDisconnect( UIPatternPin pin )
	{
		Connections.Remove( pin );
		//print( $"remove connection from {uiPattern.PatternData.Name} to {pin.uiPattern.PatternData.Name}" );
	}
	public bool Disconnect( UIPatternPin pin )
	{
		if ( pin == null ) return false;
		if ( !Connections.TryGetValue( pin, out UIPatternConnection connection ) ) return false;
	
		//  destroy connection
		if ( connection != null )
			Destroy( connection.gameObject );

		//  disconnect both
		DoDisconnect( pin );
		pin.DoDisconnect( this );

		return true;
	}
	public void ClearConnections()
	{
		foreach ( var pair in Connections.ToList() )
			Disconnect( pair.Key );
	}

	public string[] GetPossibleRelations()
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

	public JSONNode Serialize()
	{
		JSONArray array = new();

		foreach ( UIPatternPin pin in Connections.Keys )
			array.Add( pin.UIPattern.UID.ID );

		return array;
	}

	public void OnBeginDrag( PointerEventData data )
	{
		if ( data.button != inputButton ) return;

		Blueprinter.Instance.Searcher.Hide();

		preview_connection = UILineConnection.Spawn( relationOut );
	}

	public void OnDrag( PointerEventData data )
	{
		if ( data.button != inputButton )
		{
			Blueprinter.Instance.OnDrag( data );
			return;
		}

		preview_connection.Connect( transform.position, Blueprinter.Instance.ScreenToWorld( data.position ), connectionAxisPreference );
	}

	public void OnEndDrag( PointerEventData data )
	{
		if ( data.button != inputButton )
		{
			Blueprinter.Instance.OnEndDrag( data );
			return;
		}

		//  check hovered pins
		foreach ( GameObject hovered in data.hovered )
			if ( hovered != null && hovered.TryGetComponent( out UIPatternPin pin ) )
			{
				//  destroy preview
				Destroy( preview_connection.gameObject );

				if ( pin.relationIn == relationOut && GetPossibleRelations().Contains( pin.uiPattern.ID ) )
				{
					//  connect to it
					Connect( pin );
				}
				return;
			}

		//  spawn searcher
		UIPatternSearcher searcher = Blueprinter.Instance.ShowSearcherAtMousePosition();

		//  add related patterns
		string category_name = GetRelationName();
		searcher.SetActivePatterns( GetPossibleRelations(), category_name );

		//  listen to events
		searcher.OnSpawnPattern.AddListener( pattern => Connect( pattern ) );
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
		if ( GetPossibleRelations().Length == 0 )
			Destroy( gameObject );
	}

	void OnDestroy()
	{
		ClearConnections();
	}
}