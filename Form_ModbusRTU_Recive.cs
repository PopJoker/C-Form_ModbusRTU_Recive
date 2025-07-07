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
            int bytesToRead = serialPort.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            serialPort.Read(buffer, 0, bytesToRead);

            string hexString = BitConverter.ToString(buffer).Replace("-", " ");

    // 假設 Modbus RTU 回應格式: SlaveID(1) + Func(1) + ByteCount(1) + Data(ByteCount) + CRC(2)
            if (buffer.Length >= 5)
            {
                int byteCount = buffer[2];
                if (buffer.Length >= 3 + byteCount + 2)
                {
                    byte[] dataBytes = new byte[byteCount];
                    Array.Copy(buffer, 3, dataBytes, 0, byteCount);
                    string asciiData = System.Text.Encoding.ASCII.GetString(dataBytes);

                    Invoke(new Action(() =>
                    {
                        txtReceived.AppendText($"HEX: {hexString}\r\nASCII資料區: {asciiData}\r\n");
                    }));
                    return;//不想用else :D
                }
            }

        // 若格式不符，至少顯示 HEX
            Invoke(new Action(() =>
            {
                txtReceived.AppendText($"HEX: {hexString}\r\n");
            }));
            /*string data = serialPort.ReadExisting();
            Invoke(new Action(() => {
                txtReceived.AppendText("收到: " + data + Environment.NewLine);
            }));*/
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
                serialPort.Close();
        }
    }
}
