using UnityEngine;

namespace Utils
{
	public class Vector3Average
	{
		public int Count { get; private set; } = 0;
		public Vector3 Sum { get; private set; } = default;
		public Vector3 Result => Sum / Count;

		public void Add( Vector3 pos )
		{
			Sum += pos;
			Count++;
		}
	}
}
