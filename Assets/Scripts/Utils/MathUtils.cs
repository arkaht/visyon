using UnityEngine;

namespace Utils
{
	public static class MathUtils
	{
		public static float DirectionalAngle( Vector2 dir ) => Mathf.Atan2( dir.y, dir.x ) * Mathf.Rad2Deg;
		public static float DirectionalAngle( Vector2 origin, Vector2 target ) => DirectionalAngle( target - origin );
	}
}