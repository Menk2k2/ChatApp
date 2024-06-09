using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNet.SignalR.Client;
using DAL;
using System.Data.SqlClient;

namespace GUI
{
    public static class Connection
    {
        public static HubConnection connection;
        public static IHubProxy chatHub;
        public static Microsoft.AspNet.SignalR.Client.ConnectionState _connectionState = Microsoft.AspNet.SignalR.Client.ConnectionState.Disconnected;
        public static void Connect()
        {
            // connect đến server
            connection = new HubConnection("https://localhost:44332/"); // địa chỉ server
            chatHub = connection.CreateHubProxy("chatHub");

            try
            {
                // Mở kết nối đến chathub trên server
                connection.Start().Wait();

                // lấy ID của kết nối
                string connectionId = connection.ConnectionId;

                // update Connection_Id
                //if (DbConnect._conn.State != System.Data.ConnectionState.Open) DbConnect._conn.Open();
                //SqlCommand cmd = new SqlCommand();
                //cmd.Connection = DbConnect._conn;
                //cmd.CommandText = string.Format("UPDATE tblEmployee SET Connection_Id = '{0}' where Email = '{1}'", connectionId, Const.Email);
                //var result = cmd.ExecuteNonQuery();
                //DbConnect._conn.Close();
                using (var conn = new SqlConnection(DbConnect.connstr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = string.Format("UPDATE tblEmployee SET Connection_Id = '{0}' WHERE Email = '{1}'", connectionId, Const.Email);
                    var result = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi trong quá trình kết nới tới web server. Message: " + ex.Message);
            }
        }
    }
}
