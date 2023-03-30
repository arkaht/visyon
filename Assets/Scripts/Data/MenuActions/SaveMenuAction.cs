using SFB;
using System.IO;
using UnityEngine;

[CreateAssetMenu( menuName = "Data/MenuAction/Save" )]
public class SaveMenuAction : MenuAction
{
	public bool AskNewPath = false;

	public override void Execute( Blueprinter blueprint ) 
	{
		bool is_path_empty = blueprint.FilePath.Length == 0;
		if ( AskNewPath || is_path_empty )
		{
			//  get path names
			string filename = is_path_empty ? "" : Path.GetFileNameWithoutExtension( blueprint.FilePath );
			string directory = is_path_empty ? "" : Path.GetDirectoryName( blueprint.FilePath );

			//  ask path to user
			CursorWrapper.IsHardwareVisible = true;
			StandaloneFileBrowser.SaveFilePanelAsync( "Save Blueprint", directory, filename, Blueprinter.FILE_EXTENSION, 
				( path ) =>
				{
					CursorWrapper.IsHardwareVisible = false;
					if ( path.Length == 0 ) return;

					//  save & replace file path
					blueprint.Save( path );
				} 
			);
			
			return;
		}

		blueprint.Save( blueprint.FilePath );
	}
}