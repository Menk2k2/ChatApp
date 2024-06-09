using DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    public static partial class Const
    {
        public static string UserName = "Người gửi";
        public static string Email = "";
        public static int ReceiverID = 0;
        public static int GroupID = 0;
        public static string MessTag = "single";
        public static int currentUserId = 0;

        public static int GetCurrentUserId()
        {
            try
            {
                if(DbConnect._conn.State != System.Data.ConnectionState.Open) DbConnect._conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = DbConnect._conn;
                cmd.CommandText = string.Format("SELECT Id FROM tblEmployee where Email = '{0}'", Const.Email);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    return id;
                }

                DbConnect._conn.Close();
            }
            catch (Exception)
            {

                throw;
            }

            return 0;
        }
    }
}
