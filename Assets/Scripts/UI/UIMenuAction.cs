using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMenuAction : MonoBehaviour,
							IPointerClickHandler
{
	public UIMenu Menu { get; set; }

	public string Name 
	{
		get => tmpText.text;
		set => tmpText.text = value;
	}
	public string Shortcut
	{
		get => tmpShortcut.text;
		set => tmpShortcut.text = value;
	}
	public Action<Blueprinter> Callback { get; set; }

	[SerializeField]
	private TMPro.TextMeshProUGUI tmpText, tmpShortcut;

	public void OnPointerClick( PointerEventData data )
	{
		Callback.Invoke( Blueprinter.Instance );
		Menu.Hide();
	}
}