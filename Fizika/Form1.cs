using System;
using System.IO.Ports;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Fizika
{
    public partial class Form1 : Form
    {
        bool isConnected = false;
        SerialPort sr = new SerialPort();
        string selectedPort;

        List<double> ports = new List<double>();

        public Form1()
        {
            InitializeComponent();
            sr.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }
        bool conectText = false;
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = sr.ReadLine();
                // �������� �������� ����� ������ � �������� �������
                data = data.Replace("\r", "").Replace("\n", "");
                if (conectText)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        
                        //����� �������� ������
                        ports.Add(Convert.ToDouble(data));
                    });
                }
                conectText = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("������ ��� ������ ������: " + ex.Message);
            }
        }

        // ���������� ������ ��� ������ COM-������
        private void button1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            // �������� ������ COM ������, ��������� � �������
            string[] portNames = SerialPort.GetPortNames();
            // ���������, ���� �� ��������� �����
            if (portNames.Length == 0)
            {
                MessageBox.Show("COM PORT �� ������");
            }
            else
            {
                foreach (string portName in portNames)
                {
                    // ��������� ��������� COM ����� � ComboBox           
                    comboBox1.Items.Add(portName);
                }
                comboBox1.SelectedIndex = 0; // ������������� �������� ������ ����
            }
            selectedPort = comboBox1.GetItemText(comboBox1.SelectedItem);
        }

        // ���������� ������ ��� ����������� ��� ����������
        private void connectButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedPort))
            {
                MessageBox.Show("COM PORT �� ������");
                return;
            }

            if (!isConnected)
            {
                connectToArduino();
            }
            else
            {
                disconnectFromArduino();
            }
        }

        private void connectToArduino()
        {
            try
            {
                sr.PortName = selectedPort;
                sr.BaudRate = 9600;
                sr.Parity = Parity.None;
                sr.DataBits = 8;
                sr.StopBits = StopBits.One;
                sr.Open();
                isConnected = true;
                button2.Text = "Disconnect";
                MessageBox.Show("���������� � " + selectedPort);
            }
            catch (Exception ex)
            {
                MessageBox.Show("������ �����������: " + ex.Message);
            }
        }

        private void disconnectFromArduino()
        {
            try
            {
                sr.Close();
                isConnected = false;
                button2.Text = "Connect";
                MessageBox.Show("��������� �� " + selectedPort);
                ShowChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("������ ����������: " + ex.Message);
            }
        }
        private void ShowChart()
        {
            // �������� � ����������� ������ ���� � ��������
            ChartForm chartForm = new ChartForm(ports);
            chartForm.Show();
        }
    }
}

