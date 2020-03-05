using Dapper;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

/// <summary>
/// Summary description for PartnersList
/// </summary>
public class PartnersList
{
    private readonly ILog _Logger = LogManager.GetLogger(typeof(PartnersList));
    public PartnersList()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public virtual PartnersListResponse PartnerList(MySqlConnection Connection, IModel Model) 
    {
        try
        {
            Models.Approver data = (Models.Approver)Model;
            List<Models.PartnersData> partnersList = Connection.Query<Models.PartnersData>(StoredProcedures.PARTNERS_LIST, new { _division = data.division }, null, false, 60, CommandType.StoredProcedure).ToList();
            if (partnersList.Count < 1)
            {
                _Logger.Info("No data found!");
                return new PartnersListResponse { ResponseCode = 404, ResponsMessage = "No data found." };
            }

            return new PartnersListResponse { ResponseCode = 200, ResponsMessage = "Success", partnersList = partnersList };
        }
        catch (MySqlException mex)
        {
            _Logger.Fatal(mex.ToString());
            return new PartnersListResponse { ResponseCode = 400, ResponsMessage = "Unable to process request. Please check your data and try again." };
        }
        catch (TimeoutException tex)
        {
            _Logger.Fatal(tex.ToString());
            return new PartnersListResponse { ResponseCode = 408, ResponsMessage = "Unable to process request. Connection timeout occured. Please try again later." };
        }
        catch (Exception ex)
        {
            _Logger.Fatal(ex.ToString());
            return new PartnersListResponse { ResponseCode = 500, ResponsMessage = "Unable to process request. The system encountered some technical problem. Sorry for the inconvenience." };
        }
    }

}