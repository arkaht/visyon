using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visyon.Wiki
{
	public class RequestArguments
	{
		private readonly Dictionary<string, string> collection = new();

		public RequestArguments() {}
		public RequestArguments( params KeyValuePair<string, string>[] args )
		{
			foreach ( var pair in args )
				Set( pair.Key, pair.Value );
		}

		public void Set( string key, string value )
		{
			collection.Add( key, value );
		}

		public string Get( string key, string default_value = null )
		{
			if ( collection.TryGetValue( key, out string result ) )
				return result;

			return default_value;
		}

		public string Stringify()
		{
			if ( collection.Count == 0 ) return "";

			string str = "?";
			int id = 0;
			foreach ( var pair in collection )
			{
				str += $"{pair.Key}={pair.Value}" + ( id + 1 < collection.Count ? "&" : "" );
				id++;
			}

			return str;
		}
	}
}
