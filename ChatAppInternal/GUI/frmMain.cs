using BUS;
using System;
using System.Drawing;
using System.Windows.Forms;
using DAL;
using System.Data.SqlClient;
using Microsoft.AspNet.SignalR.Client;
using System.Linq;
using Guna.UI2.WinForms;

namespace GUI
{
    public partial class frmMain : Form
    {
        BUS_Employee busEmployee = new BUS_Employee();
        frmChat fChat = new frmChat();
        frmUpdateGroup fUpdateGroup = new frmUpdateGroup();
        frmStart fStart = new frmStart();
        frmAccount fAccount;
        int filterIndex = 0;

        public frmMain(string email)
        {
            InitializeComponent();
            var str = busEmployee.GetEmployeeIdName(email);
            var strlist = str.Split('|');
            txtUserName.Text = strlist[1].Trim();
            Const.UserName = strlist[1].Trim();
            Const.Email = email;
            txtUserName.TextAlignment = ContentAlignment.MiddleLeft;
            txtUserName.Margin = new Padding(txtUserName.Margin.Left, 10, txtUserName.Margin.Right, txtUserName.Margin.Bottom);

            cbbFilter.SelectedIndex = 0;

            // create tooltip for button
            ToolTip toolTip1 = new ToolTip();
            toolTip1.SetToolTip(btnAddGroup, "Tạo nhóm chat");

            // render list message
            RenderListMessage();

            if (!busEmployee.GetEmployeeRole(email))
            {
                fStart.TopLevel = false;
                fStart.Dock = DockStyle.Fill;
                pnlBody.Controls.Add(fStart);
                fStart.Show();
            }
            else
            {
                fStart.TopLevel = false;
                fStart.Dock = DockStyle.Fill;
                pnlBody.Controls.Add(fStart);
                fStart.Show();
            }
            fAccount = new frmAccount(email);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (Connection._connectionState != Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
            {
                EstablishConnection();
            }
        }

        //Hàm kết nối tới web server để gửi dữ liệu
        private void EstablishConnection()
        {
            // khi nhận tín hiệu từ server thông ua phương thức addMessageToPage thì hiển thị tin nhắn
            Connection.chatHub.On<int, string>("pushMessage", (error, message) =>
            {
                InvokeRenderListMessage(txtSearch.Text);
            });
        }

        void InvokeRenderListMessage(string search = "")
        {
            if (panelListMessage.InvokeRequired)
            {
                panelListMessage.Invoke(new Action(() =>
                {
                    RenderListMessage(search);
                }));
            }
            else
            {
                RenderListMessage(txtSearch.Text);
            }
        }

        // hàm hiển thị danh sách tin nhắn
        void RenderListMessage(string search = "")
        {
            panelListMessage.Controls.Clear();
            int numberOfButtons = 0; // Adjust the number of buttons as needed
            int buttonHeight = 50;
            int buttonSpacing = 0;

            if (DbConnect._conn.State != System.Data.ConnectionState.Open) DbConnect._conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = DbConnect._conn;

            if(filterIndex == 0 || filterIndex == 1)
            {
                // render list group
                cmd.CommandText = string.Format("SELECT cl.Id, cl.Name FROM Channels cl JOIN ChannelUsers cu on cu.ChannelId = cl.Id WHERE cu.UserId = {0} and cl.Name like '%{1}%' ORDER BY cl.CreateDate DESC", Const.currentUserId, txtSearch.Text);
                SqlDataReader reader = cmd.ExecuteReader();
                //var i = 0;
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);

                    var myButton = CreateMessageButton(name, id.ToString(), numberOfButtons, "group");

                    panelListMessage.Controls.Add(myButton);
                    numberOfButtons++;
                }
                reader.Close();
            }

            if (filterIndex == 0 || filterIndex == 2)
            {
                // render list friend
                cmd.CommandText = string.Format("SELECT * FROM tblEmployee where (Name like '%{0}%' or PhoneNumber like '%{0}%' or Email like '%{0}%') and Email != '{1}'", search, Const.Email);
                SqlDataReader reader1 = cmd.ExecuteReader();
                while (reader1.Read())
                {
                    int id = reader1.GetInt32(0);
                    string name = reader1.GetString(1);

                    var myButton = CreateMessageButton(name, id.ToString(), numberOfButtons, "single");

                    panelListMessage.Controls.Add(myButton);
                    numberOfButtons++;
                }
                reader1.Close();
            }

            panelListMessage.AutoScrollMinSize = new Size(0, numberOfButtons * (buttonHeight + buttonSpacing));
            DbConnect._conn.Close();

            //listMessage.SmallImageList = imageList;
        }

