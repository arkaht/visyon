using UnityEngine;

[CreateAssetMenu( menuName = "Data/MenuAction/Quit" )]
public class QuitMenuAction : MenuAction
{
	public override void Execute( Blueprinter blueprint ) 
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}
}