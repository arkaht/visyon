using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Blueprinter : MonoBehaviour
{
	public static Blueprinter Instance { get; private set; }

	[SerializeField]
	private Canvas canvas;

	private UINodeSearcher currentSearcher;

	public static Vector2 GetMousePosition()
	{
		Vector2 pos = Input.mousePosition;
		//pos /= Instance.canvas.scaleFactor;
		return pos;
	}

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		PatternRegistery.LoadAll();
	}

	void LateUpdate()
	{
		if ( Input.GetMouseButtonDown( 1 ) )
		{
			if ( currentSearcher == null )
			{
				currentSearcher = UINodeSearcher.Spawn();
				currentSearcher.AddAllPatterns();
			}
			currentSearcher.transform.position = GetMousePosition();
		}
	}
}