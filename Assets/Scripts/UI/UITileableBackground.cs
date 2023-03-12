using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITileableBackground : MonoBehaviour
{
	[SerializeField]
	private UIGrid grid;
	[SerializeField]
	private Transform target;

	private RectTransform rectTransform;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	void Update()
	{
		Blueprinter blueprinter = Blueprinter.Instance;

		//  get tiling
		Vector2 camera_size = blueprinter.CameraSize * blueprinter.PixelRatio;
		Vector3 tile_pos = new(
			Mathf.Round( target.position.x / camera_size.x ),
			Mathf.Round( target.position.y / camera_size.y )
		);
		Vector2 pos = tile_pos * camera_size;

		//  apply tiling
		rectTransform.offsetMax = pos + camera_size;
		rectTransform.offsetMin = pos - camera_size;
	}
}
