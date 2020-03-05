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
/// Summary description for ApprovePartner
/// </summary>
public class ApprovePartner
{
    private readonly ILog _Logger = LogManager.GetLogger(typeof(ApprovePartner));
    public ApprovePartner()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public virtual Response Approve(MySqlConnection Connection, IModel Model, MySqlTransaction Transaction) 
    {
        try
        {
            //_username = partners username
            Models.Approver data = (Models.Approver)Model;
            Dictionary<string, object> isApprovePartnerParam = new Dictionary<string, object>();

            int type = 0;

            #region Disapprove Partner
            if (data.isApproved.Equals(2))
            {
                isApprovePartnerParam.Add("_username", data.username);
                isApprovePartnerParam.Add("_approver", data.approver);
                isApprovePartnerParam.Add("_remarks", data.remarks.IsNull() ? "" : data.remarks);
                isApprovePartnerParam.Add("_type", type);
                isApprovePartnerParam.Add("_isApproved", data.isApproved);

                int disApprovalResult = Connection.Execute(StoredProcedures.APPROVE_PARTNER, isApprovePartnerParam, Transaction, 60, CommandType.StoredProcedure);
                if (disApprovalResult < 1)
                {
                    _Logger.Error(string.Format("Disapprove Partner Failed: {0}", data.Serialize()));
                    Transaction.Rollback();
                    return new Response { ResponseCode = 400, ResponsMessage = "Unable to process request. Please try again later." };
                }
                Transaction.Commit();
                #region SMS Notification
                //
                //send sms notification to partner regarding the request
                //
                List<dynamic> approvers_contact = Connection.Query<dynamic>(StoredProcedures.GET_APPROVERS_CONTACT, new { _type = 0 }, null, false, 40, CommandType.StoredProcedure).ToList();

                Thread executePartnerSMS = new Thread(delegate ()
                {

                    SMSHandler.Instance.SmsNotification(data.contact_number,
                        string.Format("Hi {0} {1} {2} {3} {4} {5} {6} {7} {8}", data.business_name,
                                        "We regret to inform you that your recent application for being MLhuillier partner has been denied.",
                                        "After a thorough review of your application and the supporting documents you supplied,",
                                        "we have determined that it would not be possible to accommodate your request at this time.",
                                        "Your application was denied because of the following reason(s):", data.remarks,
                                        "For inquiries, please e - mail us at ", approvers_contact[0].email, "and", approvers_contact[0].tg_email));
                })
                {
                    IsBackground = true
                };
                executePartnerSMS.Start();
                #endregion

                #region email notification
                //
                //Send email notification to partner as a confirmation that the request was denied
                //
                string transdate = Connection.Query<string>(StoredProcedures.GET_DATE, new { _type = 11, _username = data.username }, null, false, 40, CommandType.StoredProcedure).FirstOrDefault();
                StringBuilder mBody = new StringBuilder();
                mBody.Append("<html><head></head><body><br/><font face='arial' align='left' size='2px'><p class='normal' style='text-indent: 0px;text-align:left'>" +
                   "Hi " + data.business_name + "," +
                   "<br/><br/>Registration Date and Time: " + transdate +
                   "<br/>Registered Business Name:" + data.business_name +
                   "<br/><br/><br/>Thank you for registering as MLhuillier partner!" +
                   "<br/><br/>We regret to inform you that your recent application for being MLhuillier partner has been denied." +
                   "<br/After a thorough review of your application and the supporting documents you supplied, we have" +
                   "<b/>determined that it would not be possible to accommodate your request at this time." +
                   "<b/><b/>Your application was denied because of the following reason(s):<br/>" + data.remarks +
                   "<b/><b/>For inquiries, please e-mail us at " + approvers_contact[0].email + " and " + approvers_contact[0].tg_email + "." +
                   "<b/><b/>Thank you again for your interest and for applying" +
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

                _Logger.Info(string.Format("Disapprove Partner successfull: {0}", data.Serialize()));
                return new Response { ResponseCode = 200, ResponsMessage = "Partner was successfully disapproved." };
            }
            #endregion

            #region Approve Partner
            if (data.division.Equals(DivisionType.FSD))
            {
                int checkFSDLevel = Connection.Query<int>(StoredProcedures.CHECK_FSD_LEVEL, new { _username = data.username }, null, false, 40, CommandType.StoredProcedure).FirstOrDefault();
                if (checkFSDLevel == 0)
                {

                    type = 1;
                    #region SMS Notification
                    //
                    //send sms notification to partner regarding the request
                    //
                    List<dynamic> FSD_contact = Connection.Query<dynamic>(StoredProcedures.GET_APPROVERS_CONTACT, new { _type = 0 }, null, false, 40, CommandType.StoredProcedure).ToList();

                    Thread executePartnerSMS = new Thread(delegate ()
                    {
                        SMSHandler.Instance.SmsNotification(data.contact_number,
                            string.Format("Hi {0}, {1} {2} {3} {4} {5} {6} {7} {8}", data.business_name,
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
                    string transdate = Connection.Query<string>(StoredProcedures.GET_DATE, new { _type = 11, _username = data.username }, null, false, 40, CommandType.StoredProcedure).FirstOrDefault();
                    StringBuilder mBody = new StringBuilder();
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

                }
                else
                {
                    type = 4;
                    #region SMS Notification
                    //
                    //send sms notification to partner regarding the request
                    //
                    List<dynamic> FSD_contact = Connection.Query<dynamic>(StoredProcedures.GET_APPROVERS_CONTACT, new { _type = 0 }, null, false, 40, CommandType.StoredProcedure).ToList();

                    Thread executePartnerSMS = new Thread(delegate ()
                    {
                        SMSHandler.Instance.SmsNotification(data.contact_number,
                            string.Format("Hi {0}, {1} {2} {3} {4} {5} {6} {7} {8} {9}", data.business_name, "Congratulations!",
                                            "Your registration as MLhuillier partner is now APPROVED from our company president Sir Michael A. Lhuillier.",
                                            "It is now submitted to our Financial Services Division for checking of documents.",
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
                    StringBuilder mBody = new StringBuilder();
                    mBody.Append("<html><head></head><body><br/><font face='arial' align='left' size='2px'><p class='normal' style='text-indent: 0px;text-align:left'>" +
                       "Hi " + data.business_name + "," +
                       "<br/><br/>Congratulations!" +
                       "<br/><br/>Your registration as MLhuillier partner is now APPROVED from our company president Sir Michael A. Lhuillier." +
                       "<br/><br/>It is now submitted to our Financial Services Division for checking of documents." +
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
                }
            }
            else if (data.division.Equals(DivisionType.SECCOM))
            {
                type = 2;
                #region SMS Notification
                //
                //send sms notification to partner regarding the request
                //
                List<dynamic> FSD_contact = Connection.Query<dynamic>(StoredProcedures.GET_APPROVERS_CONTACT, new { _type = 0 }, null, false, 40, CommandType.StoredProcedure).ToList();

                Thread executePartnerSMS = new Thread(delegate ()
                {
                    SMSHandler.Instance.SmsNotification(data.contact_number,
                        string.Format("Hi {0}, {1} {2} {3} {4} {5} {6} {7} {8} {9}", data.business_name, "Congratulations!",
                                        "Your registration as MLhuillier partner is now APPROVED from our Financial Services Division.",
                                        "It is now submitted to our Security and Compliance (SECCOM) Division for approval.",
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
                StringBuilder mBody = new StringBuilder();
                mBody.Append("<html><head></head><body><br/><font face='arial' align='left' size='2px'><p class='normal' style='text-indent: 0px;text-align:left'>" +
                   "Hi " + data.business_name + "," +
                   "<br/><br/>Congratulations!" +
                   "<br/><br/>Your registration as MLhuillier partner is now APPROVED from our Financial Services Division." +
                   "<br/><br/>It is now submitted to our Security and Compliance (SECCOM) Division for approval." +
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

            }
            else if (data.division.Equals(DivisionType.CEO))
            {
                type = 3;
                #region SMS Notification
                //
                //send sms notification to partner regarding the request
                //
                List<dynamic> FSD_contact = Connection.Query<dynamic>(StoredProcedures.GET_APPROVERS_CONTACT, new { _type = 0 }, null, false, 40, CommandType.StoredProcedure).ToList();

                Thread executePartnerSMS = new Thread(delegate ()
                {
                    SMSHandler.Instance.SmsNotification(data.contact_number,
                        string.Format("Hi {0}, {1} {2} {3} {4} {5} {6} {7} {8} {9}", data.business_name, "Congratulations!",
                                        "Your registration as MLhuillier partner is now APPROVED from our Security and Compliance (SECCOM) Division.",
                                        "It is now submitted to our company president Sir Michael A. Lhuillier for approval.",
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

                StringBuilder mBody = new StringBuilder();
                mBody.Append("<html><head></head><body><br/><font face='arial' align='left' size='2px'><p class='normal' style='text-indent: 0px;text-align:left'>" +
                   "Hi " + data.business_name + "," +
                   "<br/><br/>Congratulations!" +
                   "<br/><br/>Your registration as MLhuillier partner is now APPROVED from our Security and Compliance (SECCOM) Division." +
                   "<br/><br/>It is now submitted to our company president Sir Michael A. Lhuillier for approval." +
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
            }
            else if (data.division.Equals(DivisionType.CAD))
            {
                type = 5;
                #region SMS Notification
                //
                //send sms notification to partner regarding the request
                //
                List<dynamic> FSD_contact = Connection.Query<dynamic>(StoredProcedures.GET_APPROVERS_CONTACT, new { _type = 0 }, null, false, 40, CommandType.StoredProcedure).ToList();

                Thread executePartnerSMS = new Thread(delegate ()
                {
                    SMSHandler.Instance.SmsNotification(data.contact_number,
                        string.Format("Hi {0}, {1} {2} {3} {4} {5} {6} {7} {8} {9}", data.business_name, "Congratulations!",
                                        "Your registration as MLhuillier partner is now APPROVED from our Financial Services Division.",
                                        "It is now submitted to our Central Accounting Division for approval.",
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

                StringBuilder mBody = new StringBuilder();
                mBody.Append("<html><head></head><body><br/><font face='arial' align='left' size='2px'><p class='normal' style='text-indent: 0px;text-align:left'>" +
                   "Hi " + data.business_name + "," +
                   "<br/><br/>Congratulations!" +
                   "<br/><br/>Your registration as MLhuillier partner is now APPROVED from our Financial Services Division." +
                   "<br/><br/>It is now submitted to our Central Accounting Division for approval." +
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
            }
            else if (data.division.Equals(DivisionType.TG_HELPDESK))
            {
                int checkHDLevel = Connection.Query<int>(StoredProcedures.CHECK_HELPDESK_LEVEL, new { _username = data.username }, null, false, 40, CommandType.StoredProcedure).FirstOrDefault();
                if (checkHDLevel == 0)
                {
                    type = 6;

                    #region SMS Notification
                    //
                    //send sms notification to partner regarding the request
                    //
                    List<dynamic> FSD_contact = Connection.Query<dynamic>(StoredProcedures.GET_APPROVERS_CONTACT, new { _type = 0 }, null, false, 40, CommandType.StoredProcedure).ToList();

                    Thread executePartnerSMS = new Thread(delegate ()
                    {
                        SMSHandler.Instance.SmsNotification(data.contact_number,
                            string.Format("Hi {0}, {1} {2} {3} {4} {5} {6} {7} {8} {9}", data.business_name, "Congratulations!",
                                            "Your registration as MLhuillier partner is now APPROVED from our Central Accounting Division.",
                                            "It is now submitted to our Helpdesk for checking of requirements and creation of request for integration.",
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

                    StringBuilder mBody = new StringBuilder();
                    mBody.Append("<html><head></head><body><br/><font face='arial' align='left' size='2px'><p class='normal' style='text-indent: 0px;text-align:left'>" +
                       "Hi " + data.business_name + "," +
                       "<br/><br/>Congratulations!" +
                       "<br/><br/>Your registration as MLhuillier partner is now APPROVED from our Central Accounting Division." +
                       "<br/><br/>It is now submitted to our Helpdesk for checking of requirements and creation of request for integration." +
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
                }
                else
                {
                    type = 8;
                    #region SMS Notification
                    //
                    //send sms notification to partner regarding the request
                    //
                    List<dynamic> FSD_contact = Connection.Query<dynamic>(StoredProcedures.GET_APPROVERS_CONTACT, new { _type = 0 }, null, false, 40, CommandType.StoredProcedure).ToList();

                    Thread executePartnerSMS = new Thread(delegate ()
                    {
                        SMSHandler.Instance.SmsNotification(data.contact_number,
                            string.Format("Hi {0}, {1} {2} {3} {4} {5} {6} {7} {8} {9}", data.business_name, "Congratulations!",
                                            "Your registration as MLhuillier partner is now APPROVED from our Tech Group Assistant CTO.",
                                            "It is now submitted to our Helpdesk for submission of request for integration development.",
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

                    StringBuilder mBody = new StringBuilder();
                    mBody.Append("<html><head></head><body><br/><font face='arial' align='left' size='2px'><p class='normal' style='text-indent: 0px;text-align:left'>" +
                       "Hi " + data.business_name + "," +
                       "<br/><br/>Congratulations!" +
                       "<br/><br/>Your registration as MLhuillier partner is now APPROVED from our Tech Group Assistant CTO." +
                       "<br/><br/>It is now submitted to our Helpdesk for submission of request for integration development." +
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
                }
            }
            else if (data.division.Equals(DivisionType.TG_ASST_CTO))
            {
                type = 7;
                #region SMS Notification
                //
                //send sms notification to partner regarding the request
                //
                List<dynamic> FSD_contact = Connection.Query<dynamic>(StoredProcedures.GET_APPROVERS_CONTACT, new { _type = 0 }, null, false, 40, CommandType.StoredProcedure).ToList();

                Thread executePartnerSMS = new Thread(delegate ()
                {
                    SMSHandler.Instance.SmsNotification(data.contact_number,
                        string.Format("Hi {0}, {1} {2} {3} {4} {5} {6} {7} {8} {9}", data.business_name, "Congratulations!",
                                        "Your registration as MLhuillier partner is now APPROVED from our Helpdesk.",
                                        "It is now submitted to our Tech Group Assistant CTO for approval of request for integration.",
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

                StringBuilder mBody = new StringBuilder();
                mBody.Append("<html><head></head><body><br/><font face='arial' align='left' size='2px'><p class='normal' style='text-indent: 0px;text-align:left'>" +
                   "Hi " + data.business_name + "," +
                   "<br/><br/>Congratulations!" +
                   "<br/><br/>Your registration as MLhuillier partner is now APPROVED from our Helpdesk." +
                   "<br/><br/>It is now submitted to our Tech Group Assistant CTO for approval of request for integration." +
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
            }
            else if (data.division.Equals(DivisionType.TG_PRO))
            {
                type = 9;
                #region SMS Notification
                //
                //send sms notification to partner regarding the request
                //
                List<dynamic> FSD_contact = Connection.Query<dynamic>(StoredProcedures.GET_APPROVERS_CONTACT, new { _type = 0 }, null, false, 40, CommandType.StoredProcedure).ToList();

                Thread executePartnerSMS = new Thread(delegate ()
                {
                    SMSHandler.Instance.SmsNotification(data.contact_number,
                        string.Format("Hi {0}, {1} {2} {3} {4} {5} {6} {7} {8} {9}", data.business_name, "Congratulations!",
                                        "Your registration as MLhuillier partner is now APPROVED from our Helpdesk.",
                                        "It is now submitted to our Tech Group Partners Relation Office for Hi/Hello meeting.",
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

                StringBuilder mBody = new StringBuilder();
                mBody.Append("<html><head></head><body><br/><font face='arial' align='left' size='2px'><p class='normal' style='text-indent: 0px;text-align:left'>" +
                   "Hi " + data.business_name + "," +
                   "<br/><br/>Congratulations!" +
                   "<br/><br/>Your registration as MLhuillier partner is now APPROVED from our Helpdesk." +
                   "<br/><br/>It is now submitted to our Tech Group Partners Relation Office for Hi/Hello meeting." +
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
            }
            else if (data.division.Equals(DivisionType.TG_PMO))
            {
                type = 10;
                #region SMS Notification
                //
                //send sms notification to partner regarding the request
                //
                List<dynamic> FSD_contact = Connection.Query<dynamic>(StoredProcedures.GET_APPROVERS_CONTACT, new { _type = 0 }, null, false, 40, CommandType.StoredProcedure).ToList();

                Thread executePartnerSMS = new Thread(delegate ()
                {
                    SMSHandler.Instance.SmsNotification(data.contact_number,
                        string.Format("Hi {0}, {1} {2} {3} {4} {5} {6} {7} {8} {9}", data.business_name, "Congratulations!",
                                        "Your registration as MLhuillier partner is now APPROVED from Tech Group Partners Relation Office .",
                                        "It is now submitted to our Tech Group Project Management Office for timeline and integration.",
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

                StringBuilder mBody = new StringBuilder();
                mBody.Append("<html><head></head><body><br/><font face='arial' align='left' size='2px'><p class='normal' style='text-indent: 0px;text-align:left'>" +
                   "Hi " + data.business_name + "," +
                   "<br/><br/>Congratulations!" +
                   "<br/><br/>Your registration as MLhuillier partner is now APPROVED from Tech Group Partners Relation Office ." +
                   "<br/><br/>It is now submitted to our Tech Group Project Management Office for timeline and integration." +
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
            }

            isApprovePartnerParam.Add("_username", data.username);
            isApprovePartnerParam.Add("_approver", data.approver);
            isApprovePartnerParam.Add("_remarks", data.remarks.IsNull() ? "" : data.remarks);
            isApprovePartnerParam.Add("_isApproved", data.isApproved);
            isApprovePartnerParam.Add("_type", type);
            isApprovePartnerParam.Add("_registration_checklist", data.registration_checklist.IsNull() ? "" : data.registration_checklist);
            isApprovePartnerParam.Add("_technical_requirements", data.technical_requirements.IsNull() ? "" : data.technical_requirements);
            isApprovePartnerParam.Add("_wo_attachment", data.wo_attachment.IsNull() ? "" : data.wo_attachment);

            int approvalResult = Connection.Execute(StoredProcedures.APPROVE_PARTNER, isApprovePartnerParam, Transaction, 60, CommandType.StoredProcedure);
            if (approvalResult < 1)
            {
                _Logger.Error(string.Format("Approve Partner Failed: {0}", data.Serialize()));
                Transaction.Rollback();
                return new Response { ResponseCode = 400, ResponsMessage = "Unable to process request. Please try again later." };
            }
            #endregion


            Transaction.Commit();
            _Logger.Info(string.Format("Approve Partner successfull: {0}", data.Serialize()));
            return new Response { ResponseCode = 200, ResponsMessage = "Partner was successfully approved. Thank you." };
        }
        catch (MySqlException mex)
        {
            Transaction.Rollback();
            _Logger.Error(string.Format("Approve Partner Transaction Failed: {0}", Model.Serialize()));
            _Logger.Fatal(mex.ToString());
            return new Response { ResponseCode = 400, ResponsMessage = "Unable to process request. Please check your data and try again." };
        }
        catch (TimeoutException tex)
        {
            Transaction.Rollback();
            _Logger.Error(string.Format("Approve Partner Transaction Failed: {0}", Model.Serialize()));
            _Logger.Fatal(tex.ToString());
            return new Response { ResponseCode = 408, ResponsMessage = "Unable to process request. Connection timeout occured. Please try again later." };
        }
        catch (Exception ex)
        {
            Transaction.Rollback();
            _Logger.Error(string.Format("Approve Partner Transaction Failed: {0}", Model.Serialize()));
            _Logger.Fatal(ex.ToString());
            return new Response { ResponseCode = 500, ResponsMessage = "Unable to process request. The system encountered some technical problem. Sorry for the inconvenience." };
        }
    }
}