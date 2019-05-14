using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RawTools.QC;
using RawTools.Utilities;


namespace RawToolsViz
{
    using OxyPlot;
    using OxyPlot.Series;
    using OxyPlot.Axes;
    using PresentationControls;

    public partial class QcDataViz : Form
    {
        DataTable QcData;

        public QcDataViz()
        {

        }

        public QcDataViz(string pathToQcCsvFile)
        {
            this.InitializeComponent();
            QcData = ReadWrite.LoadDataTable(pathToQcCsvFile);
            
            foreach (DataColumn d in QcData.Columns)
            {
                double i;
                if (Double.TryParse(QcData.Rows[0][d.ColumnName].ToString(), out i))
                {
                    checkBoxComboBox1.Items.Add(d.ColumnName);
                }
            }
            axisTypeComboBox.SelectedIndex = 0;
            this.Text = "QcDataViz - " + pathToQcCsvFile;
        }

        private void checkBoxComboBox1_CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void UpdateChart()
        {
            if ((yMinFixed.Checked && yMinFixedValue.BackColor == Color.Red)
                || yMaxFixed.Checked && yMaxFixedValue.BackColor == Color.Red)
            {
                return;
            }

            List<string> selected = new List<string>();

            foreach (var data in checkBoxComboBox1.CheckBoxItems)
            {
                //if (!(from x in checkBoxComboBox1.CheckBoxItems select x.Text).Contains(data.ColumnName)) return;

                if (data.Checked)
                {
                    selected.Add(data.Text);
                }
            }

            var myModel = new PlotModel();

            myModel.LegendPlacement = LegendPlacement.Outside;

            if (logYScale.Checked)
            {
                myModel.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Left, Base = Convert.ToDouble(logYScaleBase.Text) });
            }
            else
            {
                myModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
            }

            if (axisTypeComboBox.SelectedIndex == 0)
            {
                foreach (string columnName in selected)
                {
                    var scatter = new ScatterSeries();
                    scatter.Title = columnName;
                    scatter.MarkerType = MarkerType.Circle;
                    scatter.MarkerSize = 4.0;

                    for (int currRow = 0; currRow < QcData.Rows.Count; currRow++)
                    {
                        scatter.Points.Add(new ScatterPoint(currRow, Convert.ToDouble(QcData.Rows[currRow][columnName].ToString())));
                    }
                    myModel.Series.Add(scatter);
                }
                myModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom});
                
            }
            else
            {
                DateTimeAxis xAxis = new DateTimeAxis
                {
                    Position = AxisPosition.Bottom,
                    StringFormat = "yyy-MM-dd_HH:mm",
                    Title = "Acquisition date and time",
                    MinorIntervalType = DateTimeIntervalType.Auto,
                    IntervalType = DateTimeIntervalType.Auto,
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.None,
                    Angle = 45,
                };

                foreach (string columnName in selected)
                {
                    var scatter = new FunctionSeries();
                    scatter.Title = columnName;
                    scatter.MarkerType = MarkerType.Circle;
                    scatter.MarkerSize = 4.0;
                    scatter.LineStyle = LineStyle.None;

                    for (int currRow = 0; currRow < QcData.Rows.Count; currRow++)
                    {
                        scatter.Points.Add(new DataPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(QcData.Rows[currRow]["DateAcquired"].ToString())), Convert.ToDouble(QcData.Rows[currRow][columnName].ToString())));
                    }
                    myModel.Series.Add(scatter);
                }
                myModel.Axes.Add(xAxis);
            }

            for (int i = 0; i < myModel.Axes.Count; i++) myModel.Axes[i].MajorGridlineStyle = LineStyle.Solid;

            myModel.Axes[0].MinimumPadding = 0.05;
            myModel.Axes[1].MinimumPadding = 0.05;

            if (yMinFixed.Checked)
            {
                myModel.Axes[0].Minimum = Convert.ToDouble(yMinFixedValue.Text);
            }

            if (yMaxFixed.Checked)
            {
                myModel.Axes[0].Maximum = Convert.ToDouble(yMaxFixedValue.Text);
            }

            this.plotView1.Model = myModel;
        }

        private void axisTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void linearYScale_CheckedChanged(object sender, EventArgs e)
        {
            if (linearYScale.Checked)
            {
                logYScaleBase.Enabled = false;
            }
            else
            {
                logYScaleBase.Enabled = true;
            }
            UpdateChart();
        }

        private void logYScaleBase_TextChanged(object sender, EventArgs e)
        {
            double i;
            bool success = double.TryParse(Convert.ToString(logYScaleBase.Text), out i);

            if (!success || i <= 1 || logYScaleBase.Text == "")
            {
                logYScaleBase.BackColor = Color.Red;
            }
            else
            {
                logYScaleBase.BackColor = Color.White;
                UpdateChart();
            }
        }

        private void yMinAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (yMinAuto.Checked)
            {
                yMinFixedValue.Enabled = false;
            }
            else
            {
                yMinFixedValue.Enabled = true;
            }
            
            UpdateChart();
        }

        private void yMaxAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (yMaxAuto.Checked)
            {
                yMaxFixedValue.Enabled = false;
            }
            else
            {
                yMaxFixedValue.Enabled = true;
            }

            UpdateChart();
        }

        private void yMinFixedValue_TextChanged(object sender, EventArgs e)
        {
            double i;
            bool success = double.TryParse(Convert.ToString(yMinFixedValue.Text), out i);

            if (!success || yMinFixedValue.Text == "")
            {
                yMinFixedValue.BackColor = Color.Red;
            }
            else
            {
                yMinFixedValue.BackColor = Color.White;
            }
        }

        private void yMaxFixedValue_TextChanged(object sender, EventArgs e)
        {
            double i;
            bool success = double.TryParse(Convert.ToString(yMaxFixedValue.Text), out i);

            if (!success || yMaxFixedValue.Text == "")
            {
                yMaxFixedValue.BackColor = Color.Red;
            }
            else
            {
                yMaxFixedValue.BackColor = Color.White;
            }
        }

        private void yMinFixedValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Return)
            {
                label1.Focus();
            }
        }

        private void yMaxFixedValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Return)
            {
                label1.Focus();
            }
        }

        private void yMinFixedValue_Leave(object sender, EventArgs e)
        {
            if (yMinFixedValue.BackColor == Color.White)
            {
                UpdateChart();
            }
        }

        private void yMaxFixedValue_Leave(object sender, EventArgs e)
        {
            if (yMinFixedValue.BackColor == Color.White)
            {
                UpdateChart();
            }
        }
    }
}
