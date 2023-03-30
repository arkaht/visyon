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

		//  show cursor
		CursorWrapper.IsHardwareVisible = true;

		//  ask path to user
		StandaloneFileBrowser.OpenFilePanelAsync( "Open Blueprint", directory, Blueprinter.FILE_EXTENSION, false, 
			( paths ) =>
			{
				//  hide cursor
				CursorWrapper.IsHardwareVisible = false;
			
				//  load
				if ( paths.Length == 0 ) return;
				blueprint.Load( paths[0] );
			} 
		);
		
	}
}