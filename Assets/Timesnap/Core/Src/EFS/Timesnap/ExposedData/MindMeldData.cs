using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MindMeldData.asset", menuName ="TimeSnap/Mind Meld Data", order =1)]
public class MindMeldData : ScriptableObject
{
    public string Id;
    [TextArea]
    public string Text;
    public AudioClip Audio;

    public List<MindMeldChoiceData> Choices;
}

[System.Serializable]
public class MindMeldChoiceData
{
    [TextArea]
    public string Text;
    public string NextMindMeldId;

    public AudioClip Audio;
    [TextArea]
    public string ClosedCaptioning;
}