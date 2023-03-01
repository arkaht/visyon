using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Blueprinter : MonoBehaviour
{
	public static Blueprinter Instance { get; private set; }
	
	public float Width => canvas.pixelRect.width;
	public float Height => canvas.pixelRect.height;
	public float UnscaledWidth => canvas.pixelRect.width / canvas.scaleFactor;
	public float UnscaledHeight => canvas.pixelRect.height / canvas.scaleFactor;

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
			Vector2 mouse_pos = GetMousePosition();
			if ( currentSearcher == null )
			{
				currentSearcher = UINodeSearcher.Spawn( mouse_pos );
				currentSearcher.AddAllPatterns();
			}
			else
			{
				currentSearcher.SpawnPosition = mouse_pos;
				currentSearcher.transform.position = mouse_pos;
			}

			//  offset Y position to prevent off-screens
			RectTransform rect_transform = (RectTransform) currentSearcher.transform;
			float y_diff = UnscaledHeight - ( rect_transform.anchoredPosition.y + rect_transform.sizeDelta.y );
			if ( y_diff <= 0.0f )
				rect_transform.anchoredPosition += new Vector2( 0.0f, -rect_transform.sizeDelta.y );
		}
	}
}