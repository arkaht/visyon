
namespace Visyon.Core
{
    public static class TextMarkup
    {
        public static string AsLink( string link, string name ) => $"<style=\"Link\"><link=\"{link}\">{name}</link></style>";
        public static string AsCurrentLink( string name ) => $"<style=\"CurrentLink\">{name}</style>";
		public static string AsHeader( int type, string header ) => $"<style=\"H{type}\">{header}</style>";
        public static string AsDefinition( string def ) => $"<style=\"Definition\">{def}</style>";
	}
}
