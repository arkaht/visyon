using UnityEditor;
using UnityEditor.UI;

[CustomEditor( typeof( MinMaxContentSizeFitter ) )]
public class MinMaxContentSizeFitterEditor : ContentSizeFitterEditor
{
	SerializedProperty horizontalRange, verticalRange;

	protected override void OnEnable()
	{
        horizontalRange = serializedObject.FindProperty( "horizontalRange" );
        verticalRange = serializedObject.FindProperty( "verticalRange" );
		base.OnEnable();
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField( horizontalRange, true );
        EditorGUILayout.PropertyField( verticalRange, true );
        serializedObject.ApplyModifiedProperties();

		base.OnInspectorGUI();
	}
}