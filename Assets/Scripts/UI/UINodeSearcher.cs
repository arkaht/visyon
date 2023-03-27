using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UINodeSearcher : MonoBehaviour
{
	public bool IsSearchFieldFocused => searchField.isFocused;
	public TMP_InputField SearchField => searchField;

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
	private static GameObject selfPrefab;

	private readonly Dictionary<string, UINodeSearcherCategory> categories = new();
	private bool isRemoveCalled = false;

	public void FocusSearchField()
	{
		searchField.Select();
		searchField.ActivateInputField();
	}

	public void Clear()
	{
		foreach ( UINodeSearcherCategory category in categories.Values )
			Destroy( category.gameObject );

		categories.Clear();
	}

	public void AddPatterns( IEnumerable<string> ids, string category_name )
	{
		foreach ( string id in ids )
			AddPattern( id, category_name );
	}
	public void AddAllPatterns( string category_name = "All" ) => AddPatterns( PatternRegistery.AllKeys, category_name );

	public void AddPattern( string id, string category_name )
	{
		if ( !PatternRegistery.TryGet( id, out PatternData pattern ) )
		{
			Debug.LogError( "UINodeSearcher: failed to find pattern '" + id + "'" );
			return;
		}

		AddChoice( category_name, pattern.Name, id );
	}

	public UINodeSearcherCategory AddCategory( string name )
	{
		GameObject obj = Instantiate( categoryPrefab, content );

		UINodeSearcherCategory category = obj.GetComponent<UINodeSearcherCategory>();
		category.Name = name;

		categories.Add( name, category );
		return category;
	}

	public UINodeSearcherChoice AddChoice( string category_name, string name, string id )
	{
		if ( !categories.TryGetValue( category_name, out UINodeSearcherCategory category ) ) 
		{
			category = AddCategory( category_name );
		}

		UINodeSearcherChoice choice = category.AddChoice( name, id );
		choice.Button.onClick.AddListener( OnChoiceClicked );

		return choice;
	}

	public void UpdateSearchFilter()
	{
		string search_text = searchField.text.ToLower();
		foreach ( UINodeSearcherCategory category in categories.Values )
		{
			category.FilterByName( search_text );
		}
	}

	private void OnChoiceClicked()
	{
		GameObject selected = EventSystem.current.currentSelectedGameObject;
		if ( !selected.TryGetComponent( out UINodeSearcherChoice choice ) )
			return;

		UIPattern pattern = UIPattern.Spawn( choice.ID );
		pattern.transform.position = Blueprinter.Instance.ScreenToWorld( SpawnPosition );
		OnSpawnPattern.Invoke( pattern );

		Hide();
	}

	public static UINodeSearcher Spawn( Vector2 pos )
	{
		if ( selfPrefab == null )
		{
			selfPrefab = Resources.Load<GameObject>( "Prefabs/UI/NodeSearcher" );
		}

		GameObject obj = Instantiate( selfPrefab, Blueprinter.Instance.OverlayTransform );
		UINodeSearcher searcher = obj.GetComponent<UINodeSearcher>();
		searcher.transform.position = pos;
		searcher.SpawnPosition = pos;

		return searcher;
	}

	public void Hide() => gameObject.SetActive( false );
	public void Show() => gameObject.SetActive( true );

	void OnDisable()
	{
		if ( isRemoveCalled ) return;

		OnRemove.Invoke();
		isRemoveCalled = true;
	}

	void OnEnable()
	{
		OnSpawnPattern.RemoveAllListeners();
		OnRemove.RemoveAllListeners();
		isRemoveCalled = false;
	}
}
