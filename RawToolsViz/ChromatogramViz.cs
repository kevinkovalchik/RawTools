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

    public partial class ChromatogramViz : Form
    {
        DataTable ChromatogramData;
        double minValue;

        public ChromatogramViz()
        {

        }

        public ChromatogramViz(string pathToChromatogramFile)
        {
            this.InitializeComponent();
            ChromatogramData = ReadWrite.LoadDataTable(pathToChromatogramFile, '\t');

            var columnNames = (from dc in ChromatogramData.Columns.Cast<DataColumn>()
                                    select dc.ColumnName).ToList();
            
            this.Text = "ChromatogramViz - " + pathToChromatogramFile;

            List<double> intensities = new List<double>();
            for (int currRow = 0; currRow < ChromatogramData.Rows.Count; currRow++)
                intensities.Add(Convert.ToDouble(ChromatogramData.Rows[currRow]["Intensity"].ToString()));
            minValue = intensities.Min();

            UpdateChart();
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
            
            var myModel = new PlotModel();
            
            if (logYScale.Checked)
            {
                myModel.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Left, Base = Convert.ToDouble(logYScaleBase.Text) });
            }
            else
            {
                myModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
            }

            var area = new AreaSeries();
            area.Color = Colors.ColorBrewer8ClassSet2(255).First();

            List<double> intensities = new List<double>();
            for (int currRow = 0; currRow < ChromatogramData.Rows.Count; currRow++)
                intensities.Add(Convert.ToDouble(ChromatogramData.Rows[currRow]["Intensity"].ToString()));
            double minValue = intensities.Min();

            for (int currRow = 0; currRow < ChromatogramData.Rows.Count; currRow++)
            {
                area.Points.Add(new DataPoint(Convert.ToDouble(ChromatogramData.Rows[currRow]["RetentionTime"].ToString()),
                    Convert.ToDouble(ChromatogramData.Rows[currRow]["Intensity"].ToString())));
                area.Points2.Add(new DataPoint(Convert.ToDouble(ChromatogramData.Rows[currRow]["RetentionTime"].ToString()), minValue));
            }

            myModel.Series.Add(area);

            myModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });

            for (int i = 0; i < myModel.Axes.Count; i++)
            {
                myModel.Axes[i].MajorGridlineStyle = LineStyle.Solid;
            }

            if (yMinFixed.Checked)
            {
                myModel.Axes[y].Minimum = Convert.ToDouble(yMinFixedValue.Text);
            }

            if (yMaxFixed.Checked)
            {
                myModel.Axes[y].Maximum = Convert.ToDouble(yMaxFixedValue.Text);
            }

            //myModel.Axes[y].IsZoomEnabled = false;

            myModel.Axes[y].MinimumPadding = 0;
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
                label3.Focus();
            }
        }

        private void yMaxFixedValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Return)
            {
                label3.Focus();
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
                saveParameters.Filter = "SVG files|*.svg";
                saveParameters.Title = "Export figure as .svg";
                saveParameters.ShowDialog();

                using (var stream = File.Create(saveParameters.FileName))
                {
                    var exporter = new SvgExporter
                    {
                        Width = Convert.ToInt32(exportWidthValue.Text.ToString()),
                        Height = Convert.ToInt32(exportHeightValue.Text.ToString())
                    };
                    exporter.Export(plotView1.Model, stream);
                }
            }
            else if (exportAsComboBox.Text == "PDF")
            {
                SaveFileDialog saveParameters = new SaveFileDialog();
                saveParameters.Filter = "PDF files|*.pdf";
                saveParameters.Title = "Export figure as .pdf";
                saveParameters.ShowDialog();

                using (var stream = File.Create(saveParameters.FileName))
                {
                    var exporter = new PdfExporter
                    {
                        Width = Convert.ToInt32(exportWidthValue.Text.ToString()),
                        Height = Convert.ToInt32(exportHeightValue.Text.ToString())
                    };
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

        private void checkBoxComboBox1_CheckBoxCheckedChanged_1(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void axisTypeComboBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void xAxisLabel_TextChanged(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void yAxisLabel_TextChanged(object sender, EventArgs e)
        {
            UpdateChart();
        }
    }
}
