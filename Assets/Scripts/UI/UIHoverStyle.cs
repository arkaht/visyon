using TMPro;
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

			foreach ( Image background in backgrounds )
				background.color = isHovered ? style.HoverBackgroundColor : style.DefaultBackgroundColor;
			foreach ( TextMeshProUGUI text in tmpTexts )
				text.color = isHovered ? style.HoverTextColor : style.DefaultTextColor;
		}
	}
	private bool isHovered = false;

	[SerializeField]
	private HoverStyleData style;

	[SerializeField]
	private Image[] backgrounds;
	[SerializeField]
	private TextMeshProUGUI[] tmpTexts;

	public void OnPointerEnter( PointerEventData data ) => IsHovered = true;
	public void OnPointerExit( PointerEventData data ) => IsHovered = false;

	void Start()
	{
		IsHovered = false;
	}

	void OnValidate()
	{
		if ( backgrounds != null && backgrounds.Length == 0 )
			backgrounds = GetComponentsInChildren<Image>();
		if ( tmpTexts != null && tmpTexts.Length == 0 )
			tmpTexts = GetComponentsInChildren<TMPro.TextMeshProUGUI>();

		if ( style != null )
			IsHovered = false;
	}
}