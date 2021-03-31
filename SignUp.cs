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
    public partial class SignUp : Form
    {
        public SignUp()
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
            string email = txtEmail.Text;
            string name = txtName.Text;
            string password = txtPassword.Text;
            string confPassword = txtConfirmPassword.Text;
            if (email != null && name != null && password != null && confPassword != null)
            {
                if (password == confPassword)
                {
                    cmd = new SqlCommand("SELECT email FROM users WHERE email='" + email + "'", conn);
                    conn.Open();
                    dataReader = cmd.ExecuteReader();
                    
                    if (dataReader.Read())
                    {
                        dataReader.Close();
                        MessageBox.Show("This email address have existed, please try another or login with this email!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                    }
                    else
                    {
                        dataReader.Close();
                        cmd = new SqlCommand("INSERT INTO users(email, name, password) VALUES (@email, @name, @password)", conn);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Registered successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Hide();
                        Form1 form1 = new Form1();
                        form1.ShowDialog();
                    }
                    conn.Close();
                }
                else
                {
                    MessageBox.Show("Password is not match!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter your information to register!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            this.Hide();
            SignIn signIn = new SignIn();
            signIn.ShowDialog();
        }
    }
}
