using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace Utils
{
	public class History<T>
	{
		public T Current => cursor == -1 ? default : content[cursor];
		public T Previous => cursor - 1 <= -1 ? default : content[cursor - 1];
		public T Next => cursor + 1 >= content.Count ? default : content[cursor + 1];

		private readonly List<T> content = new();
		private int cursor = -1;

		public T Navigate( int offset )
		{
			if ( content.Count == 0 ) return Current;

			//  get new cursor
			cursor = Math.Clamp( cursor + offset, 0, content.Count - 1 );

			/*Debug.Log( content.Count );
			for ( int i = 0; i < content.Count; i++ )
			{
				T t = content[i];
				if ( i == cursor )
					Debug.Log( "(it's me) " + t  );
				else
					Debug.Log( t );
			}*/

			return Current;
		}

		public void Add( T previous )
		{
			content.Insert( ++cursor, previous );
		}

		public void Clear()
		{
			cursor = -1;
			content.Clear();
		}
	}
}