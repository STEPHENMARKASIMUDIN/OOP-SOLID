using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Summary description for SMSHandler
/// </summary>
public class SMSHandler:Models
{

    private readonly log4net.ILog _Logger = log4net.LogManager.GetLogger(typeof(EmailHandler));
    private static readonly object _InstanceLock = new object();
    private static SMSHandler _Instance;
    public static SMSHandler Instance { get
        {
            if (_Instance == null) 
            {
                lock (_InstanceLock)
                {
                    if (_Instance == null)
                    {
                        _Instance = new SMSHandler();
                    }
                }
            }
            return _Instance;
        } }

    public SMSHandler() 
    {
        var ini = new IniFile(Path);

        SMS.SMS_URL = ini.IniReadValue("Config Text", "url");
    }
  

    //sends sms notification to the walletuser
    public void SmsNotification(string contact,string message)
    {
        try
        {
            Uri uri = new Uri(SMS.SMS_URL + "/sendSMS");
            string post = JsonConvert.SerializeObject(new SMS { celno = contact, message = message });
            byte[] bytesParam = Encoding.UTF8.GetBytes(post);
            RequestHandler RequestHandler = new RequestHandler();
            string PostResp = RequestHandler.PostSMS(uri, bytesParam, "application/json", "POST");

            var resp = JsonConvert.DeserializeObject<SMSResponses>(PostResp);
            if (!resp.ResultCode.Equals("200"))
            {

                _Logger.Error(string.Format("contact: {0} msg: {1} ", contact, resp.ResultMessage));
            }
            else
            {
                _Logger.Info(string.Format("contact: {0} msg: {1}", contact, message));

            }

        }
        catch (Exception ex)
        {
            _Logger.Fatal(ex.ToString());

        }
        _Logger.Info("==================== SMS Done ========================");
    }

   
}