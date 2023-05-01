using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
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
	public Tasker Tasker { get; private set; }
	public bool IsRunning { get; private set; }

	public UnityEvent OnBegin, OnEnd;

	[SerializeField]
	private TMP_Text titleTMP, stateTMP, timerTMP;
	[SerializeField]
	private UIProgressBar progress;
	[SerializeField]
	private Button hideButton, stopButton;

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

		hideButton.interactable = false;
		IsRunning = true;

		OnBegin.Invoke();
	}

	public void End()
	{
		stopwatch.Stop();
		print( "UITaskViewer: finished tasks in " + stopwatch.Elapsed );

		if ( hideButton )
			hideButton.interactable = true;

		IsRunning = false;
		Tasker = null;

		OnEnd.Invoke();
	}

	public Tasker Use( string title, int count = 1 )
	{
		if ( IsRunning ) return null;

		Tasker = new( this, title, count );
		return Tasker;
	}

	public void CancelToken()
	{
		if ( Tasker?.CancelToken == null ) return;
		Tasker.CancelToken.Cancel();

		print( "UITaskViewer: cancel token" );
	}

	void Update()
	{
		if ( stopwatch.IsRunning )
		{
			TimeSpan span = stopwatch.Elapsed;
			timerTMP.text = $"{span.Hours:D2}:{span.Minutes:D2}:{span.Seconds:D2}";
		}

		stopButton.interactable = Tasker?.CancelToken != null && !Tasker.CancelToken.IsCancellationRequested;
	}

	void Start()
	{
		this.Hide();
	}

	void OnEnable()
	{
		instance = this;
	}

	void OnDisable()
	{
		CancelToken();
	}
}
