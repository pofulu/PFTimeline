using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PFTimelineGUIController : MonoBehaviour
{
	public PFTimelineCore source;

	public Slider barProgress;
	public Button btnPlay;
	public Button btnPause;
	public Button btnPlayOverlay;
	public Button btnPauseOverlay;
	public Button btnStop;
	public InputField timeText;

	public bool useKeyboardControl;
	private int pauseProcess = 1;

	public bool IsSeeking
	{
		get; set;
	}
	public bool IsPlaying
	{
		get
		{
			bool isPlaying = false;
			if (source != null)
			{
				isPlaying = source.IsPlaying();
			}
			return isPlaying;
		}
	}

	public float CurrentTimeMs
	{
		get
		{
			float currentTime = 0f;

			if (source != null)
			{
				currentTime = source.GetCurrentTimeMs();
			}
			return currentTime;
		}
	}
	public float DurationTimeMs
	{
		get
		{
			float durationTime = 0f;

			if (source != null)
			{
				durationTime = source.GetDurationMs();
				if (float.IsNaN(durationTime))
				{
					durationTime = 0f;
				}
			}
			return durationTime;
		}
	}

	public Action<float> OnSeeking;
	public Action OnPause;
	public Action OnStop;
	public Action OnPlay;

	#region Private Methods
	private void Start()
	{
		RegisterUIListener();
	}

	private void Update()
	{
		if (barProgress)
			barProgress.value = Mathf.InverseLerp(0, DurationTimeMs, CurrentTimeMs);

		if (timeText)
		{
			timeText.text = $"{(CurrentTimeMs / 1000).ToString("0.000")} / {(DurationTimeMs / 1000).ToString("0.000")}";
		}

		if (useKeyboardControl)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				if (IsPlaying)
					Pause();
				else
					PlayImmediatly();
			}
		}
	}

	private void RegisterUIListener()
	{
		if (barProgress)
		{
			barProgress.onValueChanged.AddListener(PlayScheduled);
			EventTrigger trigger = barProgress.GetComponent<EventTrigger>() ?? barProgress.gameObject.AddComponent<EventTrigger>();

			EventTrigger.Entry onPointDown = new EventTrigger.Entry();
			onPointDown.eventID = EventTriggerType.PointerDown;
			onPointDown.callback.AddListener((eventData) =>
			{
				IsSeeking = true;
				Pause();
			});

			EventTrigger.Entry onPointUp = new EventTrigger.Entry();
			onPointUp.eventID = EventTriggerType.PointerUp;
			onPointUp.callback.AddListener((eventData) =>
			{
				IsSeeking = false;
				Play();
			});

			trigger.triggers.Add(onPointUp);
			trigger.triggers.Add(onPointDown);
		}


		btnPause?.onClick.AddListener(Pause);

		btnPlay?.onClick.AddListener(Play);

		btnStop?.onClick.AddListener(Stop);

		btnPauseOverlay?.onClick.AddListener(Pause);

		btnPlayOverlay?.onClick.AddListener(Play);
	}

	private void PlayScheduled(float progress)
	{
		if (IsSeeking)
		{
			float newTime = Mathf.Lerp(0, DurationTimeMs, progress);

			if (newTime != CurrentTimeMs)
			{
				source.Seek(newTime);
				OnSeeking?.Invoke(progress);
			}
		}
	}

	private void PlayImmediatly()
	{
		source.Play();
		OnPlay?.Invoke();

		btnPlay?.gameObject.SetActive(false);
		btnPause?.gameObject.SetActive(true);
		btnPlayOverlay?.gameObject.SetActive(false);
		btnPauseOverlay?.gameObject.SetActive(true);

		pauseProcess = 0;
	}
	#endregion

	#region Public Methods
	public void Seek(float durationTimeMs, bool pause)
	{
		if (durationTimeMs != CurrentTimeMs)
		{
			source.Seek(durationTimeMs);
			OnSeeking?.Invoke(barProgress.value);

			if (pause)
				Pause();
		}
	}

	public void Play()
	{
		pauseProcess--;
		pauseProcess = (int)Mathf.Clamp(pauseProcess, 0, Mathf.Infinity);
		if (pauseProcess != 0)
			return;

		PlayImmediatly();
	}

	public void Pause()
	{
		pauseProcess++;

		source.Pause();
		OnPause?.Invoke();

		btnPlay?.gameObject.SetActive(true);
		btnPause?.gameObject.SetActive(false);
		btnPlayOverlay?.gameObject.SetActive(true);
		btnPauseOverlay?.gameObject.SetActive(false);
	}

	public void Stop()
	{
		source.Rewind();
		OnStop?.Invoke();

		btnPlay?.gameObject.SetActive(true);
		btnPause?.gameObject.SetActive(false);
		btnPlayOverlay?.gameObject.SetActive(true);
		btnPauseOverlay?.gameObject.SetActive(false);
	}

	public float GetProgress(float durationTimeMs)
	{
		return Mathf.InverseLerp(0, DurationTimeMs, durationTimeMs);
	}
	#endregion
}