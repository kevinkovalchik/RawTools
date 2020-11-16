using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RawTools.QC;
using RawTools.Utilities;
using RawToolsViz.Resources;


namespace RawToolsViz
{
    using OxyPlot;
    using OxyPlot.Series;
    using OxyPlot.Axes;

    public partial class ParseDataViz : Form
    {
        DataTable QcData;

        public ParseDataViz()
        {

        }

        public ParseDataViz(string pathToQcCsvFile)
        {
            this.InitializeComponent();
            QcData = ReadWrite.LoadDataTable(pathToQcCsvFile, '\t');
            
            foreach (DataColumn d in QcData.Columns)
            {
                double i;
                if (Double.TryParse(QcData.Rows[0][d.ColumnName].ToString(), out i))
                {
                    checkedListBox1.Items.Add(d.ColumnName);
                    axisTypeComboBox.Items.Add(d.ColumnName);
                }
            }
            axisTypeComboBox.SelectedIndex = 0;
            this.Text = "ParseDataViz - " + pathToQcCsvFile;
        }

        private void checkBoxComboBox1_CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void UpdateChart()
        {
            int y = 0;
            int x = 1;

            if ((yMinFixed.Checked && yMinFixedValue.BackColor == Color.Red)
                || yMaxFixed.Checked && yMaxFixedValue.BackColor == Color.Red)
            {
                return;
            }

            List<string> selected = new List<string>();

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                //if (!(from x in checkBoxComboBox1.CheckBoxItems select x.Text).Contains(data.ColumnName)) return;

                if (checkedListBox1.GetItemChecked(i))
                {
                    selected.Add(checkedListBox1.Items[i].ToString());
                }
            }

            var myModel = new PlotModel();

            myModel.SetDefaultColorsToColorBrewer8ClassSet2();

            myModel.LegendPosition = LegendPosition.BottomCenter;
            myModel.LegendPlacement = LegendPlacement.Outside;
            myModel.LegendOrientation = LegendOrientation.Horizontal;


