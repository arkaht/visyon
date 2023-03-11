using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//  source: https://www.youtube.com/watch?v=--LB7URk60A&ab_channel=GameDevGuide
public class UILineRenderer : Graphic
{
	public float Thickness = 5.0f;

	public List<Vector2> Points = new();

	protected override void OnPopulateMesh( VertexHelper vh )
	{
		vh.Clear();

		if ( Points.Count < 2 )
			return;

		float angle = 0.0f;
		for ( int i = 0; i < Points.Count - 1; i++ )
		{
			Vector2 start = Points[i];
			Vector2 end = Points[i + 1];

			if ( i < Points.Count - 1 )
			{
				angle = GetAngle( Points[i], Points[i + 1] ) + 90.0f;
			}

			DrawVerticesForPoint( start, end, angle, vh );
		}

		for ( int i = 0; i < Points.Count - 1; i++ )
		{
			int index = i * 4;
			vh.AddTriangle( index + 0, index + 1, index + 2 );
			vh.AddTriangle( index + 1, index + 2, index + 3 );
		}
	}
	public float GetAngle( Vector2 me, Vector2 target )
	{
		return Mathf.Atan2( target.y - me.y, target.x - me.x ) * Mathf.Rad2Deg;
	}
	void DrawVerticesForPoint( Vector2 start, Vector2 end, float angle, VertexHelper vh )
	{
		UIVertex vertex = UIVertex.simpleVert;
		vertex.color = color;

		Quaternion rotation = Quaternion.Euler( 0.0f, 0.0f, angle );
		Vector3 thickness = new( Thickness * 0.5f, 0.0f );
		vertex.position = (Vector3)start + rotation * -thickness;
		vh.AddVert( vertex );

		vertex.position = (Vector3)start + rotation * thickness;
		vh.AddVert( vertex );

		vertex.position = (Vector3)end + rotation * -thickness;
		vh.AddVert( vertex );

		vertex.position = (Vector3)end + rotation * thickness;
		vh.AddVert( vertex );
	}
}