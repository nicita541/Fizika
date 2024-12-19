using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

namespace Fizika
{
    public class ChartForm : Form
    {
        DataGridView dataGridView1;
        Button buttonCalculate, buttonMeasure;
        Label labelHeight;
        TextBox textBoxHeight;

        public void InitializeComponent()
        {
            this.Text = "График данных";
            this.Width = 1200;
            this.Height = 400;

        }
        public ChartForm()
        {
            InitializeComponent();

            // Создание метки для высоты
            labelHeight = new Label
            {
                Text = "Высота h:",
                Location = new Point(10, 10),
                AutoSize = true
            };

            // Создание текстового поля для ввода высоты
            textBoxHeight = new TextBox
            {
                Location = new Point(labelHeight.Right + 5, 10),
                Width = 100
            };

            // Создание кнопки "Расчитать"
            buttonCalculate = new Button
            {
                Text = "Расчитать",
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right

            };

            // Устанавливаем позицию кнопки "Расчитать" в правом нижнем углу
            buttonCalculate.Location = new Point(this.ClientSize.Width - buttonCalculate.Width - 10, this.ClientSize.Height - buttonCalculate.Height - 10);

            // Создание кнопки "Произвести замер"
            buttonMeasure = new Button
            {
                Text = "Произвести замер",
                Size = new Size(120, 30),
                Location = new Point(textBoxHeight.Right + 10, 8) // Расположить справа от textBoxHeight
            };

            // Добавляем обработчики нажатия
            buttonCalculate.Click += Button_Click;
            buttonMeasure.Click += ButtonMeasure_Click;


            // Добавляем элементы на форму
            this.Controls.Add(labelHeight);
            this.Controls.Add(textBoxHeight);
            this.Controls.Add(buttonCalculate);
            this.Controls.Add(buttonMeasure);

            // Событие изменения размеров формы, чтобы кнопки оставались на месте
            this.Resize += ChartForm_Resize;

            // Настройка таблицы
            dataGridView1 = new DataGridView
            {
                Location = new Point(10, 50),
                Size = new Size(this.ClientSize.Width - 20, this.ClientSize.Height - 97),
                ColumnCount = 9,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowTemplate = { Height = 25 },
                AllowUserToResizeRows = false
            };

            // Инициализация заголовков столбцов
            dataGridView1.Columns[0].Name = "№ п/п";
            dataGridView1.Columns[1].Name = "m, кг";
            dataGridView1.Columns[2].Name = "t, с";
            dataGridView1.Columns[3].Name = "<t>, с";
            dataGridView1.Columns[4].Name = "(a), м/с²";
            dataGridView1.Columns[5].Name = "υ, м/с";
            dataGridView1.Columns[6].Name = "Fн, Н";
            dataGridView1.Columns[7].Name = "Eп, Дж";
            dataGridView1.Columns[8].Name = "Eк, Дж";

            // Добавление строк
            for (int i = 0; i < 3; i++)
            {
                dataGridView1.Rows.Add("1");
                dataGridView1.Rows.Add("2");
                dataGridView1.Rows.Add("3");
            }
            dataGridView1.CellPainting += dataGridView1_CellPainting;

            // Настройка доступности редактирования ячеек
            for (int i = 0; i < 9; i++)
            {
                dataGridView1.Columns[i].ReadOnly = true;
                if (i == 1)
                {
                    dataGridView1.Rows[1].Cells[1].ReadOnly = false;
                    dataGridView1.Rows[4].Cells[1].ReadOnly = false;
                    dataGridView1.Rows[7].Cells[1].ReadOnly = false;
                }
            }

            // Добавление таблицы на форму
            this.Controls.Add(dataGridView1);
        }

