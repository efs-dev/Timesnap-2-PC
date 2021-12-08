using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using sharpPDF;

public class EmailController
{

    public static void SendGmail(GmailCredentials credentials, EmailData data)
    {
        MailMessage mail = new MailMessage();

        mail.From = new MailAddress(data.From);
        mail.To.Add(data.To);
        mail.Subject = data.Subject;
        mail.Body = data.Body;

        if (data.BCCSender)
            mail.Bcc.Add(mail.From);

        for (var i = 0; i < data.Attachments.Count; i++)
        {
            mail.Attachments.Add(data.Attachments[i]);
        }


        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential(credentials.Username, credentials.Password) as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
        smtpServer.Send(mail);
        //smtpServer.SendMailAsync(mail);
        Debug.Log("success");

    }
    /*
    public static void TestSendFinalPdf()
    {
        pdfDocument myDoc = new pdfDocument("Timesnap Report", "Student", false);
        pdfPage myFirstPage = myDoc.addPage();
        myFirstPage.addText("You created a timesnap report from " + Application.platform, 50, 50, sharpPDF.Enumerators.predefinedFont.csCourier, 15);

        var emailData = new EmailData("mus.timesnap@gmail.com", "dale.efs@gmail.com", "test", "this is a test sent from: " + Application.platform);

        using (System.IO.Stream stream = new System.IO.MemoryStream())
        {
            myDoc.createPDF(stream);
            stream.Position = 0;

            emailData.AddAttachment(stream, "timesnap_report.pdf");
            EmailController.SendGmail(new GmailCredentials("mus.timesnap@gmail.com", "efstimesnap"), emailData);

        }
    }
    */
}

public class EmailData
{
    public string From;
    public string To;
    public string Subject;
    public string Body;

    public bool BCCSender;

    public List<Attachment> Attachments = new List<Attachment>();

    public EmailData(string from, string to, string subject, string body)
    {
        From = from;
        To = to;
        Subject = subject;
        Body = body;
    }

    public void AddAttachment(System.IO.Stream stream, string name)
    {
        Attachments.Add(new Attachment(stream, name));
    }

    public void AddAttachment(pdfDocument pdf, string name)
    {
        System.IO.Stream stream = new System.IO.MemoryStream();

        pdf.createPDF(stream);
        stream.Position = 0;

        AddAttachment(stream, name);
    }

    public void AddAttachment(AudioClip clip, string name)
    {
      /*  var stream = SavWav.WriteToStream(clip);
        stream.Position = 0;

        AddAttachment(stream, name);
        */
    }

}

public class GmailCredentials
{
    public string Username;
    public string Password;

    public GmailCredentials(string username, string password)
    {
        Username = username;
        Password = password;
    }
}