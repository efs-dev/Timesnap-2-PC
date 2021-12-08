/* James did this
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class AudioRecorder : MonoBehaviour {

    private VRButton _button;

    private bool _isRecording;

    public AudioClip RecordedAudio { get; private set; }


	// Use this for initialization
	IEnumerator Start ()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);

        if (Microphone.devices.Length < 1)
            gameObject.SetActive(false);

        for (var i = 0; i < Microphone.devices.Length; i++)
        {
            Debug.Log(Microphone.devices[i]);
        }

        var boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
            boxCollider = gameObject.AddComponent<BoxCollider>();

        boxCollider.size = transform.parent.GetComponent<RectTransform>().sizeDelta;

        _button = GetComponent<VRButton>();
        if (_button == null)
            _button.gameObject.AddComponent<VRButton>();


        _button.ClickDown.AddListener(() =>
        {
            if (!_isRecording)
            {
                Debug.Log("record");
                _isRecording = true;
                RecordedAudio = Microphone.Start(Microphone.devices[0], true, 20, 44100);
            }
            else
            {
                _isRecording = false;
                var pos = Microphone.GetPosition(Microphone.devices[0]);
                Microphone.End("Built-in Microphone");
                RecordedAudio = TrimAudio(RecordedAudio, pos);
            }
        });
	}

    AudioClip TrimAudio(AudioClip clip, int pos)
    {
        float[] samples = new float[pos * clip.channels];
        clip.GetData(samples, 0);
        var newClip = AudioClip.Create("record", pos, clip.channels, clip.frequency, false);
        newClip.SetData(samples, 0);
        return newClip;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
*/