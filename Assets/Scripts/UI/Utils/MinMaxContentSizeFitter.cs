using UnityEngine;
using UnityEngine.UI;

public class MinMaxContentSizeFitter : ContentSizeFitter
{
	[SerializeField]
	private Vector2 horizontalRange = new( 0.0f, 100.0f );
	[SerializeField]
	private Vector2 verticalRange = new( 0.0f, 100.0f );

	public RectTransform RectTransform
	{
		get 
		{
			if ( rectTransform == null )
				rectTransform = GetComponent<RectTransform>();
			return rectTransform;
		}
	}
	private RectTransform rectTransform;

	public override void SetLayoutHorizontal()
	{
		base.SetLayoutHorizontal();

		if ( horizontalRange.x > -1 )
		{
			Vector2 size = RectTransform.sizeDelta;
			size.x = Mathf.Clamp( size.x, horizontalRange.x, horizontalRange.y );
			RectTransform.sizeDelta = size;
		}
	}

	public override void SetLayoutVertical()
	{
		base.SetLayoutVertical();

		if ( verticalRange.x > -1 )
		{
			Vector2 size = RectTransform.sizeDelta;
			size.y = Mathf.Clamp( size.y, verticalRange.x, verticalRange.y );
			RectTransform.sizeDelta = size;
		}
	}
}