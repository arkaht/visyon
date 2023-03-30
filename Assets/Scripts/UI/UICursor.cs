using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof( Image ) )]
public class UICursor : MonoBehaviour
{
	public static UICursor Instance { get; private set; }

	public CursorData Data 
	{ 
		get => data; 
		set 
		{
			data = value == null ? defaultData : value;

			image.sprite = data.Sprite;
			image.rectTransform.pivot = new Vector2( data.Hotspot.x, image.sprite.textureRect.size.y - data.Hotspot.y ) / image.sprite.textureRect.size;
			image.material = data.Material;
		}
	}
	private CursorData data;
	[SerializeField]
	private CursorData defaultData;

	private Image image;

	void Awake()
	{
		image = GetComponent<Image>();
	}

	void Start()
	{
		Instance = this;
		CursorWrapper.IsHardwareVisible = false;
	}

	void OnValidate()
	{
		Awake();
		Data = defaultData;
	}

	void Update()
	{
		transform.position = Input.mousePosition;
	}
}