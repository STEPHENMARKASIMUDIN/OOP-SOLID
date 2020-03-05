using log4net;
using System;
using System.IO;
using System.Net;
using System.Threading;

/// <summary>
/// Summary description for RequestHandler
/// </summary>
public class RequestHandler
{
    #region Private Variables
    private readonly Uri _Url = null;
    private readonly string _Method = string.Empty;
    private readonly string _ContentType = string.Empty;
    private readonly byte[] _jsonData = null;
    private static readonly ILog kplog = LogManager.GetLogger(typeof(RequestHandler));
    #endregion

    /*<summary>
        HTTPPostRequest Constructor
     <summary>*/
    public RequestHandler(Uri url, string Method, string ContentType, byte[] jsonData)
    {
        _Url = url;
        _Method = Method;
        _ContentType = ContentType;
        _jsonData = jsonData;
    }

    /*<summary>
        HTTPGetRequest Constructor
     <summary>*/
    public RequestHandler(Uri url, string Method, string ContentType)
    {
        _Url = url;
        _Method = Method;
        _ContentType = ContentType;
    }
    public RequestHandler()
    {

    }

    public virtual string HttpGetRequest()
    {
        int attempt = 0;
        do
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                if (attempt > 0)
                {

                    if (attempt == 1)
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    }
                    else
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                    }
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_Url) as HttpWebRequest;
                request.Method = _Method;
                request.ContentType = _ContentType;
                request.Credentials = CredentialCache.DefaultCredentials;
                //request.Timeout = Timeout.Infinite;
                WebResponse webresponse = request.GetResponse();
                string res = null;
                using (Stream response = webresponse.GetResponseStream())
                {
                    if (webresponse != null)
                    {
                        using (StreamReader reader = new StreamReader(response))
                        {
                            res = reader.ReadToEnd();
                            reader.Close();
                            webresponse.Close();
                        }
                    }
                }
                return res;
            }
            catch (WebException ex)
            {
                kplog.Fatal("Error Details: {0} : " + ex.ToString());
                if (ex.ToString().ToLower().Contains("underlying") || ex.ToString().ToLower().Contains("ssl"))
                {
                    if (attempt < 2)
                    {
                        attempt++;
                        Thread.Sleep(attempt * 2000);
                    }
                    else
                    {
                        try
                        {
                            using (WebResponse response = ex.Response)
                            {
                                HttpWebResponse httpResponse = (HttpWebResponse)response;
                                kplog.Fatal("Error code: {0} : " + httpResponse.StatusCode);
                                using (Stream data = response.GetResponseStream())
                                using (StreamReader reader = new StreamReader(data))
                                {
                                    string text = reader.ReadToEnd();
                                    string respcode = httpResponse.StatusCode.ToString();
                                    if (respcode == "422")
                                    {
                                        kplog.Info("_QueryString: " + _Url + " Response Message : Already fulfilled");
                                        kplog.Info("_QueryString: " + _Url + " Response Message : " + text);
                                        return "ERROR" + "|" + text;
                                    }
                                    kplog.Info("_QueryString: " + _Url + " Response Message : " + text);
                                    return "ERROR" + "|" + text;
                                }
                            }
                        }
                        catch (Exception tex)
                        {
                            return "ERROR" + "|" + tex.ToString();
                        }
                    }
                }
                else
                {
                    return "ERROR" + "|" + ex.ToString();
                }
            }
        } while (true);
    }
    public virtual string HttpPostRequest()
    {
        int attempt = 0;
        do
        {
            try
            {

                if (attempt > 0)
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    if (attempt == 1)
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                    }
                    else
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    }
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_Url) as HttpWebRequest;
                request.Method = _Method;
                request.ContentType = _ContentType;
                request.ContentLength = _jsonData.Length;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.KeepAlive = false;
                //request.Timeout = Timeout.Infinite;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(_jsonData, 0, _jsonData.Length);
                    stream.Close();
                }
                WebResponse webresponse = request.GetResponse();
                string res = null;
                using (Stream response = webresponse.GetResponseStream())
                {
                    if (webresponse != null)
                    {
                        using (StreamReader reader = new StreamReader(response))
                        {
                            res = reader.ReadToEnd();
                            reader.Close();
                            webresponse.Close();
                        }
                    }
                }
                return res;
            }
            catch (WebException ex)
            {
                kplog.Fatal("Error #03: " + "uri: " + _Url.ToString() + string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", " | jsonDataBytes: ", _jsonData, " | contentType: ", _ContentType, " | method: ", _Method, " - ", ex));
                if (ex.ToString().ToLower().Contains("underlying") || ex.ToString().ToLower().Contains("ssl"))
                {
                    if (attempt < 2)
                    {
                        attempt++;
                        Thread.Sleep(attempt * 2000);
                    }
                    else
                    {
                        try
                        {
                            using (WebResponse response = ex.Response)
                            {
                                HttpWebResponse httpResponse = (HttpWebResponse)response;
                                kplog.Fatal("Error code: {0} : " + httpResponse.StatusCode);
                                using (Stream data = response.GetResponseStream())
                                using (StreamReader reader = new StreamReader(data))
                                {
                                    string text = reader.ReadToEnd();
                                    string respcode = httpResponse.StatusCode.ToString();
                                    if (respcode == "422")
                                    {
                                        kplog.Info("_QueryString: " + _Url + " Response Message : Already fulfilled");
                                        kplog.Info("_QueryString: " + _Url + " Response Message : " + text);
                                        return "ERROR" + "|" + text;
                                    }
                                    kplog.Info("_QueryString: " + _Url + " Response Message : " + text);
                                    return "ERROR" + "|" + text;
                                }
                            }
                        }
                        catch (Exception tex)
                        {
                            return "ERROR" + "|" + tex.ToString();
                        }
                    }
                }
                else
                {
                    return "ERROR" + "|" + ex.ToString();
                }

            }
        } while (true);

    }
    public virtual string PostSMS(Uri uri, byte[] jsonDataBytes, string contentType, string method)
    {
        int attempt = 0;
        do
        {
            try
            {
                kplog.Info(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", "Uri: ", uri, " | jsonDataBytes: ", jsonDataBytes, " | contentType: ",
                    contentType, " | method: ", method));
                if (attempt > 0)
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    if (attempt == 1)
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                    }
                    else
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    }
                }
                WebRequest req = WebRequest.Create(uri);
                req.ContentType = contentType;
                req.Method = method;
                req.ContentLength = jsonDataBytes.Length;
                //req.Timeout = Timeout.Infinite;

                Stream stream = req.GetRequestStream();
                stream.Write(jsonDataBytes, 0, jsonDataBytes.Length);
                stream.Close();

                WebResponse webresponse = req.GetResponse();
                Stream response = webresponse.GetResponseStream();

                string res = null;
                if (response != null)
                {
                    StreamReader reader = new StreamReader(response);
                    res = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                }
                kplog.Info(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", "uri: ", uri.ToString(), " | jsonDataBytes: ", jsonDataBytes,
                    " | contentType: ", contentType, " | method: ", method, " - ", res));
                return res;
            }
            catch (WebException ex)
            {
                kplog.Fatal("Error Details: {0} : " + ex.ToString());
                if (ex.ToString().ToLower().Contains("underlying") || ex.ToString().ToLower().Contains("ssl"))
                {
                    if (attempt < 2)
                    {
                        attempt++;
                        Thread.Sleep(attempt * 2000);
                    }
                    else
                    {
                        try
                        {
                            using (WebResponse response = ex.Response)
                            {
                                HttpWebResponse httpResponse = (HttpWebResponse)response;
                                kplog.Fatal("Error code: {0} : " + httpResponse.StatusCode);
                                using (Stream data = response.GetResponseStream())
                                using (StreamReader reader = new StreamReader(data))
                                {
                                    string text = reader.ReadToEnd();
                                    string respcode = httpResponse.StatusCode.ToString();
                                    if (respcode == "422")
                                    {
                                        kplog.Info("_QueryString: " + uri.ToString() + " Response Message : Already fulfilled");
                                        kplog.Info("_QueryString: " + uri.ToString() + " Response Message : " + text);
                                        return "ERROR" + "|" + text;
                                    }
                                    kplog.Info("_QueryString: " + uri.ToString() + " Response Message : " + text);
                                    return "ERROR" + "|" + text;
                                }
                            }
                        }
                        catch (Exception tex)
                        {

                            return "ERROR" + "|" + tex.ToString();
                        }
                    }
                }
                else
                {
                    return "ERROR" + "|" + ex.ToString();
                }

            }
        } while (true);
    }
}