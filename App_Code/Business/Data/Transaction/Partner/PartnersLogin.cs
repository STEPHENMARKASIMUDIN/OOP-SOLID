using Dapper;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;

/// <summary>
/// Summary description for PartnersLogin
/// </summary>
public class PartnersLogin
{
    private readonly ILog _Logger = LogManager.GetLogger(typeof(PartnersLogin));
    public PartnersLogin()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public virtual LoginResponse Login(MySqlConnection Connection, IModel Model) 
    {
        try
        {
            Models.Login data = (Models.Login)Model;
            Models.PartnersData loginData = Connection.Query<Models.PartnersData>(StoredProcedures.PARTNERS_LOGIN, new { _username = data.username, _password = data.password }, null, false, 60, CommandType.StoredProcedure).FirstOrDefault();
            if (loginData == null)
            {
                _Logger.Info(string.Format("username: {0} password: {1}", data.username, data.password));
                return new LoginResponse { ResponseCode = 404, ResponsMessage = "Invalid Credentials!" };
            }
            if (loginData.isApproved == 0)
            {
                return new LoginResponse { ResponseCode = 404, ResponsMessage = "We're still processing your request. Thank you!" };
            }
            if (loginData.isApproved == 2)
            {
                return new LoginResponse { ResponseCode = 404, ResponsMessage = "Your request was disapproved!" };
            }
            return new LoginResponse { ResponseCode = 200, ResponsMessage = "Success", loginData = loginData };
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