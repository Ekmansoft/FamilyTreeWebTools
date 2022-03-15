using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Ekmansoft.FamilyTree.WebTools.Services
{
  public class SendMailClass
  {
    private static TraceSource trace = new TraceSource("EmailClass", SourceLevels.Warning);

    public static void SendMail(string sender, string credentialAddress, string credentialPassword, string receiver, string subject, string message, IList<string> attachmentFiles = null)
    {
      trace.TraceEvent(TraceEventType.Information, 0, " sending mail from:" + sender + " to:" + receiver + " msg-len:" + message.Length);
      MailMessage mailObj = new MailMessage();
      mailObj.From = new MailAddress(sender);
      mailObj.To.Add(new MailAddress(receiver));
      mailObj.Subject = subject;
      mailObj.IsBodyHtml = true;
      mailObj.Body = message;
      mailObj.BodyEncoding = System.Text.Encoding.UTF8;
      mailObj.SubjectEncoding = System.Text.Encoding.UTF8;

      if ((attachmentFiles != null) && (attachmentFiles.Count > 0))
      {
        foreach (string attachmentFile in attachmentFiles)
        {
          // Create  the file attachment for this email message.
          Attachment data = new Attachment(attachmentFile, MediaTypeNames.Application.Octet);
          // Add time stamp information for the file.
          ContentDisposition disposition = data.ContentDisposition;
          disposition.CreationDate = System.IO.File.GetCreationTime(attachmentFile);
          disposition.ModificationDate = System.IO.File.GetLastWriteTime(attachmentFile);
          disposition.ReadDate = System.IO.File.GetLastAccessTime(attachmentFile);
          // Add the file attachment to this email message.
          mailObj.Attachments.Add(data);
        }
      }

      //SmtpClient SMTPServer = new SmtpClient("127.0.0.1");
      SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
      client.UseDefaultCredentials = false;
      client.EnableSsl = true;
      //client.Port = 587;
      client.DeliveryMethod = SmtpDeliveryMethod.Network;
      client.Credentials = new NetworkCredential(credentialAddress, credentialPassword);
      try
      {
        client.Send(mailObj);
        //SMTPServer.Send(mailObj);
      }
      catch (Exception ex)
      {
        trace.TraceData(TraceEventType.Error, 0, ex.ToString());
        trace.TraceEvent(TraceEventType.Warning, 0, " email-credentials:" + credentialAddress + ":" + credentialPassword);
      }
      trace.TraceEvent(TraceEventType.Information, 0, " send mail done");
    }
  }
}
