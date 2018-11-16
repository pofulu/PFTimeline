using System;
using UnityEngine;
using UnityEngine.Playables;

public enum FPTimelineEventType
{
	Ready,
	Started,
	Finished,
}

public class PFTimelineCore : MonoBehaviour
{
	public PlayableDirector playableDirector;
	public float currentTimeMs = 0;
	public float durationMs = 5000;
	public bool isPlyaing;

	public Action<FPTimelineEventType> Events;

	public bool IsPlaying()
	{
		return isPlyaing;
	}

	public float GetCurrentTimeMs()
	{
		return currentTimeMs;
	}

	public float GetDurationMs()
	{
		return durationMs;
	}

	public void Seek(float newTime)
	{
		isPlyaing = false;
		currentTimeMs = newTime;
	}

	public void Play()
	{
		isPlyaing = true;
	}

	public void Pause()
	{
		isPlyaing = false;
	}

	public void Rewind(bool autoPlay = false)
	{
		isPlyaing = autoPlay;
		currentTimeMs = 0;
	}

	private void Start()
	{
		Events?.Invoke(FPTimelineEventType.Ready);
		durationMs = (float)playableDirector.duration * 1000;
	}

	private void Update()
	{
		//if (isPlyaing)
		{
			currentTimeMs = (float)playableDirector.time * 1000;
			//currentTime = Mathf.Clamp(currentTime += Time.deltaTime * 1000, 0, lengthMs);

			if (currentTimeMs > 0)
			{
				Events?.Invoke(FPTimelineEventType.Started);
			}
			if (currentTimeMs == durationMs)
			{
				Events?.Invoke(FPTimelineEventType.Finished);
				isPlyaing = false;
			}
		}
	}
}