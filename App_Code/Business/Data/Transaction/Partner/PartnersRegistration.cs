using Dapper;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

/// <summary>
/// Summary description for PartnersRegistration
/// </summary>
public class PartnersRegistration
{
    private readonly ILog _Logger = LogManager.GetLogger(typeof(PartnersRegistration));
    public PartnersRegistration()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public virtual Response Registration(MySqlConnection Connection,IModel Model, MySqlTransaction Transaction)
    {
        try
        {
            Models.PartnersData data = (Models.PartnersData)Model;

            //check username if exist
            int checkIfExist = Connection.Query<int>(StoredProcedures.IS_USERNAME_TAKEN, new { _username = data.username }, null, false, 40, CommandType.StoredProcedure).FirstOrDefault();
            if (checkIfExist > 0)
            {
                Transaction.Rollback();
                return new Response { ResponseCode = 401, ResponsMessage = "Username provided was already taken." };
            }
            Dictionary<string, object> RegistrationParam = new Dictionary<string, object>()
                      {
                        {"_username", data.username},
                        {"_password", data.password},
                        {"_business_name", data.business_name},
                        {"_email",data.email},
                        {"_contact_person", data.contact_person},
                        {"_contact_number", data.contact_number},
                        {"_memorandom_agreement", data.memorandom_agreement.IsNull() ? "" : data.memorandom_agreement},
                        {"_nondisclosure_agreement", data.nondisclosure_agreement.IsNull() ? "" : data.nondisclosure_agreement},
                        {"_registration_checklist", data.registration_checklist.IsNull() ? "" :  data.registration_checklist},
                        {"_access_form", data.access_form.IsNull() ? "" : data.access_form},
                        {"_technical_requirements", data.technical_requirements.IsNull() ? "" : data.technical_requirements},
                        {"_api_document", data.api_document.IsNull() ? "" : data.api_document},
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
            int registrationResult = Connection.Execute(StoredProcedures.PARTNERS_REGISTRATION, RegistrationParam, Transaction, 60, CommandType.StoredProcedure);
            if (registrationResult < 1)
            {
                _Logger.Error(string.Format("Partners Registration Failed: {0}", data.Serialize()));
                Transaction.Rollback();
                return new Response { ResponseCode = 400, ResponsMessage = "Unable to process request. Please try again later." };
            }
            Transaction.Commit();

            #region SMS Notification
            //
            //send sms notification to partner regarding the request
            //
            List<dynamic> FSD_contact = Connection.Query<dynamic>(StoredProcedures.GET_APPROVERS_CONTACT, new { _type = 0 }, null, false, 40, CommandType.StoredProcedure).ToList();

            Thread executePartnerSMS = new Thread(delegate ()
            {

                SMSHandler.Instance.SmsNotification(data.contact_number,
                    string.Format("Hi {0} {1} {2} {3} {4} {5} {6} {7} {8}", data.business_name,
                                    "Thank you for registering as MLhuillier partner!",
                                    "We have received your application and will be submitted to our Financial Services Division for approval.",
                                    "As application is on process, you will be notified thru your email and contact number.",
                                    "Please visit " + Models.Email.Link + " to access our API for your test reference.",
                                    "For inquiries, please e - mail us at ", FSD_contact[0].email, "and", FSD_contact[0].tg_email));
            })
            {
                IsBackground = true
            };
            executePartnerSMS.Start();
            //
            //send sms notification to FSD regarding the request
            //
            Thread executeDivisionSMS = new Thread(delegate ()
            {
                SMSHandler.Instance.SmsNotification(FSD_contact[0].contact_number,
                                             string.Format("A new partner {0} {1} {2} {3} {4}", data.business_name,
                                                           "has been registered and waiting for your approval.",
                                                           "Please visit URL", Models.Email.Link, "to administer the partner registration request."));
            })
            {
                IsBackground = true
            };
            executeDivisionSMS.Start();
            #endregion

            #region email notification
            //
            //Send email notification to partner as a confirmation that the request was successful
            //
            string transdate = Connection.Query<string>(StoredProcedures.GET_DATE, new { _type = 0, _username = string.Empty }, null, false, 40, CommandType.StoredProcedure).FirstOrDefault();
            var mBody = new StringBuilder();
            mBody.Append("<html><head></head><body><br/><font face='arial' align='left' size='2px'><p class='normal' style='text-indent: 0px;text-align:left'>" +
               "Hi " + data.business_name + "," +
               "<br/><br/>Registration Date and Time: " + transdate +
               "<br/>Registered Business Name:" + data.business_name +
               "<br/><br/><br/>Thank you for registering as MLhuillier partner!" +
               "<br/><br/>We have received your application and will be submitted to our Financial Services Division for approval." +
               "<br/You can access our API for your test reference by clicking the URL below:<b/>" + Models.Email.Link +
               "<b/><b/>As application is on process, you will be notified thru your email " + data.email + " and contact number " + data.contact_number + "." +
               "<b/><b/>For inquiries, please e-mail us at " + FSD_contact[0].email + " and " + FSD_contact[0].tg_email + "." +
               "<b/><b/>Please ensure that your User ID and password are CONFIDENTIAL at all times." +
               "<b/><b/><b/>At your service," +
               "M Lhuillier Financial Service, Inc.</p></font></p></font></body></html>");
            Thread executePartnerEmail = new Thread(delegate ()
            {
                EmailHandler.Instance.SendEmail(data.email, mBody.ToString());
            })
            {
                IsBackground = true
            };
            executePartnerEmail.Start();
            #endregion


            _Logger.Info(string.Format("Partners Registration successfull: {0}", data.Serialize()));
            return new Response { ResponseCode = 200, ResponsMessage = "Successfully submitted to FSD and wait for verification." };
        }
        catch (MySqlException mex)
        {
            Transaction.Rollback();
            _Logger.Error(string.Format("Partners Registration Transaction Failed: {0}", Model.Serialize()));
            _Logger.Fatal(mex.ToString());
            return new Response { ResponseCode = 400, ResponsMessage = "Unable to process request. Please check your data and try again." };
        }
        catch (TimeoutException tex)
        {
            Transaction.Rollback();
            _Logger.Error(string.Format("Partners Registration Transaction Failed: {0}", Model.Serialize()));
            _Logger.Fatal(tex.ToString());
            return new Response { ResponseCode = 408, ResponsMessage = "Unable to process request. Connection timeout occured. Please try again later." };
        }
        catch (Exception ex)
        {
            _Logger.Fatal(ex.ToString());
            Transaction.Rollback();
            _Logger.Error(string.Format("Partners Registration Transaction Failed: {0}", Model.Serialize()));
            return new Response { ResponseCode = 500, ResponsMessage = "Unable to process request. The system encountered some technical problem. Sorry for the inconvenience." };
        }
    }
}