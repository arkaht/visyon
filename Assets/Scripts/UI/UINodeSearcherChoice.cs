using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINodeSearcherChoice : MonoBehaviour
{
	public string ID { get; set; }

	public TextMeshProUGUI Text { get; private set; }
	public Button Button { get; private set; }
	public string Name
	{
		get => Text.text;
		set => Text.text = value;
	}

	void Awake()
	{
		Text = GetComponent<TextMeshProUGUI>();
		Button = GetComponent<Button>();
	}
}
