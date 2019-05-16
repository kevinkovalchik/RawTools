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
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data;
using RawTools.Data.Containers;


namespace RawToolsViz
{
    using OxyPlot;
    using OxyPlot.Series;
    using OxyPlot.Axes;
    using PresentationControls;

    public partial class RawFileViz : Form
    {
        IRawDataPlus RawData;
        IRawFileThreadManager rawFileThreadManager;
        CentroidStreamData CentroidData;
        SegmentedScanData SegmentedScanData;
        SimpleCentroid SimpleCentroidData;
        int TotalNumScans;
        int FirstScan, LastScan;

        public RawFileViz()
        {

        }

        public RawFileViz(string rawFile)
        {
            this.InitializeComponent();

            rawFileThreadManager = RawFileReaderFactory.CreateThreadManager(rawFile);

            RawData = rawFileThreadManager.CreateThreadAccessor();

            RawData.SelectMsData();

            TotalNumScans = RawData.RunHeaderEx.SpectraCount;

            FirstScan = RawData.RunHeader.FirstSpectrum;

            LastScan = RawData.RunHeader.LastSpectrum;

            this.Text = "ParseDataViz - " + RawData.FileName;

            totalScans.Text = String.Format("/ {0}", TotalNumScans);

            splitContainer1.SplitterDistance = this.Size.Width - 300;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (ConfirmExit() == false)
            {
                e.Cancel = true;
            }
            else
	        {
                RawData.Dispose();
                rawFileThreadManager.Dispose();
            }
        }

        private void updateScanHeaderTextBox()
        {
            var scanEvent = RawData.GetScanEventStringForScanNumber(Convert.ToInt32(scanNumber.Text));
            scanFilterTextBox.Text = "Scan event: "+ scanEvent;
        }

        private void updateTrailerExtraTextBox()
        {
            var trailer = RawData.GetTrailerExtraInformation(Convert.ToInt32(scanNumber.Text));
            string text = "";

            for (int i = 0; i < trailer.Length; i++)
            {
                text += String.Format("{0} {1}{2}", trailer.Labels[i], trailer.Values[i], Environment.NewLine);
            }
            
            trailerExtraTextBox.Text = text;
        }

        private void updateInstLogTextBox()
        {
            var retTime = RawData.RetentionTimeFromScanNumber(Convert.ToInt32(scanNumber.Text));
            var statusLog = RawData.GetStatusLogForRetentionTime(retTime);

            string text = "";

            for (int i = 0; i < statusLog.Length; i++)
            {
                text += String.Format("{0} {1}{2}", statusLog.Labels[i], statusLog.Values[i], Environment.NewLine);
            }

            instLogTextBox.Text = text;
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
                /*
                for (int currRow = 0; currRow < ParseData.Rows.Count; currRow++)
                {
                    scatter.Points.Add(new ScatterPoint(Convert.ToDouble(ParseData.Rows[currRow][axisTypeComboBox.Text].ToString()), Convert.ToDouble(ParseData.Rows[currRow][columnName].ToString())));
                }*/
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

            myModel.Axes[y].IsZoomEnabled = false;

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

        private void scanNumber_TextChanged(object sender, EventArgs e)
        {
            int i;
            bool success = int.TryParse(Convert.ToString(exportHeightValue.Text), out i);

            if (!success || exportHeightValue.Text == "" || i < FirstScan || i > LastScan)
            {
                exportHeightValue.BackColor = Color.Red;
            }
            else
            {
                exportHeightValue.BackColor = Color.White;
                updateScanHeaderTextBox();
                updateTrailerExtraTextBox();
                updateInstLogTextBox();
            }
        }
    }
}