        Guna2GradientButton CreateMessageButton(string text, string id, int index, string tag = "single")
        {
            int buttonHeight = 50;
            int buttonSpacing = 0;
            int panelWidth = panelListMessage.Width - SystemInformation.VerticalScrollBarWidth;

            // Create a new Guna2GradientButton
            Guna2GradientButton myButton = new Guna2GradientButton();
            myButton.Name = id;
            myButton.Tag = tag;

            // Set the button properties
            myButton.Location = new Point(50, 50);
            myButton.Text = text;
            myButton.Font = new Font("Segoe UI", 12);
            myButton.ForeColor = Color.White;

            // Set the gradient colors
            myButton.FillColor = Color.FromArgb(40, 42, 52); // Start color
            myButton.FillColor2 = Color.FromArgb(30, 32, 42); // End color

            // Set the image
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            myButton.Image = Image.FromFile(basePath + (tag == "single" ? @"Images\nam.png" : @"Images\group.png")); // Replace with your image path
            myButton.ImageAlign = HorizontalAlignment.Left; // Align image to the left
            myButton.ImageSize = new Size(20, 20); // Set the size of the image
            myButton.BackColor = Color.White;
            myButton.BorderRadius = 8;

            // Set text alignment and padding
            myButton.TextAlign = HorizontalAlignment.Left;
            myButton.Padding = new Padding(10, 10, 0, 10);

            myButton.Size = new Size(panelWidth, buttonHeight);
            myButton.Location = new Point(0, index * (buttonHeight + buttonSpacing));
            myButton.Click += Button_Click;

            return myButton;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Guna2GradientButton clickedButton = sender as Guna2GradientButton;

            if (clickedButton != null)
            {
                clickedButton.FillColor = Color.FromArgb(94, 148, 255); // Change the fill color on click
                clickedButton.FillColor2 = Color.FromArgb(74, 128, 235); // Optionally, change the second gradient color too
                                                                         // Reset the colors of other buttons
                foreach (Control control in panelListMessage.Controls)
                {
                    if (control is Guna2GradientButton button && button != clickedButton)
                    {
                        button.FillColor = Color.FromArgb(40, 42, 52);
                        button.FillColor2 = Color.FromArgb(30, 32, 42);
                    }
                }
            }

            Const.MessTag = clickedButton.Tag.ToString();
            if (clickedButton.Tag.ToString() == "single")
                Const.ReceiverID = int.Parse(clickedButton.Name);
            else Const.ReceiverID = 0;
            if (clickedButton.Tag.ToString() == "group")
                Const.GroupID = int.Parse(clickedButton.Name);
            else Const.GroupID = 0;

            txtChatTitle.Text = clickedButton.Text;

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            pic1.Image = Image.FromFile(basePath + (Const.MessTag == "single" ? @"Images\nam.png" : @"Images\group.png"));

            pnlBody.Controls.Clear();
            fChat = new frmChat();
            fChat.TopLevel = false;
            fChat.Dock = DockStyle.Fill;
            pnlBody.Controls.Add(fChat);
            fChat.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            var fLogin = new frmLogin();
            fLogin.Show();
        }

        private void btnAccount_Click(object sender, EventArgs e)
        {
            Const.GroupID = 0;
            txtChatTitle.Text = "Phần mềm chat nội bộ";
            pnlBody.Controls.Clear();
            fAccount.TopLevel = false;
            pnlBody.Controls.Add(fAccount);
            fAccount.Dock = DockStyle.Fill;
            fAccount.Show();
        }

        private void btnStatistic_Click(object sender, EventArgs e)
        {

        }

        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            Const.GroupID = 0;
            fUpdateGroup = new frmUpdateGroup();
            fUpdateGroup.StartPosition = FormStartPosition.CenterScreen;
            fUpdateGroup.Show();
        }

        private void pnlBody_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            RenderListMessage(txtSearch.Text);
        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //cbbFilter
            // Assuming comboBox1 is the name of your ComboBox control
            if (cbbFilter.SelectedIndex != -1)
            {
                filterIndex = cbbFilter.SelectedIndex;
                RenderListMessage(txtSearch.Text);
            }
            else
            {
                // No item is selected
                MessageBox.Show("Please select an item from the ComboBox.");
            }
        }

        private void txtChatTitle_Click(object sender, EventArgs e)
        {
            if(Const.GroupID > 0)
            {
                fUpdateGroup = new frmUpdateGroup();
                fUpdateGroup.StartPosition = FormStartPosition.CenterScreen;
                fUpdateGroup.Show();
            }
        }

        private void txtSearch_TextChanged_1(object sender, EventArgs e)
        {
            RenderListMessage(txtSearch.Text);
        }
    }
}
