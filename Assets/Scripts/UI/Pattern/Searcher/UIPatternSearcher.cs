using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIPatternSearcher : MonoBehaviour
{
	public bool IsCategorizing { get; private set; } = false;
	public ICollection<string> ActivePatterns { get; private set; }

	public UnityEvent<UIPattern> OnSpawnPattern;
	public UnityEvent OnRemove;

	public Vector2 SpawnPosition { get; set; }

	[Header( "References" )]
	[SerializeField]
	private RectTransform content;
	[SerializeField]
	private TMP_InputField searchField;

	[Header( "Prefabs" )]
	[SerializeField]
	private GameObject categoryPrefab;

	private readonly Dictionary<string, UIPatternSearcherCategory> categories = new();
	private readonly HashSet<string> currentCategories = new();
	private bool isRemoveCalled = false;
	private string globalCategory = ALL_PATTERNS_CATEGORY;

	private const string ALL_PATTERNS_CATEGORY = "Patterns";

	public void FocusSearchField()
	{
		searchField.Select();
		searchField.ActivateInputField();
	}

	public void ToggleCategories()
	{
		IsCategorizing = !IsCategorizing;

		Populate();
	}

	public void SetActivePatterns( ICollection<string> ids, string global_category = ALL_PATTERNS_CATEGORY )
	{
		ActivePatterns = ids;
		globalCategory = global_category;

		Clear();
		Populate();
	}

	public void Reload() 
	{
		Clear();
		Populate();
	}

	public void UpdateSearchFilter()
	{
		string search_text = searchField.text.ToLower();
		foreach ( string id in categories.Keys )
		{
			if ( !currentCategories.Contains( id ) ) continue;

			UIPatternSearcherCategory category = categories[id];
			int count = category.FilterByName( search_text );
			if ( count > 0 )
				category.Show();
			else
				category.Hide();
		}
	}

	public void OnChoiceClicked()
	{
		GameObject selected = EventSystem.current.currentSelectedGameObject;
		if ( !selected.TryGetComponent( out UIPatternSearcherChoice choice ) )
			return;

		UIPattern pattern = UIPattern.Spawn( choice.ID );
		pattern.transform.position = Blueprinter.Instance.ScreenToWorld( SpawnPosition );
		OnSpawnPattern.Invoke( pattern );

		this.Hide();
	}

	private UIPatternSearcherCategory CreateCategory( string name, bool is_folded = true )
	{
		if ( categories.TryGetValue( name, out UIPatternSearcherCategory category ) ) 
			return category;

		GameObject obj = Instantiate( categoryPrefab, content );
		category = obj.GetComponent<UIPatternSearcherCategory>();
		categories[name] = category;

		category.Name = name;
		category.Searcher = this;
		category.SetFolded( is_folded );

		return category;
	}

	private void CreatePattern( PatternData data, string category_name )
	{
		//  ensure category creation
		if ( !categories.TryGetValue( category_name, out UIPatternSearcherCategory category ) )
			category = CreateCategory( category_name );

		//  prevent duplicates
		if ( category.IsContained( data.ID ) )
			return;

		//  add pattern
		if ( category.IsFolded )
			category.AddCachedChoice( data.Name, data.ID );
		else
			category.AddChoice( data.Name, data.ID );
	}

	private void Populate()
	{
		CreateCategory( globalCategory, false );

		//  populate patterns & categories
		foreach ( string id in ActivePatterns )
		{
			if ( !PatternRegistery.TryGet( id, out PatternData data ) )
				continue;

			if ( IsCategorizing )
				foreach ( string category in data.Categories )
				{
					if ( category != globalCategory && category != ALL_PATTERNS_CATEGORY )
						CreatePattern( data, category );
				}
			else
				CreatePattern( data, globalCategory );
		}

		//  update categories visibility
		currentCategories.Clear();
		foreach ( string id in categories.Keys )
		{
			UIPatternSearcherCategory category = categories[id];
			if ( id == globalCategory )
			{
				if ( IsCategorizing )
					category.Hide();
				else
				{
					category.Show();
					currentCategories.Add( id );
				}
			}
			else
			{
				if ( IsCategorizing )
				{
					category.Show();
					currentCategories.Add( id );
				}
				else
					category.Hide();
			}
		}

		UpdateSearchFilter();
	}

	public void Clear()
	{
		IsCategorizing = false;
		searchField.text = "";

		TransformUtils.Clear( content );
		categories.Clear();
	}

	void Start()
	{
		this.Hide();
	}

	void OnEnable()
	{
		OnSpawnPattern.RemoveAllListeners();
		OnRemove.RemoveAllListeners();
		isRemoveCalled = false;

		//  bind to event
		PatternRegistery.OnReload.AddListener( Reload );
	}

	void OnDisable()
	{
		if ( !isRemoveCalled ) 
		{
			OnRemove.Invoke();
			isRemoveCalled = true;
		}

		//  unbind to event
		PatternRegistery.OnReload.RemoveListener( Reload );
	}
}