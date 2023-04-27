using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Visyon.Core;

[RequireComponent( typeof( RectTransform ) )]
public class UITooltip : MonoBehaviour
{
	public static UITooltip Instance { get; private set; }

	[SerializeField]
	private Image highlightImage;
	[SerializeField]
	private TMP_Text textTMP;
	[SerializeField]
	private Canvas canvas;
	[SerializeField]
	private Color highlightColor, disabledColor;

	private RectTransform rectTransform;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}
	void Start()
	{
		Hide();
	}
	void OnEnable()
	{
		Instance = this;
	}

	public void Show( Vector2 pos, string name, string text = "" )
	{
		gameObject.SetActive( true );

		Move( pos );

		//  change text
		if ( text == string.Empty )
			textTMP.text = TextMarkup.AsHeader( 4, name );
		else
			textTMP.text = $"{TextMarkup.AsHeader( 4, name )}\n{TextMarkup.AsItalic( text )}";
	}
	public void Hide()
	{
		gameObject.SetActive( false );
	}
	public void SetHighlight( bool is_highlight )
	{
		Color color = is_highlight ? highlightColor : disabledColor;
		highlightImage.color = color;
		textTMP.color = color;
	}

	public void Move( Vector2 pos )
	{
		Vector2 pivot = Vector2.zero;
		if ( pos.x + rectTransform.sizeDelta.x * canvas.scaleFactor >= canvas.pixelRect.size.x )
			pivot.x = 1.0f;
		rectTransform.pivot = pivot;

		rectTransform.position = pos;
	}
}