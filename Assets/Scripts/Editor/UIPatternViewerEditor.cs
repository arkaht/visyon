
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor( typeof( UIPatternViewer ) )]
public class UIPatternViewerEditor : Editor
{
	private bool isFoldout = true;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		UIPatternViewer viewer = target as UIPatternViewer;

		EditorGUI.BeginDisabledGroup( true );
		isFoldout = EditorGUILayout.Foldout( isFoldout, "History" );

		var history = viewer.History;
		if ( history != null && isFoldout )
		{
			EditorGUILayout.TextField( "Cursor", history.Cursor.ToString() );
			EditorGUILayout.TextField( "Count", history.Content.Count.ToString() );

			if ( history.Content.Count > 0 )
			{
				EditorGUILayout.TextField( "Current", history.Current == null ? "null" : history.Current.ToString() );
				EditorGUILayout.TextField( "Previous", history.Previous == null ? "null" : history.Previous.ToString() );
				EditorGUILayout.TextField( "Next", history.Next == null ? "null" : history.Next.ToString() );

				EditorGUILayout.LabelField( "Content:" );
				EditorGUI.indentLevel++;
				foreach ( var data in history.Content )
				{
					EditorGUILayout.TextField( data.Data.ToString() );
				}
				EditorGUI.indentLevel--;
			}
		}

		EditorGUI.EndDisabledGroup();
	}
}