            if (logYScale.Checked)
            {
                myModel.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Left, Base = Convert.ToDouble(logYScaleBase.Text) });
            }
            else
            {
                myModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
            }

            foreach (string columnName in selected)
            {
                var scatter = new ScatterSeries();
                scatter.Title = columnName;
                scatter.MarkerType = MarkerType.Circle;
                scatter.MarkerSize = 4.0;

                for (int currRow = 0; currRow < QcData.Rows.Count; currRow++)
                {
                    scatter.Points.Add(new ScatterPoint(Convert.ToDouble(QcData.Rows[currRow][axisTypeComboBox.Text].ToString()), Convert.ToDouble(QcData.Rows[currRow][columnName].ToString())));
                }
                myModel.Series.Add(scatter);
            }
            myModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });

            for (int i = 0; i < myModel.Axes.Count; i++)
            {
                myModel.Axes[i].MajorGridlineStyle = LineStyle.Solid;
            }

            for (int i = 0; i < myModel.Series.Count; i++)
            {
                myModel.Series[i].TrackerFormatString = "{0}\n{1}: {2:0.###}\n{3}: {4}\nFile: {RawFile}";
            }

            if (yMinFixed.Checked)
            {
                myModel.Axes[y].Minimum = Convert.ToDouble(yMinFixedValue.Text);
            }

            if (yMaxFixed.Checked)
            {
                myModel.Axes[y].Maximum = Convert.ToDouble(yMaxFixedValue.Text);
            }

            myModel.Axes[y].MinimumPadding = 0.05;
            myModel.Axes[y].MaximumPadding = 0.05;

            if (!String.IsNullOrEmpty(xAxisLabel.Text)) myModel.Axes[x].Title = xAxisLabel.Text;
            if (!String.IsNullOrEmpty(yAxisLabel.Text)) myModel.Axes[y].Title = yAxisLabel.Text;

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

            if (!success || yMinFixedValue.Text == "" || yMinFixedValue.Text == yMaxFixedValue.Text)
            {
                yMinFixedValue.BackColor = Color.Red;
            }
            else
            {
                yMinFixedValue.BackColor = Color.White;
                UpdateChart();
            }
        }

        private void yMaxFixedValue_TextChanged(object sender, EventArgs e)
        {
            double i;
            bool success = double.TryParse(Convert.ToString(yMaxFixedValue.Text), out i);

            if (!success || yMaxFixedValue.Text == "" || yMinFixedValue.Text == yMaxFixedValue.Text)
            {
                yMaxFixedValue.BackColor = Color.Red;
            }
            else
            {
                yMaxFixedValue.BackColor = Color.White;
                UpdateChart();
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

        private void exportWidthValue_TextChanged(object sender, EventArgs e)
        {
            int i;
            bool success = int.TryParse(Convert.ToString(exportWidthValue.Text), out i);

            if (!success || exportWidthValue.Text == "")
            {
                exportWidthValue.BackColor = Color.Red;
            }
            else
            {
                exportWidthValue.BackColor = Color.White;
            }
        }

        private void exportHeightValue_TextChanged(object sender, EventArgs e)
        {
            int i;
            bool success = int.TryParse(Convert.ToString(exportHeightValue.Text), out i);

            if (!success || exportHeightValue.Text == "")
            {
                exportHeightValue.BackColor = Color.Red;
            }
            else
            {
                exportHeightValue.BackColor = Color.White;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (exportHeightValue.BackColor == Color.Red)
            {
                const string message = "Please choose an integer value for the export height.";
                const string caption = "Error";
                var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Error);
                return;
            }

            if (exportWidthValue.BackColor == Color.Red)
            {
                const string message = "Please choose an integer value for the export width.";
                const string caption = "Error";
                var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Error);
                return;
            }

            if (exportAsComboBox.Text == "SVG")
            {
                SaveFileDialog saveParameters = new SaveFileDialog();
                saveParameters.Filter = ".svg files|*.svg";
                saveParameters.Title = "Export figure as .svg";
                saveParameters.ShowDialog();

                using (var stream = File.Create(saveParameters.FileName))
                {
                    var exporter = new SvgExporter { Width = Convert.ToInt32(exportWidthValue.Text.ToString()),
                        Height = Convert.ToInt32(exportHeightValue.Text.ToString()) };
                    exporter.Export(plotView1.Model, stream);
                }
            }
            else if (exportAsComboBox.Text == "PDF")
            {
                SaveFileDialog saveParameters = new SaveFileDialog();
                saveParameters.Filter = ".pdf files|*.pdf";
                saveParameters.Title = "Export figure as .pdf";
                saveParameters.ShowDialog();

                using (var stream = File.Create(saveParameters.FileName))
                {
                    var exporter = new PdfExporter { Width = Convert.ToInt32(exportWidthValue.Text.ToString()),
                        Height = Convert.ToInt32(exportHeightValue.Text.ToString()) };
                    exporter.Export(plotView1.Model, stream);
                }
            }
            else if (exportAsComboBox.Text == "PNG")
            {
                SaveFileDialog saveParameters = new SaveFileDialog();
                saveParameters.Filter = "PNG files|*.pdf";
                saveParameters.Title = "Export figure as .pdf";
                saveParameters.ShowDialog();

                using (var stream = File.Create(saveParameters.FileName))
                {
                    var exporter = new OxyPlot.WindowsForms.PngExporter
                    {
                        Width = Convert.ToInt32(exportWidthValue.Text.ToString()),
                        Height = Convert.ToInt32(exportHeightValue.Text.ToString())
                    };
                    exporter.Export(plotView1.Model, stream);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (ConfirmExit() == false)
            {
                e.Cancel = true;
            };
        }

        private bool ConfirmExit()
        {
            const string message = "Are you sure you wish to exit?";
            const string caption = "Exit";
            var result = MessageBox.Show(message, caption,
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                return true;
            else
                return false;
        }

        private void xAxisLabel_TextChanged(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void yAxisLabel_TextChanged(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void checkBoxComboBox1_CheckBoxCheckedChanged_1(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void axisTypeComboBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void xAxisLabel_TextChanged_1(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void yAxisLabel_TextChanged_1(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)(
                () => UpdateChart()));
        }
    }
}
