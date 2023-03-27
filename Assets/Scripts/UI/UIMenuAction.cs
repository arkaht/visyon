using System;
using System.Collections;
using UnityEngine;

public class UIMenuAction : MonoBehaviour
{
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
}