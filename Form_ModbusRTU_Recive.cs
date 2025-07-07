using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace SerialReceiver
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpenPort_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
                MessageBox.Show("COM1 開啟成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show("錯誤: " + ex.Message);
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadExisting();
            Invoke(new Action(() => {
                txtReceived.AppendText("收到: " + data + Environment.NewLine);
            }));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
                serialPort.Close();
        }
    }
}
