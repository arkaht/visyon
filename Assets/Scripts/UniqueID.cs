using System.Collections.Generic;
using UnityEngine;

public class UniqueID : MonoBehaviour
{
	public static int NextID()
	{
		while ( registery.ContainsKey( ++nextID ) );
		return nextID;
	}
	public static void ResetNextID() => nextID = -1;
	private static int nextID = 0;
	public static bool TryGet( int uid, out UniqueID component ) => registery.TryGetValue( uid, out component );
	private readonly static Dictionary<int, UniqueID> registery = new();

	public int ID { get; set; } = -1;

	void Start()
	{
		//  assign unique id
		if ( ID == -1 )
			ID = NextID();

		//  register it
		registery[ID] = this;
	}
}