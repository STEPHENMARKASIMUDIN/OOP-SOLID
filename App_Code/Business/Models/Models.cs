using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Model
/// </summary>
public abstract class Models : IModel
{
    public static string Path { get { return "C:\\kpconfig\\PartnersIntegration.ini"; } }

    public class SMS
    {
        public static string SMS_URL { get; set; }
        public string message { get; set; }
        public string celno { get; set; }
    }
   
    public class Email
    {
        public static string Host { get; set; }
        public static string EmailAdd { get; set; }
        public static string SenderUser { get; set; }
        public static string SenderPass { get; set; }
        public static string Port { get; set; }
        public static string Link { get; set; }

    }

    public class PartnersData :Login, IModel
    {
      
        public string business_name { get; set; }
        
        public string contact_person { get; set; }
        
        public string memorandom_agreement { get; set; }
        public string nondisclosure_agreement { get; set; }
        public string registration_checklist { get; set; }
        public string access_form { get; set; }
        public string technical_requirements { get; set; }
        public string others { get; set; }
        public string api_document { get; set; }
        public int status { get; set; }
        public string attachment_1 { get; set; }
        public string attachment_2 { get; set; }
        public string attachment_3 { get; set; }
        public string attachment_4 { get; set; }
        public string attachment_5 { get; set; }
        public string attachment_6 { get; set; }
        public string attachment_7 { get; set; }
        public string attachment_8 { get; set; }
        public string attachment_9 { get; set; }
        public string attachment_10 { get; set; }
        public string requested_at { get; set; }
        public string approved_date { get; set; }
        public string approver { get; set; }
        public string fsd_approval { get; set; }
        public string fsd_approver { get; set; }
        public string sec_approval { get; set; }
        public string sec_approver { get; set; }
        public string ceo_approval { get; set; }
        public string ceo_approver { get; set; }
        public string fsd2_approval { get; set; }
        public string fsd2_approver { get; set; }
        public string cad_approval { get; set; }
        public string cad_approver { get; set; }
        public string helpdesk_approval { get; set; }
        public string helpdesk_approver { get; set; }
        public string asst_cto_approval { get; set; }
        public string asst_cto_approver { get; set; }
        public string helpdesk2_approval { get; set; }
        public string helpdesk2_approver { get; set; }
        public string pro_approval { get; set; }
        public string pro_approver { get; set; }
        public string pmo_approval { get; set; }
        public string pmo_approver { get; set; }
        public string wo_attachment { get; set; }
        public string remarks { get; set; }
        
        public string updated_at { get; set; }
    }
    public class Login:IModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string contact_number { get; set; }
        public string email { get; set; }
        public string operator_id { get; set; }
        public int isActive { get; set; }        
        public int isApproved { get; set; }        
    }
    public class Admin : Login,IModel
    {
        public string id_number { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public string division { get; set; }
        public string level { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        
    }
    public class Division:IModel
    {
        public string division { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int isActive { get; set; }
    }
    public class Approver:Login,IModel
    {
        public string remarks { get; set; }
        public int status { get; set; }
        public string approver { get; set; }
        public string division { get; set; }
        public string business_name { get; set; }
        public string registration_checklist { get; set; }
        public string technical_requirements { get; set; }
        public string wo_attachment { get; set; }
    }

    public class Level:IModel
    {
        public string level { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int isActive { get; set; }
    }

    public class PartnersInfo:IModel
    {
        public string business_name { get; set; }

        public string contact_person { get; set; }

        public string memorandom_agreement { get; set; }
        public string nondisclosure_agreement { get; set; }
        public string registration_checklist { get; set; }
        public string access_form { get; set; }
        public string technical_requirements { get; set; }
        public string others { get; set; }
        public string api_document { get; set; }
        public int status { get; set; }
        public string attachment_1 { get; set; }
        public string attachment_2 { get; set; }
        public string attachment_3 { get; set; }
        public string attachment_4 { get; set; }
        public string attachment_5 { get; set; }
        public string attachment_6 { get; set; }
        public string attachment_7 { get; set; }
        public string attachment_8 { get; set; }
        public string attachment_9 { get; set; }
        public string attachment_10 { get; set; }
        public string requested_at { get; set; }
        public string approved_date { get; set; }
        public string approver { get; set; }
        public string fsd_approval { get; set; }
        public string fsd_approver { get; set; }
        public string sec_approval { get; set; }
        public string sec_approver { get; set; }
        public string ceo_approval { get; set; }
        public string ceo_approver { get; set; }
        public string fsd2_approval { get; set; }
        public string fsd2_approver { get; set; }
        public string cad_approval { get; set; }
        public string cad_approver { get; set; }
        public string helpdesk_approval { get; set; }
        public string helpdesk_approver { get; set; }
        public string asst_cto_approval { get; set; }
        public string asst_cto_approver { get; set; }
        public string helpdesk2_approval { get; set; }
        public string helpdesk2_approver { get; set; }
        public string pro_approval { get; set; }
        public string pro_approver { get; set; }
        public string pmo_approval { get; set; }
        public string pmo_approver { get; set; }
        public string wo_attachment { get; set; }
        public string remarks { get; set; }

        public string updated_at { get; set; }
    }
}
