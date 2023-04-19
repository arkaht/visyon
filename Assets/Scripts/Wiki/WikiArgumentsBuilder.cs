
namespace Visyon.Wiki
{
	public static class WikiArgumentsBuilder
	{
		public static RequestArguments GetPage( string page )
		{
			return
				new RequestArguments(
					new( "action", "parse" ),
					new( "page", page ),
					new( "prop", "wikitext" ),
					new( "format", "json" )
				);
		}

		public static RequestArguments GetPagesFromCategory( string category, int limit, string _continue = "" )
		{
			return
				new RequestArguments(
					new( "action", "query" ),
					new( "list", "categorymembers" ),
					new( "cmtitle", $"Category:{category}" ),
					new( "cmlimit", limit.ToString() ),
					new( "continue", _continue ),
					new( "format", "json" )
				);
		}
	}
}
