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
using System.Net.NetworkInformation;
using System.Net.Mail;

namespace SNMPForm
{
    public partial class Form2 : Form
    {
        public Form2(string ipAdd, string comm, string name)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            lblIpAddress.Text = ipAdd;
            lblCommunity.Text = comm;
            lblName.Text = name;
        }

        public void query(string ipAdd, string comm)
        {
            // snmp community name
            OctetString community = new OctetString(comm);

            // define agent parameters class
            AgentParameters param = new AgentParameters(community);

            // Set SNMP version to 1 or 2
            param.Version = SnmpVersion.Ver1;

            // Construct the agent address object
            // IP Address is easy to use here because it will try to resolve contructor 
            // parameter if it doesn't parse to an IP Address
            IPAddress ipAddress = IPAddress.Parse(ipAdd);
            IpAddress agent = new IpAddress(ipAdd);

            // construct target
            UdpTarget target = new UdpTarget((IPAddress)agent, 161, 2000, 1);

            // pdu class use for all requests
            Pdu pdu = new Pdu(PduType.Get);
            pdu.VbList.Add("1.3.6.1.2.1.1.1.0"); //sysDescr
            pdu.VbList.Add("1.3.6.1.2.1.1.2.0"); //sysObjectID
            pdu.VbList.Add("1.3.6.1.2.1.1.3.0"); //sysUpTime
            pdu.VbList.Add("1.3.6.1.2.1.1.4.0"); //sysContact
            pdu.VbList.Add("1.3.6.1.2.1.1.5.0"); //sysName

            // Make snmp request 
            SnmpV1Packet result = (SnmpV1Packet)target.Request(pdu, param);

            // If result is null then agent didn't reply or we couldn't par se the reply.
            if (result != null)
            {
                // ErrorStatus other then 0 is an error returned by
                // the Agent - see SnmpConstants for error definitions
                if (result.Pdu.ErrorStatus != 0)
                {
                    // agent reported an error with the request
                    string errStatus = Convert.ToString(result.Pdu.ErrorStatus);
                    string errIndex = Convert.ToString(result.Pdu.ErrorIndex);
                    MessageBox.Show(errStatus, errIndex, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        string sysOid = result.Pdu.VbList[i].Oid.ToString();
                        string sysName = SnmpConstants.GetTypeName(result.Pdu.VbList[i].Value.Type);
                        string value = result.Pdu.VbList[i].Value.ToString();

                        string[] row = { sysOid, sysName, value };
                        ListViewItem item = new ListViewItem(row);
                        listViewResult.Items.Add(item);
                    }
                }
            }
            else
            {
                MessageBox.Show("No response received from SNMP agent.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string ipAddress = lblIpAddress.Text;
            string community = lblCommunity.Text;
            Thread qThread = new Thread(() => query(ipAddress, community));
            qThread.Start();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (ping()) 
            {
                lblStatus.Text = "OK!";
                timer1.Start();
            }
            else
            {
                lblStatus.Text = "Warming!";
                sendMail();
            }
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private bool ping()
        {
            Ping ping;
            PingReply reply;
            IPAddress ipAddress;

            string ipPing = lblIpAddress.Text;
            ipAddress = IPAddress.Parse(ipPing);

            ping = new Ping();
            reply = ping.Send(ipPing, 1000);

            if (reply.Status != IPStatus.Success)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void sendMail()
        {
            try
            {
                string fromAddress = "thuynguyen080700@gmail.com";
                var toAddress = new MailAddress("nttthuy08072000@gmail.com");
                const string fromPassword = "TN080700";
                string subject = "Notification!";
                string body = "Your system have some problem, check it!";

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromAddress);
                    mail.To.Add(toAddress);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                        MessageBox.Show("Your mail sent!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
