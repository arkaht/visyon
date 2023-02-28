using System.Collections;
using UnityEngine;

public class PatternManager : MonoBehaviour
{
	void Start()
	{
		PatternRegistery.LoadAll();
	}
}