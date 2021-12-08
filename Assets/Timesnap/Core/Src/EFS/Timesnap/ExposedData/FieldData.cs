using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FieldData.asset", menuName ="TimeSnap/Field Data", order =0)]
public class FieldData : ScriptableObject
{
    public string Id;
    public string DisplayName;
    [TextArea]
    public string Description;
    public List<string> Tags;
    public List<string> ConflictedWith;
    public Sprite Image;
}
