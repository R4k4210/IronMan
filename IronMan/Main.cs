using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Management;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace IronMan
{
    public partial class Main : Form
    {
        string json;
        public Main()
        {
            InitializeComponent();
      
        }

        private void Main_Load(object sender, EventArgs e)
        {

            txtFlexCount.BackColor = Color.White;
            txtFlexAng.BackColor = Color.White;
            txtExtCount.BackColor = Color.White;
            txtAngExt.BackColor = Color.White;

            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                cmbPorts.Items.Add(port);
            }

            cmbSpeed.SelectedIndex = 16;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            chart1.Series["Flex"].Points.Clear();
            chart1.Series["Ext"].Points.Clear();

            if (cmbPorts.Text.Trim().Length == 0)
            {
                MessageBox.Show("Select a port and try again", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if(cmbSpeed.Text.Trim().Length == 0)
            {
                MessageBox.Show("Select a speed and try again", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            serialPort1.PortName = cmbPorts.Text.Trim();
            serialPort1.BaudRate = Convert.ToInt32(cmbSpeed.Text.Trim());
            serialPort1.Open();


            if (serialPort1.IsOpen)
            {
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
            }
        }

        private void WritePayload(string json)
        {

            //Console.WriteLine(json);
            try
            {
  
                dynamic data = JsonConvert.DeserializeObject(json);
                JsonData jsonObj = new JsonData();
                jsonObj.Cont_flex = data["d"].Cont_flex;
                jsonObj.Ang_flex = data["d"].Ang_flex;
                jsonObj.Cont_ext = data["d"].Cont_ext;
                jsonObj.Ang_ext = data["d"].Ang_ext;    
                
                txtFlexCount.Text = jsonObj.Cont_flex;
                txtFlexAng.Text = jsonObj.Ang_flex;
                txtExtCount.Text = jsonObj.Cont_ext;
                txtAngExt.Text = jsonObj.Ang_ext;

                UpdateChartValues(jsonObj.Cont_flex, jsonObj.Ang_flex, jsonObj.Cont_ext, jsonObj.Ang_ext);
            }
            catch (Exception e)
            {
              
            }

        }

        private void DisplayThreadText(object sender, EventArgs e)
        {
            WritePayload(json);
           
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            json = serialPort1.ReadLine().Trim();
            serialPort1.NewLine = "\r\n";
            Invoke(new EventHandler(DisplayThreadText));
        }

        private void UpdateChartValues(string contFlex, string flex, string contExt, string ext)
        {
            
            chart1.Series["Flex"].Points.AddY(Convert.ToInt32(flex));
            chart1.Series["Ext"].Points.AddY(Convert.ToInt32(ext));
        }
    }

}