        private void ChartForm_Resize(object sender, EventArgs e)
        {
            // Подгоняем размер DataGridView под ширину формы при изменении размеров окна
            dataGridView1.Width = this.ClientSize.Width - 20;

            buttonCalculate.Location = new Point(this.ClientSize.Width - buttonCalculate.Width - 10, this.ClientSize.Height - buttonCalculate.Height - 10);
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // Исключаем первый и третий столбцы из объединения
            if (e.ColumnIndex == 0 || e.ColumnIndex == 2)
                return;

            // Проверяем, что находимся в первой строке каждой группы для объединения
            if (e.RowIndex % 3 == 0)
            {
                e.Handled = true;

                // Вычисляем высоту для объединения трех строк
                int combinedHeight = dataGridView1.Rows[e.RowIndex].Height
                                     + dataGridView1.Rows[e.RowIndex + 1].Height
                                     + dataGridView1.Rows[e.RowIndex + 2].Height;

                // Создаем прямоугольник для объединенной ячейки
                Rectangle cellRectangle = new Rectangle(e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Width - 1, combinedHeight - 1);

                // Рисуем объединенную ячейку (белый фон и черная граница)
                e.Graphics.FillRectangle(Brushes.White, cellRectangle);
                e.Graphics.DrawRectangle(Pens.Black, cellRectangle);

                // Получаем текст из средней ячейки в группе (строка 1, 4, 7, и т.д.)
                string cellText = dataGridView1.Rows[e.RowIndex + 1].Cells[e.ColumnIndex].Value?.ToString();

                // Рисуем текст по центру объединенной ячейки, включаем перенос текста, если он не умещается
                TextRenderer.DrawText(e.Graphics, cellText, e.CellStyle.Font, cellRectangle, e.CellStyle.ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
            }
            // Пропускаем рисование ячеек для строк, которые входят в состав объединенной ячейки
            else if (e.RowIndex % 3 == 1 || e.RowIndex % 3 == 2)
            {
                e.Handled = true;
            }
        }

        private void matimate(double h, double m, double t1, double t2, double t3, int row)
        {
            double tcp = (t1 + t2 + t3) / 3;
            double a = (2 * h) / (tcp * tcp);
            double u = a * tcp;
            double Fp = (m / 2) * (9.80665f - a);
            double Ep = m * 9.80665f * h;
            double Ek = (m * (u * u)) / 2;

            dataGridView1.Rows[row].Cells[3].Value = Math.Round(tcp, 4).ToString();
            dataGridView1.Rows[row].Cells[4].Value = Math.Round(a, 4).ToString();
            dataGridView1.Rows[row].Cells[5].Value = Math.Round(u, 4).ToString();
            dataGridView1.Rows[row].Cells[6].Value = Math.Round(Fp, 4).ToString();
            dataGridView1.Rows[row].Cells[7].Value = Math.Round(Ep, 4).ToString();
            dataGridView1.Rows[row].Cells[8].Value = Math.Round(Ek, 4).ToString();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Form1.sr.DataReceived -= DataReceivedHandler;
            double height = 0.6f;

            // Получаем значение высоты из текстового поля, если оно введено
            if (double.TryParse(textBoxHeight.Text, out double parsedHeight))
            {
                height = parsedHeight;
            }
            try
            {
                for (int i = 0; i < 9; i += 3)
                {
                    double value1 = double.TryParse(dataGridView1.Rows[i + 1].Cells[1].Value?.ToString(), out double parsedValue1) ? parsedValue1 : 0;
                    double value2 = double.TryParse(dataGridView1.Rows[i].Cells[2].Value?.ToString(), out double parsedValue2) ? parsedValue2 : 0;
                    double value3 = double.TryParse(dataGridView1.Rows[i + 1].Cells[2].Value?.ToString(), out double parsedValue3) ? parsedValue3 : 0;
                    double value4 = double.TryParse(dataGridView1.Rows[i + 2].Cells[2].Value?.ToString(), out double parsedValue4) ? parsedValue4 : 0;

                    matimate(height, value1, value2, value3, value4, i + 1);
                }

            }
            catch { }

            dataGridView1.Invalidate();
        }

        private void ButtonMeasure_Click(object sender, EventArgs e)
        {
            flag = true;
            Form1.sr.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            // Отправка строки в COM-порт
            Form1.sr.WriteLine("true");
        }

        int i = 0;
        bool flag = true;
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            // Проверка, если данные больше не нужны
            if (i >= 9)
            {
                if (Form1.sr.IsOpen)
                {
                    Form1.sr.Close();
                    MessageBox.Show("Получение данных остановлено.");
                }
                return; // Прерываем выполнение обработчика
            }

            try
            {
                string data = Form1.sr.ReadLine();
                if (flag && data != null)
                {
                    // Удаление символов новой строки и возврата каретки
                    data = data.Replace("\r", "").Replace("\n", "").Replace("\0", "");

                    this.Invoke((MethodInvoker)delegate
                    {
                        // Запись данных в ячейку таблицы
                        dataGridView1.Rows[i].Cells[2].Value = Convert.ToDouble(data);
                        i++;
                        flag = false; // Останавливаем обработку, пока не будет снова установлено в true
                    });
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Ошибка при чтении данных: " + ex.Message);
            }
        }

    }
}
