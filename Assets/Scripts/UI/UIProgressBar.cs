using TMPro;
using UnityEngine;

public class UIProgressBar : MonoBehaviour
{
	public int Current { get; private set; } = 0;
	public int Maximum { get; private set; } = 100;
	public float Ratio => (float) Current / Maximum;

	[SerializeField]
	private RectTransform activeRect;
	[SerializeField]
	private TMP_Text textTMP;

	private bool shouldUpdate = false;

	public void SetCurrent( int current )
	{
		Current = current;
		ScheduleUpdate();
	}

	public void SetMaximum( int max )
	{
		Maximum = max;
		ScheduleUpdate();
	}

	public void UpdateProgress()
	{
		textTMP.text = $"{Current} / {Maximum}";
		activeRect.localScale = new( Ratio, 1.0f, 1.0f );
	}
	public void ScheduleUpdate() => shouldUpdate = true;

	void Update()
	{
		if ( shouldUpdate )
		{
			UpdateProgress();
			shouldUpdate = false;
		}
	}
}