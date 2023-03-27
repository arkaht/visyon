using UnityEngine;

[CreateAssetMenu( menuName = "Data/HoverStyleData" )]
public class HoverStyleData : ScriptableObject
{
	public Color DefaultBackgroundColor = Color.white,
				 DefaultTextColor = Color.black;
	public Color HoverBackgroundColor = Color.black,
				 HoverTextColor = Color.white;
}