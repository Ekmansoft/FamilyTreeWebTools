using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeWebTools.Services
{
  public class SendMailClass
  {
    private static TraceSource trace = new TraceSource("EmailClass", SourceLevels.Warning);

    public static void SendMail(string sender, string receiver, string subject, string message)
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
      //SmtpClient SMTPServer = new SmtpClient("127.0.0.1");
      SmtpClient client = new SmtpClient("smtp.gmail.com");
      client.UseDefaultCredentials = false;
      client.EnableSsl = true;
      client.Credentials = new NetworkCredential("bkekman@gmail.com", "cvidwfizhpnxqogd");
      try
      {
        client.Send(mailObj);
        //SMTPServer.Send(mailObj);
      }
      catch (Exception ex)
      {
        trace.TraceData(TraceEventType.Error, 0, ex.ToString());
      }
      trace.TraceEvent(TraceEventType.Information, 0, " send mail done");
    }
  }
}
