using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for EmailHandler
/// </summary>
public sealed class EmailHandler : Models
{
    
    private readonly log4net.ILog _Logger = log4net.LogManager.GetLogger(typeof(EmailHandler));
    private string _Host = string.Empty;
    private string _Email = string.Empty;
    private string _User = string.Empty;
    private string _Pass = string.Empty;
    
    private static EmailHandler _Intance;
    private static readonly object _InstanceLock = new object();
    public static EmailHandler Instance
    {
        get
        {
            if (_Intance == null)
            {
                lock (_InstanceLock)
                {
                    if (_Intance == null)
                    {
                        _Intance = new EmailHandler();

                    }
                }
            }
            return Instance;
        }
    }
    public EmailHandler() 
    {
        var ini = new IniFile(Path);

        Email.Host = ini.IniReadValue("Config MailAmazon", "host");
        Email.EmailAdd = ini.IniReadValue("Config MailAmazon", "emailadd");
        Email.SenderUser = ini.IniReadValue("Config MailAmazon", "senderuser");
        Email.SenderPass = ini.IniReadValue("Config MailAmazon", "senderpass");
        Email.Port = ini.IniReadValue("Config MailAmazon", "port");
        Email.Link = ini.IniReadValue("Config MailAmazon", "link");
    }
   

  
    public void SendEmail(string receiver,string bodyMessage)
    {

        string subject = "ML Partner Registration";
        try
        {               
            var mailMessage = new MailMessage(Email.EmailAdd, receiver)
            {
                Subject = subject
            };
     
            mailMessage.Body = bodyMessage;
            mailMessage.IsBodyHtml = true;

            var emailClient = new SmtpClient();
            var auth = new NetworkCredential(Email.SenderUser, Email.SenderPass);
            emailClient.Host = Email.Host;
            emailClient.UseDefaultCredentials = true;
            emailClient.Credentials = auth;
            emailClient.Port = Email.Port.ParseInt();
            emailClient.EnableSsl = false;
            emailClient.Send(mailMessage);

            _Logger.Info(string.Format("amazon email successful: receiver: {0}",  receiver));
        }
        catch (Exception ex)
        {
            _Logger.Fatal(ex.ToString());

            try
            {
                EmailConfigGDC(ref _Host, ref _Email, ref _User, ref _Pass);
         
                var mailMessage = new MailMessage(_Email, receiver)
                {
                    Subject = subject
                };
           
              
                mailMessage.Body = bodyMessage;
                mailMessage.IsBodyHtml = true;

                var emailClient = new SmtpClient();
                var auth = new NetworkCredential(_User, _Pass);
                emailClient.Host = _Host;
                emailClient.UseDefaultCredentials = true;
                emailClient.Credentials = auth;
                emailClient.EnableSsl = false;
                emailClient.Send(mailMessage);

                _Logger.Info(string.Format("GDC email successful: memo: receiver: {0}", receiver));
            }
            catch (Exception exp)
            {
                _Logger.Fatal(exp.ToString());
            }
        }
    }
    private void EmailConfigGDC(ref string host, ref string email, ref string user, ref string pass)
    {
        var ini = new IniFile(Path);

        host = ini.IniReadValue("Config Mail", "host");
        email = ini.IniReadValue("Config Mail", "emailadd");
        user = ini.IniReadValue("Config Mail", "senderuser");
        pass = ini.IniReadValue("Config Mail", "senderpass");
    }

   
}