using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SNMPForm
{
    public partial class SignIn : Form
    {
        public SignIn()
        {
            InitializeComponent();
        }

        static string connectDB = "Data Source=DESKTOP-SS93F2R; Initial Catalog=snmp; User ID=sa; Password=131123Na";
        SqlConnection conn = new SqlConnection(connectDB);
        SqlCommand cmd;
        SqlDataAdapter adapter;
        SqlDataReader dataReader;

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            this.Hide();
            SignUp signUp = new SignUp();
            signUp.ShowDialog();
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            if (txtEmail.Text != string.Empty && txtPassword.Text != string.Empty)
            {
                cmd = new SqlCommand("SELECT email, password FROM users WHERE email='" + txtEmail.Text + "' AND password='" + txtPassword.Text + "'", conn);
                conn.Open();
                dataReader = cmd.ExecuteReader();
                if (dataReader.Read())
                {
                    dataReader.Close();
                    MessageBox.Show("Logged in successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide();
                    Form1 form1 = new Form1();
                    form1.ShowDialog();
                }
                else
                {
                    dataReader.Close();
                    MessageBox.Show("Invalid email or password, please try again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                conn.Close();
            }
            else
            {
                MessageBox.Show("Please enter your email and password to login!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
