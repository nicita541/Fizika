using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Fizika
{

    public class ChartForm : Form
    {
        private Chart chart;

        public ChartForm(List<double> data)
        {
            this.Text = "График данных";
            this.Width = 800;
            this.Height = 600;

            // Инициализация графика
            chart = new Chart();
            chart.Dock = DockStyle.Fill;

            // Создание области для графика
            ChartArea chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            // Создание серии данных
            Series series = new Series();
            series.ChartType = SeriesChartType.Line; // Линейный график
            series.BorderWidth = 3;                  // Установка толщины линии
            series.Color = Color.Red;                 // Установка цвета линии

            // Добавление данных в серию
            for (int i = 0; i < data.Count; i++)
            {
                series.Points.AddXY(i, data[i]);
            }

            // Добавление серии на график
            chart.Series.Add(series);

            // Добавление графика на форму
            this.Controls.Add(chart);
        }
    }
}
