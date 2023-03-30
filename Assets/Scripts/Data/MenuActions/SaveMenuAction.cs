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

			//  show cursor
			CursorWrapper.IsHardwareVisible = true;
			
			//  ask path to user
			StandaloneFileBrowser.SaveFilePanelAsync( "Save Blueprint", directory, filename, Blueprinter.FILE_EXTENSION, 
				( path ) =>
				{
					//  hide cursor
					CursorWrapper.IsHardwareVisible = false;

					//  save
					if ( path.Length == 0 ) return;
					blueprint.Save( path );
				} 
			);
			
			return;
		}

		blueprint.Save( blueprint.FilePath );
	}
}