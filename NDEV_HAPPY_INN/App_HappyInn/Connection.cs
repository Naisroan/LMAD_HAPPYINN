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

    private ISession Session;
    private IMapper Mapper;

    public IMapper GetMapper() => Mapper;

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

            Connect();
            Configure();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private void Connect()
    {
        Cluster cluster = Cluster.Builder().AddContactPoint(ClusterName).Build();
        Session = cluster.Connect(KeySpaceName);
        Mapper = new Mapper(Session);
    }

    private void Configure()
    {
        Session.UserDefinedTypes.Define(UdtMap.For<Ciudad>());
    }
}