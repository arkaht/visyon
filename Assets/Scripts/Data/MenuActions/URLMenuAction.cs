using UnityEngine;

[CreateAssetMenu( menuName = "Data/MenuAction/URL" )]
public class URLMenuAction : MenuAction
{
	public string URL;

	public override void Execute( Blueprinter blueprint ) 
	{
		Application.OpenURL( URL );
	}
}