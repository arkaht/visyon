using SFB;
using System.IO;
using UnityEngine;

[CreateAssetMenu( menuName = "Data/MenuAction/Load" )]
public class LoadMenuAction : MenuAction
{
	public override void Execute( Blueprinter blueprint ) 
	{
		//  get directory name
		bool is_path_empty = blueprint.FilePath.Length == 0;
		string directory = is_path_empty ? "" : Path.GetDirectoryName( blueprint.FilePath );

		//  ask path to user
		CursorWrapper.IsHardwareVisible = true;
		StandaloneFileBrowser.OpenFilePanelAsync( "Open Blueprint", directory, Blueprinter.FILE_EXTENSION, false, 
			( paths ) =>
			{
				CursorWrapper.IsHardwareVisible = false;
				if ( paths.Length == 0 ) return;

				//  loadnuAction
				blueprint.Load( paths[0] );
			} 
		);
		
	}
}