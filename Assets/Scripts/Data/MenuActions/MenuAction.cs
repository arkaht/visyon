﻿using System.Collections.Generic;
using UnityEngine;

public abstract class MenuAction : ScriptableObject
{
	public string Name = "N/A";
	public List<KeyCode> ShortcutKeys = new();

	private Dictionary<string, string> keyReplacement = new() {
		{ "LeftControl", "Ctrl" },	
		{ "RightControl", "Ctrl" },	
	};
	public string StringifyShortcut()
	{
		string text = "";

		int count = ShortcutKeys.Count;
		for ( int i = 0; i < count; i++ )
		{
			KeyCode key = ShortcutKeys[i];

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