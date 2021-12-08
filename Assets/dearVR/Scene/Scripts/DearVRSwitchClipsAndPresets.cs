using UnityEngine;
using System.Collections;
using DearVR;

public class DearVRSwitchClipsAndPresets : MonoBehaviour {


	[SerializeField] AudioClip [] PlayClip;

	[SerializeField] int ClipIndex = 0;

	AudioSource Source;

	void Start () {
		
		Debug.Log("DEARVR Demo Scene: press F or G to switch AudioClips");

		Source = GetComponent<AudioSource>();

		Source.clip = PlayClip[0];

		Source.Play();

	}
		
	void Update () {
	
		if(Input.GetKeyDown(KeyCode.G)) {
			
			ClipIndex = (ClipIndex + 1)%PlayClip.Length;

			Source.clip = PlayClip[ClipIndex];

			Source.Play();

		}

		if(Input.GetKeyDown(KeyCode.F)) {
			
			ClipIndex--;

			if (ClipIndex < 0) {
			
				ClipIndex = PlayClip.Length - 1;

			}

			Source.clip = PlayClip[ClipIndex];

			Source.Play();

		}

	}
		
}
