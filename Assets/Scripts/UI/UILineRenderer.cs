using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Utils;

[RequireComponent( typeof( CanvasRenderer ) )]
public class UILineRenderer : Graphic, ICanvasRaycastFilter
{
	public float Thickness = 5.0f;
	public float RaycastThicknessFactor = 3.0f;

	public List<Vector2> Points = new();

	//  source: https://www.youtube.com/watch?v=--LB7URk60A&ab_channel=GameDevGuide
	protected override void OnPopulateMesh( VertexHelper vh )
	{
		vh.Clear();

		if ( Points.Count < 2 )
			return;

		float angle = 0.0f;
		for ( int i = 0; i < Points.Count - 1; i++ )
		{
			Vector2 start = transform.InverseTransformPoint( Points[i] );
			Vector2 end = transform.InverseTransformPoint( Points[i + 1] );

			if ( i < Points.Count - 1 )
				angle = MathUtils.DirectionalAngle( start, end ) + 90.0f;

			DrawVerticesForPoint( start, end, angle, vh );
		}

		for ( int i = 0; i < Points.Count - 1; i++ )
		{
			int index = i * 4;
			vh.AddTriangle( index + 0, index + 1, index + 2 );
			vh.AddTriangle( index + 1, index + 2, index + 3 );
		}
	}

	private void DrawVerticesForPoint( Vector2 start, Vector2 end, float angle, VertexHelper vh )
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

	public bool IsRaycastLocationValid( Vector2 pos, Camera camera )
	{
		float raycast_thickness = Thickness * RaycastThicknessFactor;
		for ( int i = 1; i < Points.Count; i++ )
		{
			Vector2 local_start = camera.WorldToScreenPoint( Points[i - 1] );
			Vector2 local_end = camera.WorldToScreenPoint( Points[i] );

			if ( MathUtils.IsPositionOnSegment( local_start, local_end, pos, raycast_thickness ) )
				return true;
		}

		return false;
	}

	public void ApplySizeToBounds()
	{
		//  zeroing pivot & anchors
		rectTransform.pivot = rectTransform.anchorMin = rectTransform.anchorMax = Vector2.zero;

		//  get bounds
		Vector2 min = new( float.MaxValue, float.MaxValue ), 
				max = new( float.MinValue, float.MinValue );
		foreach ( Vector2 point in Points )
		{
			if ( point.x < min.x )
				min.x = point.x;
			if ( point.y < min.y )
				min.y = point.y;
			
			if ( point.x > max.x )
				max.x = point.x;
			if ( point.y > max.y )
				max.y = point.y;
		}

		//  apply bounds
		Vector2 offset = new Vector2( Thickness, Thickness ) * RaycastThicknessFactor;
		rectTransform.position = min - offset;
		rectTransform.sizeDelta = ( max - min ) + offset * 2.0f;
	}

	public void UpdateRenderer()
	{
		SetVerticesDirty();
		ApplySizeToBounds();
	}

	#if UNITY_EDITOR
	void Update()
	{
		if ( UnityEditor.EditorApplication.isPlaying ) return;

		if ( transform.hasChanged )
		{
			OnValidate();
			transform.hasChanged = false;
		}	
	}

	protected override void OnValidate()
	{
		SetVerticesDirty();
	}
	#endif
}