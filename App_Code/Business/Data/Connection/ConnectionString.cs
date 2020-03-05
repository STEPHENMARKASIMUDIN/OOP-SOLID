using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ConnectionString
/// </summary>
public class ConnectionString : IConnection
{
    public string GetConnectionString()
    {
        var ini = new IniFile(Models.Path);
        string server = ini.IniReadValue("DBConfig PartnerInegration", "Server");
        string database = ini.IniReadValue("DBConfig PartnerInegration", "Database");
        string usrID = ini.IniReadValue("DBConfig PartnerInegration", "UID");
        string passwd = ini.IniReadValue("DBConfig PartnerInegration", "Password");
        string pool = ini.IniReadValue("DBConfig PartnerInegration", "Pool");
        int maxCon = (ini.IniReadValue("DBConfig PartnerInegration", "MaxCon")).ParseInt();
        int minCon = (ini.IniReadValue("DBConfig PartnerInegration", "MinCon")).ParseInt();
        int tout = (ini.IniReadValue("DBConfig PartnerInegration", "Tout")).ParseInt();
        bool ispool = pool.Equals("1") ? true : false;
        string constring = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}",
                "server = ", server, "; database = ", database, "; uid = ", usrID, "; password = ", passwd, "; pooling = ",
                ispool, "; min pool size= ", minCon, "; max pool size=", maxCon,
                ";connection lifetime=0; connection timeout=", tout, "");
        return constring;
    }
}