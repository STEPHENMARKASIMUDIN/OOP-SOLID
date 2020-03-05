
using MySql.Data.MySqlClient;

/// <summary>
/// Summary description for Partner
/// </summary>
public class PartnerProcess : Process
{
    
    public PartnerProcess()
    {

    }


    //partners list
    //partners login    
    //partners registration        
    //Partners Update  
    public override IResponse IntegrationTransaction(MySqlConnection Connection, RequestType RType, IModel Model, MySqlTransaction Transaction)
    {
        switch (RType)
        {
            #region Partners Registration
            case RequestType.PartnersRegistration:
                var processRegistration = new PartnersRegistration();
                return processRegistration.Registration(Connection, Model, Transaction);
            #endregion

            #region Partners Update
            case RequestType.PartnersUpdate:
                var processPartnersUpdate = new PartnersUpdate();
                return processPartnersUpdate.Update(Connection, Model, Transaction);
            #endregion

            #region Partners Login
            case RequestType.PartnersLogin:
                var processPartnersLogin = new PartnersLogin();
                return processPartnersLogin.Login(Connection, Model);
            #endregion

            #region Partners List

            case RequestType.PartnersList:
                var processPartnerList = new PartnersList();
                return processPartnerList.PartnerList(Connection, Model);

            #endregion

            default:
                return new Response { ResponseCode = 404, ResponsMessage = "Unauthorized! Invalid method." };
        }
    }
}