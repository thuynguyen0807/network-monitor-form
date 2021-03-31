using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using SnmpSharpNet;
using System.Threading;
using System.Data.SqlClient;
using System.Net.Mail;
using SNMPForm.Properties;

namespace SNMPForm
{
    public partial class Form1 : Form
    {
        static string connectDB = "Data Source=DESKTOP-SS93F2R; Initial Catalog=snmp; User ID=sa; Password=131123Na";
        SqlConnection conn = new SqlConnection(connectDB);
        SqlCommand cmd;
        SqlDataAdapter adapter;
        public Form1()
        {
            InitializeComponent();
            displayData();
        }
        
        private void btnSave_Click(object sender, EventArgs e)
        {
            string ipAddress = txtIPAddress.Text;
            string community = txtCommunity.Text;
            string name = txtName.Text;
            if (string.IsNullOrEmpty(ipAddress) || string.IsNullOrEmpty(community))
            {
                MessageBox.Show("Please enter details of device to insert!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                cmd = new SqlCommand("INSERT INTO device(ip_address, community, name) VALUES (@ip_address, @community, @name)", conn);
                conn.Open();
                cmd.Parameters.AddWithValue("@ip_address", ipAddress);
                cmd.Parameters.AddWithValue("@community", community);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Add device successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                displayData();
                clearData();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string id = listViewDevice.SelectedItems[0].SubItems[0].Text;
            string ipAddress = txtIPAddress.Text;
            string community = txtCommunity.Text;
            string name = txtName.Text;
            
            if (string.IsNullOrEmpty(ipAddress) || string.IsNullOrEmpty(community) || string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please select device to update!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string update_query = "UPDATE device SET ip_address=@ip_address, community=@community, name=@name WHERE id=@id";
                cmd = new SqlCommand(update_query, conn);
                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@ip_address", ipAddress);
                    cmd.Parameters.AddWithValue("@community", community);
                    cmd.Parameters.AddWithValue("@name", name);
                    //adapter = new SqlDataAdapter(cmd);
                    //adapter.UpdateCommand = conn.CreateCommand();
                    //adapter.UpdateCommand.CommandText = update_query;
                    //if (adapter.UpdateCommand.ExecuteNonQuery() > 0)
                    //{
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("Device updated successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    displayData();
                    clearData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            string id = listViewDevice.SelectedItems[0].SubItems[0].Text;

            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Please select device to delete!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string delete_query = "DELETE device WHERE id=@id";
                cmd = new SqlCommand(delete_query, conn);
                conn.Open();
                cmd.Parameters.AddWithValue("@id", id);
                if (MessageBox.Show("Are you sure you want to delete this device?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK) {
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
                MessageBox.Show("Device deleted successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                displayData();
                clearData();
            }
        }

        private void displayData()
        {
            listViewDevice.Items.Clear();
            conn.Open();
            DataTable data = new DataTable();
            string query = "SELECT * FROM device";
            adapter = new SqlDataAdapter(query, conn);
            adapter.Fill(data);
            for (int i = 0; i < data.Rows.Count; i++)
            {
                ListViewItem row = new ListViewItem(data.Rows[i][0].ToString());
                for (int j = 1; j < data.Columns.Count; j++)
                {
                    row.SubItems.Add(data.Rows[i][j].ToString());
                }
                listViewDevice.Items.Add(row);
            }
            conn.Close();
        }

        private void clearData()
        {
            txtCommunity.Clear();
            txtIPAddress.Clear();
            txtName.Clear();
        }

        private void listViewDevice_MouseClick(object sender, MouseEventArgs e)
        {
            string ipAddress = listViewDevice.SelectedItems[0].SubItems[1].Text;
            string community = listViewDevice.SelectedItems[0].SubItems[2].Text;
            string name = listViewDevice.SelectedItems[0].SubItems[3].Text;

            txtIPAddress.Text = ipAddress;
            txtCommunity.Text = community;
            txtName.Text = name;
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            if (txtIPAddress == null)
            {
                MessageBox.Show("Please choose a device to scan!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string ipAdd = listViewDevice.SelectedItems[0].SubItems[1].Text;
                string comm = listViewDevice.SelectedItems[0].SubItems[2].Text;
                string name = listViewDevice.SelectedItems[0].SubItems[3].Text;
                Form2 form_result = new Form2(ipAdd, comm, name);
                Form2 form2 = form_result;
                form2.Show();
            }
        }


        private bool isCollapsed;

       

      
      
    }
}
