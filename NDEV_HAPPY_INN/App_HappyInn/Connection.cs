using System;
using System.Configuration;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using NDEV_HAPPY_INN.Model;

public class Connection
{
    private const string APP_SETTING_NAME_CLUSTER = "Cluster";
    private const string APP_SETTING_NAME_KS = "KeySpace";

    private static string ClusterName;
    private static string KeySpaceName;

    private static Cluster Cluster;
    private static ISession Session;
    private static IMapper Mapper;

    public static IMapper GetMapper() => Mapper;

    public Connection()
    {
        try
        {
            if (string.IsNullOrEmpty(ClusterName))
            {
                ClusterName = ConfigurationManager.AppSettings[APP_SETTING_NAME_CLUSTER].ToString();
            }

            if (string.IsNullOrEmpty(KeySpaceName))
            {
                KeySpaceName = ConfigurationManager.AppSettings[APP_SETTING_NAME_KS].ToString();
            }

            if (Session == null)
            {
                Connect();
                Configure();
            }
        }
        catch (Exception ex)
        {
            throw new ConnectionCassandraException(ex.Message, ex);
        }
    }

    private void Connect()
    {
        try
        {
            Cluster = Cluster.Builder().AddContactPoint(ClusterName).Build();
            Session = Cluster.Connect(KeySpaceName);
            Mapper = new Mapper(Session);
        }
        catch (ConnectionCassandraException ex)
        {
            throw ex;
        }
    }

    private void Configure()
    {
        try
        {
            Session.UserDefinedTypes.Define(UdtMap.For<UDT_Ciudad>());
            // Session.UserDefinedTypes.Define(UdtMap.For<UDT_Habitacion>());
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public bool IsConnected()
    {
        return Session != null;
    }
}

public class ConnectionCassandraException : Exception
{
    public ConnectionCassandraException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}