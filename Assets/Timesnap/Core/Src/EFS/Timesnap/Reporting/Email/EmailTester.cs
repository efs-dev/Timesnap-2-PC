using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Linq;

using sharpPDF;

public class EmailTester : MonoBehaviour {

    public bool Debug;
    //public Sprite Texture;
    public AudioClip AudioClip;

    private VRKeyboard _keyboard;


    private int pageHeight = 800;

    private int titleSize = 18;
    private int headerSize = 16;
    private int subtitleSize = 12;
    private int textSize = 10;


    int WriteCategoryToPdf(pdfPage myFirstPage, string category, int y, string categoryTitle="")
    {
        myFirstPage.addText("     - " + (string.IsNullOrEmpty(categoryTitle) ? category : categoryTitle), 40, pageHeight - y, sharpPDF.Enumerators.predefinedFont.csTimesBold, subtitleSize, "category");
        y += 14;

        FieldNoteManager.Entries.FindAll(x => x.Tags.Count > 0 && x.Tags[0] == category).ForEach(note =>
        {
            UnityEngine.Debug.Log("checking note " + note.Id + " in category " + category + ", collected = " + note.Collected);
            if (note.Collected || Debug || category == "DEF")
            {
                var hasMatching = FieldNoteManager.HasMatching(note.Id, Debug);// FieldNoteManager.ConflictingFieldNotes(note.Id).Count > 0;
                var source = "[Source: " + note.OverrideSource + "] ";

                if (string.IsNullOrEmpty(note.OverrideSource))
                {
                    if (note.Sources.Count > 0)
                        source = "[Source: " + note.Sources.Aggregate((a, b) => a + ", " + b) + "] ";
                    else
                        source = "";
                }
                myFirstPage.addText("          - " + source + note.Note + (hasMatching ? "*" : ""), 40, pageHeight - y, sharpPDF.Enumerators.predefinedFont.csTimes, textSize, "entry");
                y += 13;
            }
        });

        return y;
    }
    
	// Use this for initialization
	void Start () {
        _keyboard = GetComponent<VRKeyboard>();
        /*_keyboard.OnSubmitCallback = (value) =>
        {
        };
        */
    }

    private List<string> serverURLs = new List<string>()
    {
        "https://script.google.com/macros/s/AKfycbycDQIbTjOIo28uU5uRZx-KFQ33LlKq6TzenZAeqQprn5rMIhDG/exec",
        "https://script.google.com/macros/s/AKfycbz_FjnFksWo1BrcTWUrikhbv-u-HxiDfStv3_6ohD3YKAvz6ww/exec"
    };

    public void SendEmail()
    {
        var y = 50;

        var date = "Date: " + DateTime.Now.Month + "/" + DateTime.Now.Day + "/2047 " + (DateTime.Now.Hour % 12) + ":" + DateTime.Now.Minute + " " + (DateTime.Now.Hour >= 12 ? "pm" : "am");

        pdfDocument myDoc = new pdfDocument("Timesnap Report", _keyboard.EnteredText, false);
        pdfPage myFirstPage = myDoc.addPage();
        myFirstPage.addText("C.A.R.P.A. Field Notes: The Fugitive Slave Law", 40, pageHeight - y, sharpPDF.Enumerators.predefinedFont.csTimesBold, titleSize, "title");
        y += 14;
        myFirstPage.addText("Agent: " + _keyboard.EnteredText, 40, pageHeight - y, sharpPDF.Enumerators.predefinedFont.csTimes, textSize, "subtitle");
        y += 14;
        myFirstPage.addText(date, 40, pageHeight - y, sharpPDF.Enumerators.predefinedFont.csTimes, textSize, "subtitle");
        y += 14;
        myFirstPage.addText("Timesnap Device: " + SystemInfo.deviceUniqueIdentifier, 40, pageHeight - y, sharpPDF.Enumerators.predefinedFont.csTimes, textSize, "subtitle");
        y += 14;
        myFirstPage.addText("Field notes you have collected during your mission have been compiled below, along with the source of each note.", 40, pageHeight - y, sharpPDF.Enumerators.predefinedFont.csTimesOblique, textSize, "subtitle");
        y += 30;
        myFirstPage.addText("Effects of the Fugitive Slave Law", 40, pageHeight - y, sharpPDF.Enumerators.predefinedFont.csTimesBold, headerSize, "header");
        y += 15;

        //y += 10;
        y = WriteCategoryToPdf(myFirstPage, "PA", y, "Christiana, Pennsylvania");
        y += 10;
        y = WriteCategoryToPdf(myFirstPage, "MA", y, "Boston, Massachusetts");
        y += 10;
        y = WriteCategoryToPdf(myFirstPage, "OH", y, "Cincinnati, Ohio");
        y += 10;
        y = WriteCategoryToPdf(myFirstPage, "DEF", y, "Definitions");
        y += 15;

        myFirstPage.addText("* This evidence matches evidence you have found in another state.", 40, pageHeight - y, sharpPDF.Enumerators.predefinedFont.csTimes, textSize - 1, "footer");


        // myFirstPage.addImage(Texture.texture.EncodeToJPG(100), 5, 5, 250, 250);

        var emailData = new EmailData("mus.timesnap@gmail.com", _keyboard.EnteredText, "C.A.R.P.A. Mission Report: The Fugitive Slave Law", "Good work, agent!\nAttached is your mission recap.");
        emailData.BCCSender = true;
        emailData.AddAttachment(myDoc, "CARPAMissionReport.pdf");
        /*
        var audioRecorders = GetComponentsInChildren<AudioRecorder>();
        for (var i = 0; i < audioRecorders.Length; i++)
        {
            var ar = audioRecorders[i];
            if (ar.RecordedAudio != null)
                emailData.AddAttachment(ar.RecordedAudio, "clip" + i + ".wav");
        }
        */

        UnityEngine.Debug.Log(myFirstPage.GetTextJSON());
        //var obj = LitJson.JsonMapper.ToObject(myFirstPage.GetTextJSON());
        //UnityEngine.Debug.Log(obj);
        GameObject.Find("TS Engine").GetComponent<TSEngine>().StartCoroutine(PostEmail(_keyboard.EnteredText, myFirstPage.GetTextJSON(), 0));
    }

    IEnumerator PostEmail(string to, string pdfJson, int index)
    {
        UnityEngine.Debug.Log("PostEmail from index: " + index);
        WWWForm form = new WWWForm();
        form.AddField("to", to);
        form.AddField("pdf", pdfJson);
        form.AddField("subject", "C.A.R.P.A. Mission Report: The Fugitive Slave Law");



        WWW www = new WWW(serverURLs[index], form);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            UnityEngine.Debug.Log(www.error);
        }
        else
        {
            if (!www.text.StartsWith("doPost to"))
            {
                if (index < serverURLs.Count-1)
                {
                    UnityEngine.Debug.Log(www.text);
                    UnityEngine.Debug.Log("Could not send email, trying next.");
                    yield return PostEmail(to, pdfJson, index + 1);
                }
                else
                {
                    UnityEngine.Debug.Log("Could not send email and out of fallbacks.");
                }
            }
            else
            {
                UnityEngine.Debug.Log("Finished sending, response: " + www.text);
            }
        }
        //EmailController.SendGmail(new GmailCredentials("mus.timesnap@gmail.com", "efstimesnap"), emailData);
    }
}
