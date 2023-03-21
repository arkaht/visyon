using UnityEngine;

namespace Utils
{
	public static class MathUtils
	{
		public static float DirectionalAngle( Vector2 dir ) => Mathf.Atan2( dir.y, dir.x ) * Mathf.Rad2Deg;
		public static float DirectionalAngle( Vector2 origin, Vector2 target ) => DirectionalAngle( target - origin );

		//  https://www.alecjacobson.com/weblog/?p=1486
		public static bool PositionOnSegment( Vector2 start, Vector2 end, Vector2 pos, out Vector2 result )
		{
			Vector2 dir = end - start;
			float dir_sqr = Vector2.Dot( dir, dir );  //  dot is actually squaring the vector..
													  //  yes, I just remember my math classes
													  //  and realized some optimizations I can make now

			//  check start & end are not equal
			if ( dir_sqr == 0.0f )
			{
				result = start;
				return false;
			}

			Vector2 start_diff = pos - start;
			float t = Vector2.Dot( start_diff, dir ) / dir_sqr;

			//  check out of line
			if ( t < 0.0f )
			{
				result = start;
				return false;
			}
			else if ( t > 1.0f )
			{
				result = end;
				return false;
			}
			
			//  linear interpolation
			result = start + t * dir;
			return true;
		}
		public static bool IsPositionOnSegment( Vector2 start, Vector2 end, Vector2 pos, float max_dist )
		{
			if ( !PositionOnSegment( start, end, pos, out Vector2 result ) ) return false;

			Vector2 diff = result - pos;
			return Vector2.Dot( diff, diff ) <= max_dist * max_dist;
		}
	}
}