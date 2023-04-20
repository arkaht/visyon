using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visyon.Core
{
    public static class TextMarkup
    {
        public static string AsLink( string link, string name )
        {
			return $"<style=\"Link\"><link=\"{link}\">{name}</link></style>";
        }

        public static string AsCurrentLink( string name )
        {
            return $"<style=\"CurrentLink\">{name}</style>";
        }
    }
}
