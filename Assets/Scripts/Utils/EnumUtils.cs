using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
	public static class EnumUtils
	{
		public static bool HasFlag<T>( this T value, T mask ) where T : Enum
		{
			return ( (int)(object)value & (int)(object)mask ) != 0;
		}
	}
}