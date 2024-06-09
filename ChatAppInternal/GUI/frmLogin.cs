using BUS;
using DAL;
using System;
using System.Windows.Forms;

namespace GUI
{
    public partial class frmLogin : Form
    {
        BUS_Employee busEmployee = new BUS_Employee();

        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtEmail.Text != "" && txtPassword.Text != "")
            {
                if (busEmployee.Login(txtEmail.Text, txtPassword.Text))
                {
                    Properties.Settings.Default.Save();
                    Program.email = txtEmail.Text;
                    Const.Email = txtEmail.Text;
                    Const.currentUserId = Const.GetCurrentUserId();

                    Connection.Connect();
                    DbConnect._conn.Close();

                    frmMain fMain = new frmMain(txtEmail.Text);
                    this.Hide();
                    fMain.ShowDialog();
                    //this.Show();
                }
                else
                {
                    MessageBox.Show("Sai email hoặc mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtEmail.Text = "";
                    txtPassword.Text = "";
                    txtEmail.Focus();
                }
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.isSave)
            {
                txtEmail.Text = Properties.Settings.Default.email;
                txtPassword.Text = Properties.Settings.Default.password;
            }
        }

        private void lblForgotPassword_Click(object sender, EventArgs e)
        {
            if (txtEmail.Text != "")
            {
                busEmployee = new BUS_Employee();
                if (busEmployee.IsExistEmail(txtEmail.Text))
                {
                    string password = busEmployee.GetRandomPassword();
                    if (busEmployee.UpdatePassword(txtEmail.Text, password))
                    {
                    }
                    else
                        MessageBox.Show("Không thực hiện được", "Thông báo");
                }
            }
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
