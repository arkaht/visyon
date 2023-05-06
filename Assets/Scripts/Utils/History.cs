using System;
using System.Collections.Generic;

namespace Utils
{
	public class History<T>
	{
		public T Current => ContainerUtils.IsInRange( Cursor, Content.Count ) ? Content[Cursor] : default;
		public T Previous => ContainerUtils.IsInRange( Cursor - 1, Content.Count ) ? Content[Cursor - 1] : default;
		public T Next => ContainerUtils.IsInRange( Cursor + 1, Content.Count ) ? Content[Cursor + 1] : default;

		public int Cursor { get; private set; }
		public List<T> Content { get; init; } = new();

		public T Navigate( int offset )
		{
			if ( Content.Count == 0 ) return Current;

			//  get new cursor
			OffsetCursor( offset );

			return Current;
		}

		public void OffsetCursor( int pos )
		{
			if ( Content.Count == 0 )
			{
				Cursor = -1;
				return;
			}

			Cursor = Math.Clamp( Cursor + pos, 0, Content.Count - 1 );
		}

		public void Add( T previous )
		{
			Content.Insert( ++Cursor, previous );
		}

		public void Remove( T target )
		{
			Content.Remove( target );
			OffsetCursor( 0 );
		}
		public void RemoveAt( int id )
		{
			Content.RemoveAt( id );
			OffsetCursor( 0 );
		}

		public void Clear()
		{
			Cursor = -1;
			Content.Clear();
		}
	}
}