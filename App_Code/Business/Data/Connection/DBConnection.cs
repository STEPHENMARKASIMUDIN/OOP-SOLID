using MySql.Data.MySqlClient;
using System;
using System.Data;

/// <summary>
/// Summary description for DBConnection
/// </summary>
public class DBConnection
{
    private IConnection _Connection;
    private static DBConnection _DBInstance = null;
    private static readonly object _InstanceLock = new object();
    private readonly log4net.ILog _Logger = log4net.LogManager.GetLogger(typeof(DBConnection));
    public static DBConnection Instance { get {
            if (_DBInstance == null)
            {
                lock (_InstanceLock)
                {
                    if (_DBInstance == null)
                    {
                        _DBInstance = new DBConnection(new ConnectionString());
                    }
                }
            }
            return _DBInstance;
        } }
    public DBConnection(IConnection Connection)
    {
        _Connection = Connection;
    }

   
    protected internal IResponse DBConnect(Process Process, IModel Model, MethodType MType, RequestType RType)
    {
        try
        {
            switch (MType)
            {
                case MethodType.GET:
                    using (MySqlConnection connection = new MySqlConnection(_Connection.GetConnectionString()))
                    {
                        connection.Open();
                        return Process.IntegrationTransaction(connection, RType,Model);

                    }
                case MethodType.POST:
                    using (MySqlConnection connection = new MySqlConnection(_Connection.GetConnectionString()))
                    {
                        connection.Open();
                        using (MySqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                        {
                            return Process.IntegrationTransaction(connection, RType, Model, transaction);
                        }
                    }
                default:
                    using (MySqlConnection connection = new MySqlConnection(_Connection.GetConnectionString()))
                    {
                        connection.Open();
                        return Process.IntegrationTransaction(connection,RType);

                    }
            }
        }
        catch (Exception ex)
        {
            _Logger.Fatal(ex.ToString());
            throw new Exception("Unable to connect to the server!");
        }
    }


    protected internal IResponse DBConnect(Process Process, RequestType Type)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_Connection.GetConnectionString()))
            {
                connection.Open();
                return Process.IntegrationTransaction(connection, Type);
            }
        }
        catch (Exception ex)
        {
            _Logger.Fatal(ex.ToString());
            throw new Exception("Unable to connect to the server!");
        }
    }

    protected internal IResponse CheckConnection()
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_Connection.GetConnectionString()))
            {
                connection.Open();
                
            }
            return new Response { ResponseCode = 200, ResponsMessage = "Connection Successful" };
        }
        catch (Exception ex)
        {
            _Logger.Fatal(ex.ToString());
            return new Response { ResponseCode = 500, ResponsMessage = "Unable to connect to Credit Memo DB" };
        }
    }
}