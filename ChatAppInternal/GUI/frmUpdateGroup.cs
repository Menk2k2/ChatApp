using BUS;
using DAL;
using DTO;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace GUI
{
    public partial class frmUpdateGroup : Form
    {
        private List<int> listAdd = new List<int>();

        public frmUpdateGroup()
        {
            InitializeComponent();
        }

        private void frmUpdateGroup_Load(object sender, EventArgs e)
        {
            if (Const.GroupID > 0)
            {
                LoadGroupInfo();
            }
            else
            {
                LoadListFrien("");
            }

            if (Connection._connectionState != Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
            {
                EstablishConnection();
            }
        }

        private void EstablishConnection()
        {
            // khi nhận tín hiệu từ server thông ua phương thức addMessageToPage thì hiển thị tin nhắn
            Connection.chatHub.On<int, string>("pushMessage", (error, message) =>
            {
                if (this.InvokeRequired)
                {
                    // Invoke the method on the UI thread
                    this.Invoke(new MethodInvoker(delegate
                    {
                        if (error == 0)
                        {
                            MessageBox.Show(message, "Thông báo hệ thống", MessageBoxButtons.OK, error == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                            this.Close();
                        }
                    }));
                }
                else
                {
                    // Execute the method directly on the UI thread
                    if (error == 0)
                    {
                        this.Close();
                    }
                }
            });
        }

        void LoadGroupInfo()
        {
            if (DbConnect._conn.State != System.Data.ConnectionState.Open) DbConnect._conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = DbConnect._conn;
            cmd.CommandText = string.Format("SELECT Name from Channels where id = {0}", Const.GroupID);
            var groupName = Convert.ToString(cmd.ExecuteScalar());
            txtGroupName.Text = groupName;
            lbGroupName.Text = "Thông tin nhóm";
            btnSubmitCreate.Text = "Cập nhật";

            cmd.CommandText = string.Format(@"SELECT 
                                            CASE
                                                WHEN cl.Id > 0 THEN cl.Id
                                                ELSE 0
                                            END as clId,
                                            el.* FROM tblEmployee el
                                            left join ChannelUsers cu on cu.UserId = el.Id
                                            left join Channels cl on cl.Id = cu.ChannelId where
                                            Email != '{0}'", Const.Email);
            SqlDataReader reader = cmd.ExecuteReader();
            panelListFriend.Controls.Clear();

            int numberOfCheckBoxes = 0;
            int checkBoxHeight = 30;
            int checkBoxSpacing = 2;
            int leftMargin = 0;
            int panelWidth = panelListFriend.Width - 3;// - SystemInformation.VerticalScrollBarWidth;
            while (reader.Read())
            {
                int chanel_id = reader.GetInt32(0);
                int id = reader.GetInt32(1);
                string name = reader.GetString(2);

                // Create CheckBox
                CheckBox checkBox = new CheckBox();
                checkBox.ForeColor = Color.Black;
                checkBox.BackColor = Color.White;
                checkBox.Text = name;
                checkBox.Tag = id;
                checkBox.Padding = new Padding(10, 0, 10, 0);
                if (chanel_id == Const.GroupID)
                {
                    if (!listAdd.Contains(id)) listAdd.Add(id);
                    checkBox.Checked = true;
                    checkBox.BackColor = Color.FromArgb(134, 168, 223);
                }
                
                checkBox.Size = new Size(panelWidth - leftMargin, checkBoxHeight);
                checkBox.Location = new Point(leftMargin, numberOfCheckBoxes * (checkBoxHeight + checkBoxSpacing));

                // Set CheckBox properties
                checkBox.Font = new Font(checkBox.Font.FontFamily, 12, FontStyle.Bold);

                // Attach event handler
                checkBox.Click += CheckBox_Click;

                // Add CheckBox to Panel
                panelListFriend.Controls.Add(checkBox);

                numberOfCheckBoxes++;
            }

            panelListFriend.AutoScrollMinSize = new Size(0, numberOfCheckBoxes * (checkBoxHeight + checkBoxSpacing));
            DbConnect._conn.Close();
        }

        void LoadListFrien(string search = "")
        {
            panelListFriend.Controls.Clear();
            txtGroupName.Text = "";
            lbGroupName.Text = "Tạo nhóm";
            btnSubmitCreate.Text = "Tạo nhóm";

            int numberOfCheckBoxes = 0;
            int checkBoxHeight = 30;
            int checkBoxSpacing = 2;
            int leftMargin = 0;
            int panelWidth = panelListFriend.Width - 3;// - SystemInformation.VerticalScrollBarWidth;

            if (DbConnect._conn.State != System.Data.ConnectionState.Open) DbConnect._conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = DbConnect._conn;
            cmd.CommandText = string.Format("SELECT * FROM tblEmployee where ( Name like '%{0}%' or PhoneNumber like '%{0}%' or Email like '%{0}%' ) and Email != '{1}'", search, Const.Email);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);

                // Create CheckBox
                CheckBox checkBox = new CheckBox();
                checkBox.ForeColor = Color.Black;
                checkBox.BackColor = Color.White;
                checkBox.Text = name;
                checkBox.Tag = id;
                checkBox.Padding = new Padding(10, 0, 10, 0);
                checkBox.Size = new Size(panelWidth, checkBoxHeight);
                checkBox.Location = new Point(leftMargin, numberOfCheckBoxes * (checkBoxHeight + checkBoxSpacing));

                // Set CheckBox properties
                checkBox.Font = new Font(checkBox.Font.FontFamily, 12, FontStyle.Bold);

                // Attach event handler
                checkBox.Click += CheckBox_Click;

                // Add CheckBox to Panel
                panelListFriend.Controls.Add(checkBox);

                numberOfCheckBoxes++;
            }

            panelListFriend.AutoScrollMinSize = new Size(0, numberOfCheckBoxes * (checkBoxHeight + checkBoxSpacing));
            DbConnect._conn.Close();
        }

        private void CheckBox_Click(object sender, EventArgs e)
        {
            // Get the CheckBox that was clicked
            CheckBox clickedCheckBox = sender as CheckBox;

            if (clickedCheckBox != null)
            {
                // Retrieve the Tag value
                int tagValue = (int)clickedCheckBox.Tag;

                if (clickedCheckBox.CheckState == CheckState.Checked)
                {
                    if (!listAdd.Contains(tagValue)) listAdd.Add(tagValue);
                    clickedCheckBox.BackColor = Color.FromArgb(134, 168, 223);
                }
                else
                {
                    listAdd.Remove(tagValue);
                    clickedCheckBox.BackColor = Color.White;
                }
            }
        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void btnCancelCreate_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            LoadListFrien(txtSearch.Text);
        }

        private void btnSubmitCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtGroupName.Text))
                {
                    MessageBox.Show("Tên nhóm kông được để trống", "Thông báo tạo mới", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    listAdd.Add(Const.currentUserId);
                    Connection.chatHub.Invoke("CreateGroup", txtGroupName.Text, listAdd, Const.currentUserId, Const.GroupID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã có lỗi xảy ra: " + ex.ToString(), "Kết quả tạo mới", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                DbConnect._conn.Close();
            }
        }
    }
}
