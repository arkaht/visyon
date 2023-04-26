using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Visyon.Core;

public class UITaskViewer : MonoBehaviour
{
	public static UITaskViewer Instance => instance;
	private static UITaskViewer instance;

	public UIProgressBar Progress => progress;
	public string State
	{
		get => stateTMP.text;
		set => stateTMP.text = value;
	}
	public bool IsRunning { get; private set; }

	[SerializeField]
	private TMP_Text titleTMP, stateTMP, timerTMP;
	[SerializeField]
	private UIProgressBar progress;

	private readonly Stopwatch stopwatch = new();

	public void Begin( string title, int count = 1 )
	{
		gameObject.SetActive( true );

		titleTMP.text = title;
		stateTMP.text = "Waiting";

		progress.SetCurrent( 0 );
		progress.SetMaximum( count );

		stopwatch.Reset();
		stopwatch.Start();

		IsRunning = true;
	}

	public void End()
	{
		stopwatch.Stop();
		print( "UITaskViewer: finished tasks in " + stopwatch.Elapsed );

		IsRunning = false;
	}

	public Tasker Use( string title, int count = 1 )
	{
		if ( IsRunning ) return null;
		return new( this, title, count );
	}

	void Update()
	{
		if ( stopwatch.IsRunning )
		{
			TimeSpan span = stopwatch.Elapsed;
			timerTMP.text = $"{span.Hours:D2}:{span.Minutes:D2}:{span.Seconds:D2}";
		}
	}

	void OnEnable()
	{
		instance = this;
	}

	void Start()
	{
		gameObject.SetActive( false );
	}
}
