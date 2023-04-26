using System;
using System.Collections.Generic;

namespace Utils
{
	public class History<T>
	{
		public T Current => cursor == -1 ? default : content[cursor];
		public T Previous => cursor - 1 <= -1 ? default : content[cursor - 1];
		public T Next => cursor + 1 >= content.Count ? default : content[cursor + 1];
		public int Cursor => cursor;

		private readonly List<T> content = new();
		private int cursor = -1;

		public T Navigate( int offset )
		{
			if ( content.Count == 0 ) return Current;

			//  get new cursor
			OffsetCursor( offset );

			return Current;
		}

		public void OffsetCursor( int pos )
		{
			cursor = Math.Clamp( cursor + pos, 0, content.Count - 1 );
		}

		public void Add( T previous )
		{
			content.Insert( ++cursor, previous );
		}

		public void Remove( T target )
		{
			content.Remove( target );
			OffsetCursor( 0 );
		}
		public void RemoveAt( int id )
		{
			content.RemoveAt( id );
			OffsetCursor( 0 );
		}

		public void Clear()
		{
			cursor = -1;
			content.Clear();
		}
	}
}