using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHoverStyle : MonoBehaviour,
							IPointerEnterHandler, IPointerExitHandler
{
	public bool IsHovered
	{
		get => isHovered;
		set
		{
			isHovered = value;

			background.color = isHovered ? style.HoverBackgroundColor : style.DefaultBackgroundColor;
			tmpText.color = isHovered ? style.HoverTextColor : style.DefaultTextColor;
		}
	}
	private bool isHovered = false;

	[SerializeField]
	private HoverStyleData style;

	[SerializeField]
	private Image background;
	[SerializeField]
	private TMPro.TextMeshProUGUI tmpText;

	public void OnPointerEnter( PointerEventData data ) => IsHovered = true;
	public void OnPointerExit( PointerEventData data ) => IsHovered = false;

	void Start()
	{
		IsHovered = false;
	}

	void OnValidate()
	{
		if ( background == null )
			background = GetComponentInChildren<Image>();
		if ( tmpText == null )
			tmpText = GetComponentInChildren<TMPro.TextMeshProUGUI>();

		if ( style != null )
			IsHovered = false;
	}
}