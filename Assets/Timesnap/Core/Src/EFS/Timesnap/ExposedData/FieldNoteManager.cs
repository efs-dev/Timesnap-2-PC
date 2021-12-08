using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;

public class FieldNoteManager : MonoBehaviour
{
    public static List<FieldNoteEntry> Entries = new List<FieldNoteEntry>();

    public void Awake()
    {
        Entries.Clear();

        var fieldNotes = Resources.Load<TextAsset>("FieldNotes");

        var lines = fieldNotes.text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        lines.RemoveAt(0);

        //Debug.Log("lines: " + lines.Count);

        lines.ForEach(line =>
        {
            var columns = line.Split(new string[] { "\t" }, StringSplitOptions.None);

            Entries.Add(new FieldNoteEntry() { Id = columns[0].ToLower(), Note = columns[1], Sources = columns[2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().ConvertAll(x => x.Trim()), Tags = columns[3].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().ConvertAll(x => x.Trim()), Conflicts = columns[4].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().ConvertAll(x=>x.Trim()) });
        });
    }

    public static bool CollectedFieldNote(string id)
    {
        return Entries.Find(x => x.Id == id && x.Collected) != null;
    }

    public static List<FieldNoteEntry> ConflictingFieldNotes(string id)
    {
        var fieldNote = Entries.Find(x => x.Id == id);
        return Entries.FindAll(x => x.Collected && (fieldNote.Conflicts.Contains(x.Id) || x.Conflicts.Contains(fieldNote.Id)));
    }
}

public class FieldNoteEntry
{
    public string Id;
    public string Note;
    public List<string> Sources = new List<string>();
    public string OverrideSource;
    public List<string> Tags = new List<string>();
    public List<string> Conflicts = new List<string>();

    public bool Collected = false;
}