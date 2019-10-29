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
using Newtonsoft.Json.Linq;

namespace IronMan
{
    public partial class Main : Form
    {
        string json;

        public Main()
        {
            InitializeComponent();
      
        }

        private void btnGetPuertos_Click(object sender, EventArgs e)
        {


        }

        private void Main_Load(object sender, EventArgs e)
        {

            Console.WriteLine("START APP");
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                cmbPorts.Items.Add(port);
            }

            cmbSpeed.SelectedIndex = 16;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

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

            Console.WriteLine(json);
            try
            {
                JsonData obj = JsonConvert.DeserializeObject<JsonData>(json);
                Console.WriteLine("Cont_Flex" + obj.Cont_flex + obj.Cont_ext + obj.Ang_flex + obj.Ang_ext);
                txtMax.Text = obj.Cont_flex;
            }
            catch (Exception e)
            {
              
            }
            
            //txtMax.Text = Convert.ToString(obj.Cont_flex);
           

            //Console.WriteLine("Cont_Flex" + result.cont_flex + result.cont_ext + result.ang_flex + result.ang_ext);

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
    }

}
