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
using System.Reflection;
using RawTools.QC;
using RawTools.Utilities;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.FilterEnums;
using RawTools.Data.Containers;
using ThermoFisher.CommonCore.BackgroundSubtraction;
using RawToolsViz.Data;
using RawToolsViz.Resources;


namespace RawToolsViz
{
    using OxyPlot;
    using OxyPlot.Series;
    using OxyPlot.Axes;
    using OxyPlot.Annotations;
    using PresentationControls;

    public partial class RawFileViz : Form
    {
        IRawDataPlus RawData;
        IRawFileThreadManager rawFileThreadManager;
        IChromatogramDataPlus ChromatogramData;
        Spectrum spectrum;
        PlotModel chromatogram;
        int TotalNumScans;
        int FirstScan, LastScan;
        double TotalTime;
        int currentScan;
        int y, x;

        double currentRT
        {
            get
            {
                return RawData.RetentionTimeFromScanNumber(currentScan);
            }
        }

        public RawFileViz()
        {

        }

        public RawFileViz(string rawFile)
        {
            this.InitializeComponent();

            #region initialize fields and containers

            nextScanButton.Text = Char.ConvertFromUtf32(0x2192);
            previousScanButton.Text = Char.ConvertFromUtf32(0x2190);

            rawFileThreadManager = RawFileReaderFactory.CreateThreadManager(rawFile);

            RawData = rawFileThreadManager.CreateThreadAccessor();

            RawData.SelectMsData();

            TotalNumScans = RawData.RunHeaderEx.SpectraCount;

            FirstScan = RawData.RunHeader.FirstSpectrum;

            LastScan = RawData.RunHeader.LastSpectrum;

            currentScan = FirstScan;

            TotalTime = RawData.RunHeader.EndTime;

            this.Text = "ParseDataViz - " + RawData.FileName;

            totalScansLabel.Text = String.Format("/ {0}", TotalNumScans);

            splitContainer1.SplitterDistance = this.Size.Width - 300;

            UpdateChromatogramData();

            InitializeChromatogramPlot();

            ChroMsLevelComboBox.SelectedIndex = 0;

            initializeMassSpecData();

            initializeMassSpectrum();

            scanNumber.Text = FirstScan.ToString();
            
            y = 0;

            x = 1;


            #endregion

            //#region initial chromatogram

            this.plotViewChromatogram.Model = chromatogram;

            UpdateChromatogramPlot();
        }

        private void InitializeChromatogramPlot()
        {
            chromatogram = new PlotModel();

            chromatogram.Axes.Add(new LinearAxis { Position = AxisPosition.Left , AbsoluteMinimum = 0});
            chromatogram.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom , AbsoluteMinimum = 0, AbsoluteMaximum = TotalTime});
            chromatogram.Axes[x].MajorGridlineStyle = LineStyle.Solid;
            chromatogram.Axes[y].MajorGridlineStyle = LineStyle.Solid;
            //chromatogram.Axes[y].IsZoomEnabled = false;

            var currentScanLine = new LineAnnotation()
            {
                Type = LineAnnotationType.Vertical,
                X = 0,
                Color = OxyColors.Black
            };

            var mouseLocationLine = new LineAnnotation()
            {
                Type = LineAnnotationType.Vertical,
                X = 0,
                Color = OxyColors.DimGray
            };

            var chroSeries = new AreaSeries();
            chroSeries.Color = Colors.ColorBrewer8ClassSet2(255).Last();

            for (int i = 0; i < ChromatogramData.PositionsArray[0].Length; i++)
            {
                chroSeries.Points.Add(new DataPoint(ChromatogramData.PositionsArray[0][i], ChromatogramData.IntensitiesArray[0][i]));
            }

            chromatogram.Series.Add(chroSeries);
            
            double max = (from i in ChromatogramData.IntensitiesArray[0] select i).Max() * 1.5;

            if (max <= 0) max = 1000;

            chromatogram.Axes[y].AbsoluteMaximum = max;
            chromatogram.Axes[y].Maximum = chromatogram.Axes[y].AbsoluteMaximum;
            chromatogram.Axes[y].MajorStep = chromatogram.Axes[y].Maximum / 3;
            chromatogram.Axes[y].StringFormat = "0.00E00";

            chromatogram.Axes[y].TransformChanged += (object sender, EventArgs e) =>
            {
                chromatogram.Axes[y].MajorStep = (chromatogram.Axes[y].ActualMaximum - chromatogram.Axes[y].ActualMinimum) / 3;
            };

            plotViewChromatogram.MouseEnter += (s, e) =>
            {
                plotViewChromatogram.Cursor = Cursors.Cross;
            };
            
