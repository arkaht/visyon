using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINodeSearcherCategory : MonoBehaviour
{
	public TextMeshProUGUI Text => text;
	public string Name
	{
		get => Text.text;
		set => Text.text = value;
	}

	private bool isFolded = false;

	[SerializeField]
	private TextMeshProUGUI text;
	[SerializeField]
	private RectTransform visibilityTransform;
	[SerializeField]
	private RectTransform contentTransform;
	[SerializeField]
	private GameObject choicePrefab;

	private readonly List<UINodeSearcherChoice> choices = new();
	
	public UINodeSearcherChoice AddChoice( string name, string id )
	{
		GameObject obj = Instantiate( choicePrefab, contentTransform );

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

	public void SetFolded( bool value )
	{
		isFolded = value;

		visibilityTransform.localEulerAngles = new( 0.0f, 0.0f, isFolded ? 90.0f : 0.0f );
		contentTransform.gameObject.SetActive( !isFolded );
	} 

	public void ToggleFolding() => SetFolded( !isFolded );
}
