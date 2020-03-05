using log4net.Config;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public sealed class PartnerIntegrationService : IPartnerIntegrationService
{
     
     public PartnerIntegrationService()
    {
        XmlConfigurator.Configure();           
    }
  
   
    public Response CheckConnection()
    {
        Response result = (Response)DBConnection.Instance.CheckConnection();
        return new Response { ResponseCode = result.ResponseCode, ResponsMessage = result.ResponsMessage };
    }
    public LoginResponse PartnersLogin(Models.Login data)
    {
        LoginResponse result = (LoginResponse)DBConnection.Instance.DBConnect(new PartnerProcess(), data, MethodType.GET, RequestType.PartnersLogin);
        return new LoginResponse { ResponseCode = result.ResponseCode, ResponsMessage = result.ResponsMessage, loginData = result.loginData };
    }
    public Response PartnersRegistration(Models.PartnersData data)
    {
        Response result = (Response)DBConnection.Instance.DBConnect(new PartnerProcess(), data, MethodType.POST, RequestType.PartnersRegistration);
        return new Response { ResponseCode = result.ResponseCode, ResponsMessage = result.ResponsMessage };
    }
    public Response PartnersUpdate(Models.PartnersData data)
    {
        Response result = (Response)DBConnection.Instance.DBConnect(new PartnerProcess(), data, MethodType.POST, RequestType.PartnersUpdate);
        return new Response { ResponseCode = result.ResponseCode, ResponsMessage = result.ResponsMessage };
    }
    public LoginResponse AdminLogin(Models.Login data)
    {
        LoginResponse result = (LoginResponse)DBConnection.Instance.DBConnect(new AdminProcess(), data, MethodType.GET, RequestType.AdminLogin);
        return new LoginResponse { ResponseCode = result.ResponseCode, ResponsMessage = result.ResponsMessage, adminData = result.adminData };
    }
    public Response ApproverRegistration(Models.Admin data)
    {
        Response result = (Response)DBConnection.Instance.DBConnect(new AdminProcess(), data, MethodType.POST, RequestType.ApproverRegistration);
        return new Response { ResponseCode = result.ResponseCode, ResponsMessage = result.ResponsMessage };
    }
    public ListOfApproverResponse ListOfApprover()
    {
        ListOfApproverResponse result = (ListOfApproverResponse)DBConnection.Instance.DBConnect(new AdminProcess(), RequestType.ApproversList);
        return new ListOfApproverResponse { ResponseCode = result.ResponseCode, ResponsMessage = result.ResponsMessage, adminList = result.adminList };
    }
    public Response ApproversUpdate(Models.Admin data)
    {
        Response result = (Response)DBConnection.Instance.DBConnect(new AdminProcess(), data, MethodType.POST, RequestType.ApproversUpdate);
        return new Response { ResponseCode = result.ResponseCode, ResponsMessage = result.ResponsMessage };
    }
    public DivisionListResponse DivisionList()
    {
        DivisionListResponse result = (DivisionListResponse)DBConnection.Instance.DBConnect(new AdminProcess(), RequestType.DivisionList);
        return new DivisionListResponse { ResponseCode = result.ResponseCode, ResponsMessage = result.ResponsMessage, divisionList = result.divisionList };
    }
    public PartnersListResponse PartnersList(Models.Approver data)
    {
        PartnersListResponse result = (PartnersListResponse)DBConnection.Instance.DBConnect(new PartnerProcess(), data,MethodType.GET, RequestType.PartnersList);
        return new PartnersListResponse { ResponseCode = result.ResponseCode, ResponsMessage = result.ResponsMessage, partnersList = result.partnersList };
    }
    public Response ApprovePartner(Models.Approver data)
    {
        Response result = (Response)DBConnection.Instance.DBConnect(new AdminProcess(), data, MethodType.POST, RequestType.ApprovePartner);
        return new Response { ResponseCode = result.ResponseCode, ResponsMessage = result.ResponsMessage };
    }

    public LevelListResponse LevelList()
    {
        LevelListResponse result = (LevelListResponse)DBConnection.Instance.DBConnect(new AdminProcess(), RequestType.LevelList);
        return new LevelListResponse { ResponseCode = result.ResponseCode, ResponsMessage = result.ResponsMessage, levelList = result.levelList };
    }
}