            chromatogram.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    double rt = chromatogram.Axes[x].InverseTransform(e.Position.X, e.Position.Y, chromatogram.Axes[y]).X;

                    scanNumber.Text = RawData.ScanNumberFromRetentionTime(rt).ToString();

                    currentScanLine.X = RawData.RetentionTimeFromScanNumber(Convert.ToInt32(scanNumber.Text));

                    e.Handled = true;
                    chromatogram.InvalidatePlot(false);
                }
            };
            
            chromatogram.MouseMove += (s, e) =>
            {
                try
                {
                    mouseLocationLine.X = currentScanLine.InverseTransform(e.Position).X;
                }
                catch (Exception)
                {
                    //do nothing
                }

                e.Handled = false;
                chromatogram.InvalidatePlot(false);

            };

            
            chromatogram.MouseUp += (s, e) =>
            {
                plotViewChromatogram.Cursor = Cursors.Cross;
                chromatogram.InvalidatePlot(true);
                e.Handled = false;
            };

            chromatogram.MouseLeave += (s, e) =>
            {
                mouseLocationLine.X = 0;

                chromatogram.InvalidatePlot(false);
                e.Handled = true;
            };
            
            chromatogram.Annotations.Add(currentScanLine);
            chromatogram.Annotations.Add(mouseLocationLine);
            
            currentScanLine.MouseDown += (s, e) =>
            {
                if (e.ChangedButton != OxyMouseButton.Left)
                {
                    return;
                }
                
                plotViewChromatogram.Cursor = Cursors.SizeWE;
                chromatogram.InvalidatePlot(false);
                e.Handled = true;
            };

            currentScanLine.MouseMove += (s, e) =>
            {
                double rt = currentScanLine.InverseTransform(e.Position).X;

                currentScanLine.X = rt;
                mouseLocationLine.X = rt;

                scanNumber.Text = RawData.ScanNumberFromRetentionTime(rt).ToString();
                
                chromatogram.InvalidatePlot(false);
                e.Handled = true;
            };

            currentScanLine.MouseUp += (s, e) =>
            {
                plotViewChromatogram.Cursor = Cursors.Cross;
                e.Handled = true;
            };
            

            chromatogram.InvalidatePlot(true);
        }

        private void UpdateChromatogramPlot()
        {
            var chroSeries = new AreaSeries();
            chroSeries.Color = Colors.ColorBrewer8ClassSet2(255).Last();

            for (int i = 0; i < ChromatogramData.PositionsArray[0].Length; i++)
            {
                chroSeries.Points.Add(new DataPoint(ChromatogramData.PositionsArray[0][i], ChromatogramData.IntensitiesArray[0][i]));
            }

            chromatogram.Series.Clear();
            chromatogram.Series.Add(chroSeries);
            
            double max = (from i in ChromatogramData.IntensitiesArray[0] select i).Max() * 1.5;

            if (max <= 0) max = 1000;

            (chromatogram.Annotations[0] as LineAnnotation).X = RawData.RetentionTimeFromScanNumber(Convert.ToInt32(scanNumber.Text));

            chromatogram.Axes[y].AbsoluteMaximum = max;
            chromatogram.Axes[y].Maximum = chromatogram.Axes[y].AbsoluteMaximum;

            chromatogram.InvalidatePlot(true);

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

        private void updateAll()
        {
            updateScanHeaderTextBox();
            updateTrailerExtraTextBox();
            updateInstLogTextBox();
            updateMassSpectrum();
        }

        private void updateScanHeaderTextBox()
        {
            var scanEvent = RawData.GetScanEventStringForScanNumber(Convert.ToInt32(scanNumber.Text));
            scanFilterTextBox.Text = String.Format("Scan event: {0}{1}TIC: {2}", scanEvent, Environment.NewLine, spectrum.SummedIntensity.ToString("E3"));
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

        private void autoAdjustYScale(Axis xAxis, Axis yAxis)
        {
            if (!yMaxFixed.Checked)
            {
                List<double> intensities = new List<double>();

                double xMin = xAxis.ActualMinimum;
                double xMax = xAxis.ActualMaximum;

                double yMaxInRange = (from x in spectrum.Ions
                                      where x.Mass >= xMin && x.Mass <= xMax
                                      select x.Intensity)
                                      .Max();

                yAxis.Maximum = yMaxInRange + 0.05 * yMaxInRange;
            }
        }

        private void UpdateChromatogramData()
        {
            ChromatogramTraceSettings traceSettings;
            MassOptions tolerance = null;
            string msFilter;

            if (ChroMsLevelComboBox.Text == "MS1" | ChroMsLevelComboBox.SelectedIndex == -1) msFilter = "ms";
            else if (ChroMsLevelComboBox.Text == "MS2") msFilter = "ms2";
            else if (ChroMsLevelComboBox.Text == "MS3") msFilter = "ms3";
            else msFilter = String.Empty;

            if (ChroTICRadioButton.Checked) traceSettings = new ChromatogramTraceSettings(TraceType.TIC);
            else if (ChroBPRadioButton.Checked) traceSettings = new ChromatogramTraceSettings(TraceType.BasePeak);
            else
            {
                if (ChroXICmzTextBox.Text.IsNullOrEmpty() || ChroXICmzTextBox.BackColor == Color.Red || ChroXICmzTextBox.Text == "")
                {
                    traceSettings = new ChromatogramTraceSettings(TraceType.TIC);
                }
                else
                {
                    double da = Convert.ToDouble(ChroXICmzTextBox.Text);

                    traceSettings = new ChromatogramTraceSettings(TraceType.MassRange)
                    {
                        MassRanges = new[] { new Range(da, da) },
                    };

                    if (!chroXICToleranceTextBox.Text.IsNullOrEmpty() && !(chroXICToleranceTextBox.BackColor == Color.Red) && !(chroXICToleranceTextBox.Text == ""))
                    {
                        tolerance = new MassOptions() { Tolerance = Convert.ToDouble(chroXICToleranceTextBox.Text) };

                        if (chroMassTolPPM.Checked) tolerance.ToleranceUnits = ToleranceUnits.ppm;
                        else if (chroMassTolDa.Checked) tolerance.ToleranceUnits = ToleranceUnits.amu;
                        else tolerance.ToleranceUnits = ToleranceUnits.mmu;
                    }
                }
            }

            traceSettings.Filter = msFilter;

            IChromatogramSettingsEx[] allSettings = { traceSettings };

            ChromatogramData = RawData.GetChromatogramDataEx(allSettings, -1, -1, tolerance);
        }

        private void initializeMassSpectrum()
        {
            int y = 0;
            int x = 1;
            
            var spectrumModel = new PlotModel();

            spectrumModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
            spectrumModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });

            for (int i = 0; i < spectrumModel.Axes.Count; i++)
            {
                spectrumModel.Axes[i].MajorGridlineStyle = LineStyle.Solid;
            }

            var scatter = new LineSeries();
            scatter.MarkerSize = 0;
            scatter.StrokeThickness = 1;
            scatter.Color = OxyColors.Black;

            foreach (Ion ion in this.spectrum.Ions)
            {
                var basePoint = new DataPoint(ion.Mass, 0);
                var topPoint = new DataPoint(ion.Mass, ion.Intensity);
                scatter.Points.Add(basePoint);
                scatter.Points.Add(topPoint);
                scatter.Points.Add(basePoint);
            }

            spectrumModel.Series.Add(scatter);
            
            for (int i = 0; i < spectrumModel.Series.Count; i++)
            {
                spectrumModel.Series[i].TrackerFormatString = "{0}\n{1}: {2:0.###}\n{3}: {4}\nFile: {RawFile}";
            }

            if (yMinFixed.Checked)
            {
                spectrumModel.Axes[y].Minimum = Convert.ToDouble(yMinFixedValue.Text);
            }
            else
            {
                spectrumModel.Axes[y].Minimum = 0;
            }

            if (yMaxFixed.Checked)
            {
                spectrumModel.Axes[y].Maximum = Convert.ToDouble(yMaxFixedValue.Text);
            }
            else
            {
                spectrumModel.Axes[y].MaximumPadding = 0.05;
            }
            
            if (!String.IsNullOrEmpty(xAxisLabel.Text)) spectrumModel.Axes[x].Title = xAxisLabel.Text;
            if (!String.IsNullOrEmpty(yAxisLabel.Text)) spectrumModel.Axes[y].Title = yAxisLabel.Text;
            spectrumModel.Axes[x].AbsoluteMaximum = this.spectrum.MaximumMass;
            spectrumModel.Axes[x].AbsoluteMinimum = this.spectrum.MinimumMass;
            spectrumModel.Axes[y].AbsoluteMinimum = 0;
            spectrumModel.Axes[y].AbsoluteMaximum = this.spectrum.MaximumIntensity * 1.5;

            this.plotViewMassSpectrum.Model = spectrumModel;
        }

        private void updateMassSpectrum()
        {
            int y = 0;
            int x = 1;
            /*
            if ((yMinFixed.Checked && yMinFixedValue.BackColor == Color.Red)
                || yMaxFixed.Checked && yMaxFixedValue.BackColor == Color.Red
                || scanNumber.BackColor == Color.Red)
            {
                return;
            }

            int scan;

            if (!int.TryParse(scanNumber.Text, out scan))
            {
                return;
            }
            */

            plotViewMassSpectrum.Model.Series.Clear();

            var scatter = new LineSeries();
            scatter.MarkerSize = 0;
            scatter.StrokeThickness = 1;
            scatter.Color = OxyColors.Black;
            
            foreach (Ion ion in spectrum.Ions)
            {
                var basePoint = new DataPoint(ion.Mass, 0);
                var topPoint = new DataPoint(ion.Mass, ion.Intensity);
                scatter.Points.Add(basePoint);
                scatter.Points.Add(topPoint);
                scatter.Points.Add(basePoint);
            }

            plotViewMassSpectrum.Model.Series.Add(scatter);

            for (int i = 0; i < plotViewMassSpectrum.Model.Series.Count; i++)
            {
                plotViewMassSpectrum.Model.Series[i].TrackerFormatString = "{0}\n{1}: {2:0.###}\n{3}: {4}\nFile: {RawFile}";
            }

            if (yMinFixed.Checked)
            {
                plotViewMassSpectrum.Model.Axes[y].Minimum = Convert.ToDouble(yMinFixedValue.Text);
            }
            else
            {
                plotViewMassSpectrum.Model.Axes[y].Minimum = 0;
            }

            if (yMaxFixed.Checked)
            {
                plotViewMassSpectrum.Model.Axes[y].Maximum = Convert.ToDouble(yMaxFixedValue.Text);
            }
            else
            {
                plotViewMassSpectrum.Model.Axes[y].MaximumPadding = 0.05;
            }

            //myModel.Axes[y].IsZoomEnabled = false;

            if (!String.IsNullOrEmpty(xAxisLabel.Text)) plotViewMassSpectrum.Model.Axes[x].Title = xAxisLabel.Text;
            if (!String.IsNullOrEmpty(yAxisLabel.Text)) plotViewMassSpectrum.Model.Axes[y].Title = yAxisLabel.Text;
            plotViewMassSpectrum.Model.Axes[x].AbsoluteMaximum = spectrum.MaximumMass;
            //plotViewMassSpectrum.Model.Axes[x].Maximum = spectrum.MaximumMass;
            plotViewMassSpectrum.Model.Axes[x].AbsoluteMinimum = spectrum.MinimumMass;
            //plotViewMassSpectrum.Model.Axes[x].Minimum = spectrum.MinimumMass;
            plotViewMassSpectrum.Model.Axes[y].AbsoluteMinimum = 0;
            //plotViewMassSpectrum.Model.Axes[y].Minimum = 0;
            plotViewMassSpectrum.Model.Axes[y].AbsoluteMaximum = spectrum.MaximumIntensity * 1.5;
            //plotViewMassSpectrum.Model.Axes[y].Maximum = spectrum.MaximumIntensity * 1.5;

            plotViewMassSpectrum.Model.ZoomAllAxes(1e-6);

            plotViewMassSpectrum.Model.InvalidatePlot(true);
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
            
            updateMassSpectrum();
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

            updateMassSpectrum();
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
                updateMassSpectrum();
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
                updateMassSpectrum();
            }
        }

        private void yMinFixedValue_Leave(object sender, EventArgs e)
        {
            if (yMinFixedValue.BackColor == Color.White)
            {
                updateMassSpectrum();
            }
        }

        private void yMaxFixedValue_Leave(object sender, EventArgs e)
        {
            if (yMinFixedValue.BackColor == Color.White)
            {
                updateMassSpectrum();
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

        private void exportButton_Click(object sender, EventArgs e)
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

                if (saveParameters.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }

                using (var stream = File.Create(saveParameters.FileName))
                {
                    var exporter = new SvgExporter { Width = Convert.ToInt32(exportWidthValue.Text.ToString()),
                        Height = Convert.ToInt32(exportHeightValue.Text.ToString()) };
                    exporter.Export(plotViewMassSpectrum.Model, stream);
                }
            }
            else if (exportAsComboBox.Text == "PDF")
            {
                SaveFileDialog saveParameters = new SaveFileDialog();
                saveParameters.Filter = ".pdf files|*.pdf";
                saveParameters.Title = "Export figure as .pdf";

                if (saveParameters.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }

                using (var stream = File.Create(saveParameters.FileName))
                {
                    var exporter = new PdfExporter { Width = Convert.ToInt32(exportWidthValue.Text.ToString()),
                        Height = Convert.ToInt32(exportHeightValue.Text.ToString()) };
                    exporter.Export(plotViewMassSpectrum.Model, stream);
                }
            }
            else if (exportAsComboBox.Text == "PNG")
            {
                SaveFileDialog saveParameters = new SaveFileDialog();
                saveParameters.Filter = "PNG files|*.png";
                saveParameters.Title = "Export figure as .png";

                if (saveParameters.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }

                using (var stream = File.Create(saveParameters.FileName))
                {
                    var exporter = new OxyPlot.WindowsForms.PngExporter
                    {
                        Width = Convert.ToInt32(exportWidthValue.Text.ToString()),
                        Height = Convert.ToInt32(exportHeightValue.Text.ToString())
                    };
                    exporter.Export(plotViewMassSpectrum.Model, stream);
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
            updateMassSpectrum();
        }

        private void yAxisLabel_TextChanged(object sender, EventArgs e)
        {
            updateMassSpectrum();
        }

        private void nextScanButton_Click(object sender, EventArgs e)
        {
            int scan;
            bool success = int.TryParse(Convert.ToString(scanNumber.Text), out scan);

            if (success)
            {
                scanNumber.Text = (scan + 1).ToString();
            }
        }

        private void previousScanButton_Click(object sender, EventArgs e)
        {
            int scan;
            bool success = int.TryParse(Convert.ToString(scanNumber.Text), out scan);

            if (success)
            {
                scanNumber.Text = (scan - 1).ToString();
            }
        }

        private void initializeMassSpecData()
        {
            var massAnalyzer = RawData.GetScanEventForScanNumber(FirstScan).MassAnalyzer;

            if (massAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
            {
                spectrum = new Spectrum(RawData.GetCentroidStream(FirstScan, false));
            }
            else
            {
                spectrum = new Spectrum(RawData.GetSegmentedScanFromScanNumber(FirstScan, null));
            }
        }

        private void scanNumber_TextChanged(object sender, EventArgs e)
        {

            if (!validateScanNumText(scanNumber.Text))
            {
                scanNumber.BackColor = Color.Red;
                scanFilterTextBox.Clear();
                trailerExtraTextBox.Clear();
                instLogTextBox.Clear();
                plotViewMassSpectrum.Model = new PlotModel();
            }
            else
            {
                int scan = Convert.ToInt32(scanNumber.Text);

                var massAnalyzer = RawData.GetScanEventForScanNumber(scan).MassAnalyzer;

                if (massAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
                {
                    spectrum = new Spectrum(RawData.GetCentroidStream(scan, false));
                }
                else
                {
                    spectrum = new Spectrum(RawData.GetSegmentedScanFromScanNumber(scan, null));
                }

                scanNumber.BackColor = Color.White;
                updateAll();
                UpdateChromatogramPlot();
            }
        }

        private bool validateScanNumText(string scan)
        {
            int i;
            bool success = int.TryParse(Convert.ToString(scan), out i);

            return success && scanNumber.Text != "" && i >= FirstScan && i <= LastScan;
        }

        private void chromatogramType_checkChanged(object sender, EventArgs e)
        {
            UpdateChromatogramData();
            UpdateChromatogramPlot();
        }

        private void ChroXICmzTextBox_TextChanged(object sender, EventArgs e)
        {
            double i;
            bool success = double.TryParse(Convert.ToString(ChroXICmzTextBox.Text), out i);

            if (!success || ChroXICmzTextBox.Text == "")
            {
                ChroXICmzTextBox.BackColor = Color.Red;
            }
            else
            {
                ChroXICmzTextBox.BackColor = Color.White;
                UpdateChromatogramData();
                UpdateChromatogramPlot();
            }
        }

        private void chroXICToleranceTextBox_TextChanged(object sender, EventArgs e)
        {
            double i;
            bool success = double.TryParse(Convert.ToString(chroXICToleranceTextBox.Text), out i);

            if (!success || chroXICToleranceTextBox.Text == "")
            {
                chroXICToleranceTextBox.BackColor = Color.Red;
            }
            else
            {
                chroXICToleranceTextBox.BackColor = Color.White;
                UpdateChromatogramData();
                UpdateChromatogramPlot();
            }
        }

        private void chromatogramTolType_checkChanged(object sender, EventArgs e)
        {
            UpdateChromatogramData();
            UpdateChromatogramPlot();
        }

        private void ChroMsLevelComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateChromatogramData();
            UpdateChromatogramPlot();
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new RawToolsViz.HelpWindows.RawfileVizHelp().ShowDialog();
        }

        private bool validateScanNum(int scan)
        {
            return scan >= FirstScan || scan <= LastScan;
        }
        
    }
}
