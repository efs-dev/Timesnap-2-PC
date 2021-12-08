using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DearVRUIController : MonoBehaviour {

	[SerializeField] GameObject internalReverbObject;
	[SerializeField] GameObject reverbSendsObject;
	[SerializeField] GameObject performanceModeObject;

	[SerializeField] CanvasGroup canvasGroupPlayStop;
	[SerializeField] CanvasGroup canvasGroupPlay;
	[SerializeField] CanvasGroup canvaspresetChange;


	public void SetInternalReverbObject(bool shouldBeActive) {
		internalReverbObject.SetActive (shouldBeActive);
		canvasGroupPlayStop.interactable = false;
		canvasGroupPlay.interactable = false;
		canvaspresetChange.interactable = true;
	}
	public void SetReverbSendsObject(bool shouldBeActive) {
		reverbSendsObject.SetActive (shouldBeActive);
		canvasGroupPlayStop.interactable = false;
		canvasGroupPlay.interactable = false;
		canvaspresetChange.interactable = false;
	}	
	public void SetPerformanceModeObject(bool shouldBeActive) {
		performanceModeObject.SetActive (shouldBeActive);
		canvasGroupPlayStop.interactable = true;
		canvasGroupPlay.interactable = true;
		canvaspresetChange.interactable = false;
	}
}
