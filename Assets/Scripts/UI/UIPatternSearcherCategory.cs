using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPatternSearcherCategory : MonoBehaviour
{
	public UIPatternSearcher Searcher;

	public TextMeshProUGUI Text => text;
	public string Name
	{
		get => Text.text;
		set => Text.text = value;
	}

	public bool IsFolded { get; private set; } = false;
	public int Count => choices.Count + cachedChoices.Count;

	[SerializeField]
	private TextMeshProUGUI text;
	[SerializeField]
	private RectTransform visibilityTransform;
	[SerializeField]
	private RectTransform contentTransform;
	[SerializeField]
	private VerticalLayoutGroup layoutGroup;
	[SerializeField]
	private GameObject choicePrefab;

	private readonly HashSet<string> ids = new();
	private readonly List<UIPatternSearcherChoice> choices = new();

	struct CachedChoice
	{
		public string Name;
		public string ID;
		public bool IsFilteredOut;
	}
	private readonly List<CachedChoice> cachedChoices = new();

	public bool IsContained( string id ) => ids.Contains( id );
	
	public UIPatternSearcherChoice AddChoice( string name, string id, bool is_enabled = true )
	{
		GameObject obj = Instantiate( choicePrefab, contentTransform );

		UIPatternSearcherChoice choice = obj.GetComponent<UIPatternSearcherChoice>();
		choice.Name = name;
		choice.ID = id;

		//  connect click event to searcher
		choice.Button.onClick.AddListener( Searcher.OnChoiceClicked );

		choice.gameObject.SetActive( is_enabled );

		ids.Add( id );
		choices.Add( choice );
		return choice;
	}

	public void AddCachedChoice( string name, string id )
	{
		ids.Add( id );
		cachedChoices.Add( 
			new()
			{
				Name = name,
				ID = id,
				IsFilteredOut = false,
			}
		);
	}

	public void FilterByName( string search_text )
	{
		//  filter active choices
		choices.ForEach( 
			choice =>
			{
				string lower_name = choice.Name.ToLower();
				choice.gameObject.SetActive( lower_name.Contains( search_text ) );
			}
		);
		
		//  filter cached choices
		for ( int i = 0; i < cachedChoices.Count; i++ )
		{
			CachedChoice choice = cachedChoices[i];
			choice.IsFilteredOut = !choice.Name.ToLower().Contains( search_text );
			cachedChoices[i] = choice;
		}

		InvalidateLayout();
	}

	public void Clear()
	{
		ids.Clear();
		choices.ForEach( choice => Destroy( choice.gameObject ) );
		choices.Clear();
		cachedChoices.Clear();
	}

	public void SetFolded( bool value )
	{
		IsFolded = value;

		visibilityTransform.localEulerAngles = new( 0.0f, 0.0f, IsFolded ? 90.0f : 0.0f );
		contentTransform.gameObject.SetActive( !IsFolded );

		//  create cached choices
		if ( !IsFolded && cachedChoices.Count > 0 )
		{
			foreach ( CachedChoice choice in cachedChoices )
				AddChoice( choice.Name, choice.ID, !choice.IsFilteredOut );
			cachedChoices.Clear();

			InvalidateLayout();
		}
	}
	public void ToggleFolding() => SetFolded( !IsFolded );

	private void InvalidateLayout() => LayoutRebuilder.ForceRebuildLayoutImmediate( contentTransform );
}
