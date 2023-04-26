using System;
using System.Threading.Tasks;

namespace Visyon.Core
{
	public class Tasker : IDisposable
	{
		public UITaskViewer Viewer { get; private set; }
		public string State
		{
			get => Viewer.State;
			set => Viewer.State = value;
		}

		public Tasker( UITaskViewer viewer, string title, int count = 1 )
		{
			Viewer = viewer;
			Viewer.Begin( title, count );
		}

		public void Dispose()
		{
			Viewer.End();
		}

		public Task Task( string state, Task task ) 
		{
			State = state;
			return task;
		}

		public void AddProgress() => Viewer.Progress.SetCurrent( Viewer.Progress.Current + 1 );
		public void SetMaximumProgress( int max ) => Viewer.Progress.SetMaximum( max );
	}
}
