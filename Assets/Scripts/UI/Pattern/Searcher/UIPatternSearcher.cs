using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIPatternSearcher : MonoBehaviour
{
	public bool IsCategorizing { get; private set; } = false;
	public List<string> ActivePatterns { get; private set; } = new();

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

	public void SetActivePatterns( List<string> ids, string global_category = ALL_PATTERNS_CATEGORY )
	{
		ActivePatterns = ids;
		globalCategory = global_category;

		Clear();
		Populate();
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
		foreach ( string id in categories.Keys )
		{
			UIPatternSearcherCategory category = categories[id];
			if ( id == globalCategory )
			{
				if ( IsCategorizing )
					category.Hide();
				else
					category.Show();
			}
			else
			{
				if ( IsCategorizing )
					category.Show();
				else
					category.Hide();
			}
		}
	}

	public void Clear()
	{
		IsCategorizing = false;

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
		//PatternRegistery.OnReload.AddListener( Reload );
		//Reload();
	}

	void OnDisable()
	{
		if ( !isRemoveCalled ) 
		{
			OnRemove.Invoke();
			isRemoveCalled = true;
		}

		//  unbind to event
		//PatternRegistery.OnReload.RemoveListener( Reload );
	}
}