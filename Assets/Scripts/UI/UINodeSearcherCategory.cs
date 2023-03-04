using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINodeSearcherCategory : MonoBehaviour
{
	public TextMeshProUGUI Text { get; private set; }
	public string Name
	{
		get => Text.text;
		set => Text.text = value;
	}
	
	[SerializeField]
	private GameObject choicePrefab;

	private readonly List<UINodeSearcherChoice> choices = new();
	
	public UINodeSearcherChoice AddChoice( string name, string id )
	{
		GameObject obj = Instantiate( choicePrefab, transform );

		UINodeSearcherChoice choice = obj.GetComponent<UINodeSearcherChoice>();
		choice.Name = name;
		choice.ID = id;

		choices.Add( choice );
		return choice;
	}

	public void FilterByName( string search_text )
	{
		choices.ForEach( 
			choice =>
			{
				string lower_name = choice.Name.ToLower();
				choice.gameObject.SetActive( lower_name.Contains( search_text ) );
			}
		);
	}

	public void Clear()
	{
		choices.ForEach( choice => Destroy( choice.gameObject ) );
		choices.Clear();
	}

	void Awake()
	{
		Text = GetComponent<TextMeshProUGUI>();
	}
}
