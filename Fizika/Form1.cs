using System;
using System.IO.Ports;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Fizika
{
    public partial class Form1 : Form
    {
        public bool isConnected = false;
        public static SerialPort sr = new SerialPort();
        public string selectedPort;

        public Form1()
        {
            InitializeComponent();
        }

        // Обработчик кнопки для поиска COM-портов
        private void button1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            // Получаем список COM портов, доступных в системе
            string[] portNames = SerialPort.GetPortNames();
            // Проверяем, есть ли доступные порты
            if (portNames.Length == 0)
            {
                MessageBox.Show("COM PORT не найден");
            }
            else
            {
                foreach (string portName in portNames)
                {
                    // Добавляем доступные COM порты в ComboBox           
                    comboBox1.Items.Add(portName);
                }
                comboBox1.SelectedIndex = 0; // Автоматически выбираем первый порт
            }
            selectedPort = comboBox1.GetItemText(comboBox1.SelectedItem);
        }

        // Обработчик кнопки для подключения или отключения
        private void connectButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedPort))
            {
                MessageBox.Show("COM PORT не выбран");
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
                ShowChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения: " + ex.Message);
            }
        }

        private void disconnectFromArduino()
        {
            try
            {
                sr.Close();
                isConnected = false;
                button2.Text = "Connect";
                MessageBox.Show("Отключено от " + selectedPort);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка отключения: " + ex.Message);
            }
        }
        private void ShowChart()
        {
            // Создание и отображение нового окна с графиком
            ChartForm chartForm = new ChartForm();
            chartForm.Show();
        }
    }
}

