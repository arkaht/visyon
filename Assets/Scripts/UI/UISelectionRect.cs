using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UISelectionRect : MonoBehaviour
{
	public RectTransform RectTransform { get; private set; }

	public Vector2 StartPos
	{
		get => startPos;
		set
		{
			startPos = value;
			RectTransform.position = startPos;
			area.position = startPos;
		}
	}
	public Vector2 EndPos
	{
		get => endPos;
		set
		{
			endPos = value;

			Vector2 pos = startPos;
			Vector2 size = endPos - startPos;

			//  handle negative sizes
			if ( size.x < 0.0f )
			{
				size.x = -size.x;
				pos.x -= size.x;
			}
			if ( size.y < 0.0f )
			{
				size.y = -size.y;
				pos.y -= size.y;
			}

			RectTransform.position = area.position = pos;
			RectTransform.sizeDelta = area.size = size;
		}
	}
	public Rect Area => area;

	private Vector2 startPos, endPos;
	private Rect area;

	public bool IsInArea( Vector2 point ) => Area.Contains( point );
	public bool IsInArea( Rect rect ) => Area.Overlaps( rect );
	public bool IsInArea( RectTransform transform )
	{
		if ( transform == null ) return false;

		Rect rect = new( transform.position, transform.sizeDelta );
		rect.position -= rect.size * transform.pivot;
		return IsInArea( rect );
	}

	public List<T> Find<T>() where T : Component
	{
		List<T> objects = new();

		foreach ( T obj in FindObjectsOfType<T>() )
		{
			if ( IsInArea( obj.transform as RectTransform ) )
				objects.Add( obj );
		}

		return objects;
	}

	public void Reset()
	{
		startPos = endPos = Vector2.zero;
		RectTransform.sizeDelta = Vector2.zero;
	}

	void Start()
	{
		RectTransform = GetComponent<RectTransform>();
	}
}
