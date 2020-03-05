using Dapper;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// Summary description for ApproversRegistration
/// </summary>
public class ApproversRegistration
{
    private readonly ILog _Logger = LogManager.GetLogger(typeof(ApproversRegistration));
    public ApproversRegistration()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public virtual Response Registration(MySqlConnection Connection, IModel Model, MySqlTransaction Transaction) 
    {
        try
        {
            Models.Admin data = (Models.Admin)Model;
            Dictionary<string, object> ApproverRegistrationParam = new Dictionary<string, object>()
                                {
                                    {"_username", data.username},
                                    {"_password", data.password},
                                    {"_id_number", data.id_number},
                                    {"_firstname",data.firstname},
                                    {"_middlename", data.middlename},
                                    {"_lastname", data.lastname},
                                    {"_division", data.division},
                                    {"_level", data.level},
                                    {"_operator_id", data.operator_id},
                                    {"_contact_number", data.contact_number},
                                    {"_email", data.email},
                                };
            int registrationResult = Connection.Execute(StoredProcedures.APPROVER_REGISTRATION, ApproverRegistrationParam, Transaction, 60, CommandType.StoredProcedure);
            if (registrationResult < 1)
            {
                _Logger.Error(string.Format("Approver Registration Failed: {0}", data.Serialize()));
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
            _Logger.Info(string.Format("Approver Registration successfull: {0}", data.Serialize()));
            return new Response { ResponseCode = 200, ResponsMessage = string.Format("{0} user {1} {2} {3} was successfully added as new approver. Thank you.", data.level, data.firstname, data.middlename, data.lastname) };
        }
        catch (MySqlException mex)
        {
            Transaction.Rollback();
            _Logger.Error(string.Format("Approver Registration Transaction Failed: {0}", Model.Serialize()));
            _Logger.Fatal(mex.ToString());
            return new Response { ResponseCode = 400, ResponsMessage = "Unable to process request. Please check your data and try again." };
        }
        catch (TimeoutException tex)
        {
            Transaction.Rollback();
            _Logger.Error(string.Format("Approver Registration Transaction Failed: {0}", Model.Serialize()));
            _Logger.Fatal(tex.ToString());
            return new Response { ResponseCode = 408, ResponsMessage = "Unable to process request. Connection timeout occured. Please try again later." };
        }
        catch (Exception ex)
        {
            Transaction.Rollback();
            _Logger.Error(string.Format("Approver Registration Transaction Failed: {0}", Model.Serialize()));
            _Logger.Fatal(ex.ToString());
            return new Response { ResponseCode = 500, ResponsMessage = "Unable to process request. The system encountered some technical problem. Sorry for the inconvenience." };
        }
    }
}