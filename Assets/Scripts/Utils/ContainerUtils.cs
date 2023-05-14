using System;
using System.Collections.Generic;

namespace Utils
{
	public static class ContainerUtils
	{
		public static bool IsInRange( int index, int count ) => index >= 0 && index <= count - 1;
	}
}
