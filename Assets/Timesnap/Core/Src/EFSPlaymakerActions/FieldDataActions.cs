using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Time Snap")]
    public class GetFieldNoteData : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The id of the field data.")]
        public FsmString Id;

        public FsmString Note;

        public FsmString Source1;
        public FsmString Source2;
        public FsmString Source3;

        public FsmString Tag1;
        public FsmString Tag2;
        public FsmString Tag3;

        public FsmBool IsCollected;

        public override void Reset()
        {
            Id = null;
            Note = null;
            Source1 = null;
            Source2 = null;
            Source3 = null;
            Tag1 = null;
            Tag2 = null;
            Tag3 = null;
        }

        public override void OnEnter()
        {
            var id = Id.Value.ToLower();
            var fieldNote = FieldNoteManager.Entries.Find(x => x.Id == id);

            if (fieldNote != null)
            {
                Note.Value = fieldNote.Note;

                Source1.Value = fieldNote.Sources[0];
                Source2.Value = fieldNote.Sources.Count > 1 ? fieldNote.Sources[1] : "";
                Source3.Value = fieldNote.Sources.Count > 2 ? fieldNote.Sources[2] : "";

                Tag1.Value = fieldNote.Tags[0];
                Tag2.Value = fieldNote.Tags.Count > 1 ? fieldNote.Tags[1] : "";
                Tag3.Value = fieldNote.Tags.Count > 2 ? fieldNote.Tags[2] : "";

                IsCollected.Value = fieldNote.Collected;
            }

            Finish();
        }
    }

    public class CollectFieldNote : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The id of the field data.")]
        public FsmString Id;

        public FsmBool UseOverrideSource;
        public FsmString OverrideSource;

        public override void Reset()
        {
            Id = null;
        }

        public override void OnEnter()
        {
            var fieldNote = FieldNoteManager.Entries.Find(x => x.Id == Id.Value.ToLower());

            if (fieldNote != null)
            {
                fieldNote.Collected = true;

                if (UseOverrideSource.Value)
                    fieldNote.OverrideSource = OverrideSource.Value;
            }
            Finish();
        }
    }

    [ActionCategory("Time Snap")]
    public class GetMindMeldData : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The id of the mind meld data.")]
        public FsmString Id;

        [ObjectType(typeof(MindMeldData))]
        [Tooltip("Store the mind meld data object. Use GetProperty action to access its data.")]
        public FsmObject MindMeldData;


        [Tooltip("Store the mind meld choice data object. Use GetProperty action to access its data. Use -1 to ignore the choice data.")]
        public FsmInt ChoiceIndex = -1;

        [ObjectType(typeof(MindMeldChoicePM))]
        [Tooltip("Store the mind meld data object. Use GetProperty action to access its data.")]
        public FsmObject MindMeldChoiceData;

        public override void Reset()
        {
            Id = null;
            MindMeldData = null;
        }

        public override void OnEnter()
        {
            var mindmeldData = Resources.Load<MindMeldData>("MindMeldData/" + Id.Value);
            MindMeldData.Value = mindmeldData;

            if (ChoiceIndex.Value != -1)
            {
                var choiceData = mindmeldData.Choices[ChoiceIndex.Value];
                var choicePM = ScriptableObject.CreateInstance<MindMeldChoicePM>();
                choicePM.Data = choiceData;

                Debug.Log("created choice pm: " + choicePM);
                Debug.Log(choicePM.Data.NextMindMeldId);
                MindMeldChoiceData.Value = choicePM;
            }
        }

    }
}

public class MindMeldChoicePM : ScriptableObject
{
    public MindMeldChoiceData Data;
}