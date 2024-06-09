using System.Configuration;
using System.Data.SqlClient;

namespace DAL
{
    public class DbConnect
    {
        public static string connstr = ConfigurationManager.ConnectionStrings["ChatApp"].ToString();
        public static SqlConnection _conn = new SqlConnection(connstr);
    }
}
