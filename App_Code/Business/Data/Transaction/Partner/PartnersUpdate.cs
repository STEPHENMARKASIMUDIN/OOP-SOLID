using Dapper;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;


/// <summary>
/// Summary description for PartnersUpdate
/// </summary>
public class PartnersUpdate
{
    private readonly ILog _Logger = LogManager.GetLogger(typeof(PartnersUpdate));
    public PartnersUpdate()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public virtual Response Update(MySqlConnection Connection, IModel Model, MySqlTransaction Transaction) 
    {
        try
        {
            Models.PartnersData data = (Models.PartnersData)Model;
            Dictionary<string, object> PartnersUpdateParam = new Dictionary<string, object>()
                        {
                         {"_username", data.username},
                         {"_password", data.password},
                         {"_attachment_1", data.attachment_1.IsNull() ? "" : data.attachment_1},
                         {"_attachment_2",data.attachment_2.IsNull() ? "" : data.attachment_2},
                         {"_attachment_3", data.attachment_3.IsNull() ? "" : data.attachment_3},
                         {"_attachment_4", data.attachment_4.IsNull() ? "" : data.attachment_4},
                         {"_attachment_5", data.attachment_5.IsNull() ? "" : data.attachment_5},
                         {"_attachment_6", data.attachment_6.IsNull() ? "" : data.attachment_6},
                         {"_attachment_7", data.attachment_7.IsNull() ? "" : data.attachment_7},
                         {"_attachment_8",data.attachment_8.IsNull() ? "" : data.attachment_8},
                         {"_attachment_9", data.attachment_9.IsNull() ? "" : data.attachment_9},
                         {"_attachment_10",data.attachment_10.IsNull() ? "" : data.attachment_10},
                        };

            int updateResult = Connection.Execute(StoredProcedures.PARTNERS_UPDATE, PartnersUpdateParam, Transaction, 60, CommandType.StoredProcedure);
            if (updateResult < 1)
            {
                _Logger.Error(string.Format("Partners Update Failed: {0}", data.Serialize()));
                Transaction.Rollback();
                return new Response { ResponseCode = 400, ResponsMessage = "Unable to process request. Please try again later." };
            }
            //EmailHandler email = new EmailHandler();

            //Thread execute = new Thread(delegate()
            //{
            //    email.SendEmail("email" + "|", "partners name", "series", 1);
            //});
            //execute.IsBackground = true;
            //execute.Start();
            Transaction.Commit();
            _Logger.Info(string.Format("Partners Update successfull: {0}", data.Serialize()));
            return new Response { ResponseCode = 200, ResponsMessage = "Successfully resubmitted to FSD and wait for verification." };
        }
        catch (MySqlException mex)
        {
            Transaction.Rollback();
            _Logger.Error(string.Format("Partners Update Transaction Failed: {0}", Model.Serialize()));
            _Logger.Fatal(mex.ToString());
            return new Response { ResponseCode = 400, ResponsMessage = "Unable to process request. Please check your data and try again." };
        }
        catch (TimeoutException tex)
        {
            Transaction.Rollback();
            _Logger.Error(string.Format("Partners Update Transaction Failed: {0}", Model.Serialize()));
            _Logger.Fatal(tex.ToString());
            return new Response { ResponseCode = 408, ResponsMessage = "Unable to process request. Connection timeout occured. Please try again later." };
        }
        catch (Exception ex)
        {
            Transaction.Rollback();
            _Logger.Error(string.Format("Partners Update Transaction Failed: {0}", Model.Serialize()));
            _Logger.Fatal(ex.ToString());
            return new Response { ResponseCode = 500, ResponsMessage = "Unable to process request. The system encountered some technical problem. Sorry for the inconvenience." };
        }
    }
}