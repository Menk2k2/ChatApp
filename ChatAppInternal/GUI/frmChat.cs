using BUS;
using DTO;
using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.AspNet.SignalR.Client;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using DAL;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using Guna.UI2.WinForms;

namespace GUI
{
    public partial class frmChat : Form
    {
        int yOffset = 10;

        public frmChat()
        {
            InitializeComponent();
        }

        //Hàm kết nối tới web server để gửi dữ liệu
        private void EstablishConnection()
        {
            // khi nhận tín hiệu từ server thông ua phương thức addMessageToPage thì hiển thị tin nhắn
            Connection.chatHub.On<string, string, int, int, int>("addMessageToPage", (senderName, message, ReceiverID, SenderId, GroupId) =>
            {
                yOffset = 10;
                // Hiển thị tin nhắn trong txtChatHistory
                if (((Const.ReceiverID == ReceiverID || ReceiverID == Const.currentUserId) && (SenderId == Const.ReceiverID || SenderId == Const.currentUserId)) || Const.GroupID == GroupId)
                {
                    if (panelMessage.InvokeRequired)
                    {
                        panelMessage.Invoke(new Action(() =>
                        {
                            //RenderMessageItem(message, SenderId, senderName);
                            LoadHistoryChat();
                        }));
                    }
                    else
                    {
                        //RenderMessageItem(message, SenderId, senderName);
                        LoadHistoryChat();
                    }
                }
            });
        }

        private void LoadHistoryChat()
        {
            panelMessage.Controls.Clear();

            if (Const.currentUserId != 0)
            {
                if (DbConnect._conn.State != System.Data.ConnectionState.Open) DbConnect._conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = DbConnect._conn;
                cmd.CommandText = string.Format(@"SELECT el.Name as nguoi_gui, el1.Name as nguoi_nhan, ms.Content, el.Id FROM Messages ms 
                    join tblEmployee el on el.Id = ms.SenderId
                    left join tblEmployee el1 on el1.Id = ms.ReceiverID
                    where (SenderId = '{0}' and ReceiverID = '{1}') or (ReceiverID = '{0}' and SenderId = '{1}') or ChannelId = {2}",
                    Const.currentUserId, Const.ReceiverID, Const.GroupID);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string senderName = reader.GetString(0);
                    //string receiverName = reader.GetString(1);
                    string senderMsg = reader.GetString(2);
                    int senderId = reader.GetInt32(3);

                    RenderMessageItem(senderMsg, senderId, senderName);
                }

                panelMessage.VerticalScroll.Value = panelMessage.VerticalScroll.Maximum;
                panelMessage.PerformLayout();
                DbConnect._conn.Close();
            }
        }

        void RenderMessageItem(string senderMsg, int senderId, string senderName)
        {
            GroupBox grb = new GroupBox();
            grb.Text = senderName;
            grb.ForeColor = Color.LightBlue;
            grb.Font = new Font("Segoe UI", 8, FontStyle.Italic);
            grb.AutoSize = true;

            Label messageLabel = new Label();
            messageLabel.Text = senderMsg;
            messageLabel.AutoSize = true;
            messageLabel.MaximumSize = new Size(panelMessage.Width - 40, 0); // Max width to prevent overflow
            messageLabel.BackColor = Color.Transparent; //FromArgb(229, 239, 255);
            messageLabel.ForeColor = Color.White;
            messageLabel.Padding = new Padding(5);
            messageLabel.Margin = new Padding(10);
            messageLabel.BorderStyle = BorderStyle.None;
            messageLabel.Location = new Point(5, 10);
            messageLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);


            grb.Location = new Point(10, yOffset);
            grb.Height = messageLabel.PreferredHeight;
            grb.Padding = new Padding(5);
            grb.Height = 40;
            grb.MaximumSize = new Size(panelMessage.Width - 40, 40);

            if (senderId == Const.currentUserId)
            {
                grb.ForeColor = Color.LimeGreen;
            }
            //else messageLabel.Location = new Point(10, yOffset);

            grb.Controls.Add(messageLabel);
            panelMessage.Controls.Add(grb);

            yOffset += 50;//messageLabel.Height + 10;
        }

        private void frmChat_Load(object sender, EventArgs e)
        {
            if (Connection._connectionState != Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
            {
                EstablishConnection();
            }
            LoadHistoryChat();
        }

        // gửi tin nhắn
        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = txtMessage.Text;
            string senderName = Const.UserName;
            txtMessage.Clear();

            // Gọi phương thức SendMessage trên server
            if (Const.MessTag == "single")
            {
                Connection.chatHub.Invoke("SendMessage", Const.currentUserId, Const.ReceiverID, 0, message);
            }
            else
            {
                Connection.chatHub.Invoke("SendMessage", Const.currentUserId, 0, Const.GroupID, message);
            }
        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string message = txtMessage.Text;
                txtMessage.Clear();

                // Gọi phương thức SendMessage trên server
                if (Const.MessTag == "single")
                {
                    Connection.chatHub.Invoke("SendMessage", Const.currentUserId, Const.ReceiverID, 0, message);
                }
                else
                {
                    Connection.chatHub.Invoke("SendMessage", Const.currentUserId, 0, Const.GroupID, message);
                }
            }
        }
    }
}
