using System;
using System.Collections;
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
	private CanvasGroup canvasGroup;
	[SerializeField]
	private LayoutGroup layoutGroup;
	[SerializeField]
	private Color highlightColor, disabledColor;

	private RectTransform rectTransform;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}
	void Start()
	{
		this.Hide();
	}
	void OnEnable()
	{
		Instance = this;
	}

	private IEnumerator DoInvalidate()
	{
		yield return GameObjectUtils.DoInvalidate( layoutGroup, canvasGroup );

		//  update position
		Move( rectTransform.position );
	}

	public void Show( Vector2 pos, string name, string text = "" )
	{
		//  change text
		if ( text == string.Empty )
			textTMP.text = TextMarkup.AsHeader( 4, name );
		else
			textTMP.text = $"{TextMarkup.AsHeader( 4, name )}\n{TextMarkup.AsItalic( text )}";

		rectTransform.position = pos;

		gameObject.SetActive( true );
		StartCoroutine( DoInvalidate() );  //  rebuild layout group
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