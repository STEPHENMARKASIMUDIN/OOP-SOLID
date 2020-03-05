using Dapper;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;

/// <summary>
/// Summary description for AdminLogin
/// </summary>
public class AdminLogin
{
    private readonly ILog _Logger = LogManager.GetLogger(typeof(AdminLogin));
    public AdminLogin() { }
   
   public virtual LoginResponse Login(MySqlConnection Connection,  IModel Model) 
    {
        try
        {
            Models.Login data = (Models.Login)Model;
            Models.Admin loginData = Connection.Query<Models.Admin>(StoredProcedures.ADMIN_LOGIN, new { _username = data.username, _password = data.password }, null, false, 60, CommandType.StoredProcedure).FirstOrDefault();
            if (loginData == null)
            {
                _Logger.Info(string.Format("admin username: {0} password: {1}", data.username, data.password));
                return new LoginResponse { ResponseCode = 404, ResponsMessage = "Invalid Credentials!" };
            }
            if (loginData.isActive == 0)
            {
                return new LoginResponse { ResponseCode = 404, ResponsMessage = "We're still processing your request. Thank you!" };
            }

            return new LoginResponse { ResponseCode = 200, ResponsMessage = "Success", adminData = loginData };
        }
        catch (MySqlException mex)
        {
            _Logger.Fatal(mex.ToString());
            return new LoginResponse { ResponseCode = 400, ResponsMessage = "Unable to process request. Please check your data and try again." };
        }
        catch (TimeoutException tex)
        {
            _Logger.Fatal(tex.ToString());
            return new LoginResponse { ResponseCode = 408, ResponsMessage = "Unable to process request. Connection timeout occured. Please try again later." };
        }
        catch (Exception ex)
        {
            _Logger.Fatal(ex.ToString());
            return new LoginResponse { ResponseCode = 500, ResponsMessage = "Unable to process request. The system encountered some technical problem. Sorry for the inconvenience." };
        }
    }
}