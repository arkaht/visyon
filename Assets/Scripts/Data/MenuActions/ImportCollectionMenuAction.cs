using SFB;
using System.IO;
using UnityEngine;

[CreateAssetMenu( menuName = "Data/MenuAction/Import Collection" )]
public class ImportCollectionMenuAction : MenuAction
{
	public override void Execute( Blueprinter blueprint ) 
	{
		bool is_path_empty = blueprint.FilePath.Length == 0;

		//  get path names
		string directory = is_path_empty ? "" : Path.GetDirectoryName( blueprint.FilePath );

		//  show cursor
		CursorWrapper.IsHardwareVisible = true;
			
		//  ask path to user
		StandaloneFileBrowser.OpenFilePanelAsync( Name, directory, "csv", false,
			( paths ) =>
			{
				//  hide cursor
				CursorWrapper.IsHardwareVisible = false;
				
				//  import
				string path = paths.Length == 0 ? "" : paths[0];
				if ( path.Length == 0 ) return;
				PatternRegistery.ImportCSV( path );
			} 
		);
	}
}