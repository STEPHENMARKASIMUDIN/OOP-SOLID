
using System.ServiceModel;
using System.ServiceModel.Web;


// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService" in both code and config file together.
[ServiceContract]
public interface IPartnerIntegrationService
{
    [OperationContract]
    [WebInvoke(Method = "GET",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "CheckConnection")]
    Response CheckConnection();
    [OperationContract]
    [WebInvoke(Method="POST",
        RequestFormat=WebMessageFormat.Json,
        ResponseFormat=WebMessageFormat.Json,
        BodyStyle=WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "PartnersLogin")]
    LoginResponse PartnersLogin(Models.Login data);

    [OperationContract]
    [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "PartnersRegistration")]
    Response PartnersRegistration(Models.PartnersData data);

    [OperationContract]
    [WebInvoke(Method="POST",
        RequestFormat=WebMessageFormat.Json,
        ResponseFormat=WebMessageFormat.Json,
        BodyStyle=WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "PartnersUpdate")]
    Response PartnersUpdate(Models.PartnersData data);

    [OperationContract]
    [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "AdminLogin")]
    LoginResponse AdminLogin(Models.Login data);
    
    [OperationContract]
    [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "ApproverRegistration")]
    Response ApproverRegistration(Models.Admin data);

    [OperationContract]
    [WebInvoke(Method = "GET",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "ListOfApprover")]
    ListOfApproverResponse ListOfApprover();
    [OperationContract]
    [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "ApproversUpdate")]
    Response ApproversUpdate(Models.Admin data);

    [OperationContract]
    [WebInvoke(Method = "GET",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "DivisionList")]
    DivisionListResponse DivisionList();
    [OperationContract]
    [WebInvoke(Method = "GET",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "LevelList")]
    LevelListResponse LevelList();
    [OperationContract]
    [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "PartnersList")]
    PartnersListResponse PartnersList(Models.Approver data);
    [OperationContract]
    [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "ApprovePartner")]
    Response ApprovePartner(Models.Approver data);
}

