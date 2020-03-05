
using log4net;
using MySql.Data.MySqlClient;



/// <summary>
/// Summary description for ApproverProcess
/// </summary>
public class AdminProcess : Process
{    
    public AdminProcess()
    {
    }
    //Admin Login
    //Approve Partner
    //Approvers Update
    //approver registration   
    //Approvers List
    //DIvision List
    public override IResponse IntegrationTransaction(MySqlConnection Connection, RequestType RType, IModel Model, MySqlTransaction Transaction)
    {
        switch (RType)
        {
            #region Admin Login
            case RequestType.AdminLogin:
                var processLogin = new AdminLogin();
                return processLogin.Login(Connection, Model);
            #endregion

            #region  Approvers Update
            case RequestType.ApproversUpdate:
                var processUpdate = new ApproversUpdate();
                return processUpdate.Update(Connection, Model, Transaction);
            #endregion

            #region Approvers Registration

            case RequestType.ApproverRegistration:
                var processRegistration = new ApproversRegistration();
                return processRegistration.Registration(Connection, Model, Transaction);
            #endregion

            #region Approve Partner
            case RequestType.ApprovePartner:
                var processApprove = new ApprovePartner();
                return processApprove.Approve(Connection,Model,Transaction);
            #endregion

            #region Approvers List
            case RequestType.ApproversList:
                var processApproversList = new ApproversList();
                return processApproversList.ApproverList(Connection,Model);
            #endregion

            #region Division List
            case RequestType.DivisionList:
                var processDivisionList = new ApproverDivisionList();
                return processDivisionList.DivisionList(Connection);
            #endregion

            #region Level List
            case RequestType.LevelList:
                var processLevelList = new ApproverLevelList();
                return processLevelList.LevelList(Connection);
            #endregion

            default:
                return new Response { ResponseCode = 404, ResponsMessage = "Unauthorized! Invalid method." };
        }
    }

}