using SFB;
using System.IO;
using UnityEngine;

[CreateAssetMenu( menuName = "Data/MenuAction/Export Collection" )]
public class ExportCollectionMenuAction : MenuAction
{
	public override void Execute( Blueprinter blueprint ) 
	{
		bool is_path_empty = blueprint.FilePath.Length == 0;

		//  get path names
		string filename = is_path_empty ? "" : Path.GetFileNameWithoutExtension( blueprint.FilePath );
		string directory = is_path_empty ? "" : Path.GetDirectoryName( blueprint.FilePath );

		//  show cursor
		CursorWrapper.IsHardwareVisible = true;
			
		//  ask path to user
		StandaloneFileBrowser.SaveFilePanelAsync( Name, directory, filename, "csv", 
			( path ) =>
			{
				//  hide cursor
				CursorWrapper.IsHardwareVisible = false;
				
				//  save
				if ( path.Length == 0 ) return;
				PatternRegistery.ExportCSV( path, PatternRegistery.AllKeys );
			} 
		);
	}
}