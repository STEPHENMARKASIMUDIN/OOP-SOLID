using Dapper;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;


/// <summary>
/// Summary description for ApproversUpdate
/// </summary>
public class ApproversUpdate
{
    private readonly ILog _Logger = LogManager.GetLogger(typeof(ApproversUpdate));
    public ApproversUpdate()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public virtual Response Update(MySqlConnection Connection, IModel Model, MySqlTransaction Transaction) 
    {
        try
        {
            Models.Admin data = (Models.Admin)Model;
            Dictionary<string, object> ApproverUpdateParam = new Dictionary<string, object>()
                                {
                                    {"_username", data.username},
                                    {"_password", data.password},
                                    {"_firstname",data.firstname},
                                    {"_middlename", data.middlename},
                                    {"_lastname", data.lastname},
                                    {"_division", data.division},
                                    {"_level", data.level},
                                    {"_operator_id", data.operator_id},
                                    {"_isActive", data.isActive},
                                    {"_contact_number", data.contact_number},
                                    {"_email", data.email},
                                };
            int updateResult = Connection.Execute(StoredProcedures.APPROVERS_UPDATE, ApproverUpdateParam, Transaction, 60, CommandType.StoredProcedure);
            if (updateResult < 1)
            {
                _Logger.Error(string.Format("Approver Update Failed: {0}", data.Serialize()));
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
            _Logger.Info(string.Format("Approver Update successfull: {0}", data.Serialize()));
            return new Response { ResponseCode = 200, ResponsMessage = string.Format("{0} user {1} {2} {3} was successfully updated. Thank you.", data.level, data.firstname, data.middlename, data.lastname) };
        }
        catch (MySqlException mex)
        {
            Transaction.Rollback();
            _Logger.Error(string.Format("Approver Update Transaction Failed: {0}", Model.Serialize()));
            _Logger.Fatal(mex.ToString());
            return new Response { ResponseCode = 400, ResponsMessage = "Unable to process request. Please check your data and try again." };
        }
        catch (TimeoutException tex)
        {
            Transaction.Rollback();
            _Logger.Error(string.Format("Approver Update Transaction Failed: {0}", Model.Serialize()));
            _Logger.Fatal(tex.ToString());
            return new Response { ResponseCode = 408, ResponsMessage = "Unable to process request. Connection timeout occured. Please try again later." };
        }
        catch (Exception ex)
        {
            Transaction.Rollback();
            _Logger.Error(string.Format("Approver Update Transaction Failed: {0}", Model.Serialize()));
            _Logger.Fatal(ex.ToString());
            return new Response { ResponseCode = 500, ResponsMessage = "Unable to process request. The system encountered some technical problem. Sorry for the inconvenience." };
        }
    }
}