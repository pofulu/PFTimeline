using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class PlayableDirectorEmbedder : MonoBehaviour
{
	public PlayableDirector director;
	public PFTimelineGUIController gui;

	private void Start()
	{
		gui.OnStop = () => StartCoroutine(StopCo());
		gui.OnPause = director.Pause;
		gui.OnPlay = director.Play;
		gui.OnSeeking = Seeking;
		gui.Play();
	}

	IEnumerator StopCo()
	{
		director.time = 0;
		director.Play();
		yield return null;
		director.Stop();
	}

	private void Seeking(float progress)
	{
		var newTime = Mathf.Lerp(0, (float)director.duration, progress);
		director.time = newTime;
		director.Evaluate();
	}
}
