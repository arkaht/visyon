using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ShortcutKey
{
	public KeyCode Key;
	public bool ShouldHold;
}

public abstract class MenuAction : ScriptableObject
{
	public string Name = "N/A";
	public List<ShortcutKey> ShortcutKeys = new();

	private readonly Dictionary<string, string> keyReplacement = new() {
		{ "LeftControl", "Ctrl" },
		{ "RightControl", "Ctrl" },
		{ "LeftShift", "Shift" },
		{ "RightShift", "Shift" },
	};
	public string StringifyShortcut()
	{
		string text = "";

		int count = ShortcutKeys.Count;
		for ( int i = 0; i < count; i++ )
		{
			KeyCode key = ShortcutKeys[i].Key;

			//  concat key
			string str_key = key.ToString();
			if ( keyReplacement.TryGetValue( str_key, out string replaced ) )
				text += replaced;
			else
				text += str_key;
			
			//  concat separator
			if ( i + 1 < count )
				text += " + ";
		}

		return text;
	}

	public virtual void Execute( Blueprinter blueprint ) {}
}