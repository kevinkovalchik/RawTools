#
# This is a Shiny web application developed to observed RawTools quality control results. 
# You can run the application by clicking the 'Run App' button above.
#
# You can find out more about RawTools here: https://github.com/kevinkovalchik/RawTools.
#
#####################################################################################################
#libraries
library(shiny)
library(ggplot2)
library(reshape2)
library(RColorBrewer)
library(cowplot)

#####################################################################################################
#UI
ui = shinyUI(fluidPage(
  title = "RawTools",
  h2('RawTools Quality Control Data Plotting'),
  helpText('This simple application is designed to plot data obtained from the quality control functionality of RawTools.'),
  hr(),
  fluidRow(
    column(4,
           h4('1. Load an input file'),
           helpText('Select a quality control file output from RawTools as your input. This should be a CSV file that can be found in the directory specified by the -q flag in RawTools when using the QC module.'),
           fileInput('qc_file', label = NULL, accept=c('.csv')),
           tags$div(class="header", checked=NA,
                    tags$p("Example data can also be downloaded from the RawTools GitHub page. Right mouse click on the link and hit Save Link As..."),
                    tags$a(href="https://github.com/kevinkovalchik/RawTools/blob/master/documentation/QC_example-data/QcDataTable.csv", "Download Example Data")),
           helpText('The example data is derived from a RawTools quality control analysis of 140 individual injections on an Orbitrap Velos system. The data includes database search results from IdentiPy where the frequency of oxidation at methionine was calculated.')
    ),
    column(4,
           h4('2. Select the range of samples to plot on the x-axis.'),
           helpText('Adjust the values set the range of samples to plot (this is the x-axis range). The values in the boxes are the preset defaults. This can be adjusted in real-time once the plot is made to zoom in on regions of interest in the data.'),
           h6('first sample'),
           numericInput('xaxis_min', label = NULL, 1, 1, 10000),
           h6('last sample'),
           numericInput('xaxis_max', label = NULL, 10, 1, 10000),
           h6('x-axis tick interval'),
           numericInput('xaxis_int', label = NULL, 1, 1, 1000)
    ),
    column(4,
           h4('3. Optional: input custom values for the y-axis range.'),
           helpText('Pre-determined values are selected by default. Select the appropriate radio button to activate the custom ranges on the plot and enter values (minimum, maximum, interval). The values in the boxes are the preset defaults for the custom range. We recommend using the pre-determined values initially and using these as a starting input point before switching to custom (using the default custom values may cause odd outputs for some plots). These values can be adjusted in real-time.'),
           radioButtons('yaxis_radio', label = NULL, choices = list('Use Pre-determined Values' = 1, 'Use Custom Values' = 2), selected = 1),
           h6('minimum border'),
           numericInput('yaxis_min', label = NULL, 0, 0, 1e8),
           h6('maximum border'),
           numericInput('yaxis_max', label = NULL, 100, 1, 1e8),
           h6('y-axis tick interval'),
           numericInput('yaxis_int', label = NULL, 1000, 1, 1e8)
    )),
  hr(),
  fluidRow(
    column(8,
         h4('Select a plot to display'),
         helpText('Select the plot you are interested in seeing using the drop down menu.'),
         selectInput('qc_plot_id', label = NULL, choices = list('None' = 1,
                                                                'Total Analysis Time' = 2,
                                                                'Number of MS1 Scans' = 3,
                                                                'Number of MS2 Scans' = 4,
                                                                'Number of MS3 Scans' = 5,
                                                                'MS1 Scan Rate (Hz)' = 6,
                                                                'MS2 Scan Rate (Hz)' = 7,
                                                                'MS3 Scan Rate (Hz)' = 8,
                                                                'Duty Cycle Duration' = 9,
                                                                'MS2 Trigger Rate (per MS1)' = 10,
                                                                'MS1 Intensity' = 11,
                                                                'MS2 Intensity' = 12,
                                                                'Precursor Intensity' = 13,
                                                                'Fraction of MS2 Fragments Consuming 80% of Total Intensity' = 14,
                                                                'Electrospray Stability' = 15,
                                                                'Mass Analyzer Drift' = 16,
                                                                'Spectral Identification Rate' = 17,
                                                                'Digestion Efficiency' = 18,
                                                                'Missed Cleavages per Peptide' = 19,
                                                                'N-terminus Labeling Efficiency' = 20,
                                                                'Lysine Labeling Efficiency' = 21,
                                                                'Site Labeling Efficiency' = 22,
                                                                'MS1 Ion Injection Time' = 23,
                                                                'MS2 Ion Injection Time' = 24,
                                                                'MS3 Ion Injection Time' = 25,
                                                                'Peak Width at 10% Height' = 26,
                                                                'Peak Width at 50% Height' = 27,
                                                                'Peak Asymmetry at 10% Height' = 28,
                                                                'Peak Asymmetry at 50% Height' = 29,
                                                                'Column Peak Capacity' = 30,
                                                                'Time Before First Peak Elution' = 31,
                                                                'Time After Last Peak Elution' = 32,
                                                                'Fraction of Run With Eluting Peaks' = 33,
                                                                'Ratio of z = 3 to 2' = 34,
                                                                'Ratio of z = 4 to 2' = 35), selected = 1))),
  fluidRow(
    column(1),
    column(10, 
           plotOutput('qc_plots'),
    column(1))),
  h5('Plot Description'),
  fluidRow(
    column(12,
           textOutput('qc_descriptions'))),
  br(),
  br()
))

#####################################################################################################
#Server
server = shinyServer(function(input, output) {
  
  observeEvent(input$example_data, {
    qc_data = read.table('./RawTools/QcDataTable.csv', header = TRUE, sep = ',')
    qc_data$injection = factor(seq(1:nrow(qc_data)))
  })
  

  output$qc_plots = renderPlot({
    
    if (input$yaxis_radio == 1){
      qcFile = input$qc_file
     
      if (is.null(qcFile))
        return(NULL)
      
      qc_data = read.table(qcFile$datapath, header = TRUE, sep = ',')
      qc_data$injection = factor(seq(1:nrow(qc_data)))
      
      xaxis_first = input$xaxis_min
      xaxis_last = input$xaxis_max
      xaxis_interval = input$xaxis_int
      
      if (input$qc_plot_id == 2) {
        minBorder = 0
        maxBorder = round(max(qc_data$TotalAnalysisTime, na.rm = TRUE),2)
        intBorder = round((max(qc_data$TotalAnalysisTime, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TotalAnalysisTime)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Total Analysis Time (minutes)', title = 'Total Analysis Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 3) {
        minBorder = 0
        maxBorder = round(max(qc_data$NumMs1Scans, na.rm = TRUE),2)
        intBorder = round((max(qc_data$NumMs1Scans, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs1Scans)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Number of MS1 Scans', title = 'MS1 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 4) {
        minBorder = 0
        maxBorder = round(max(qc_data$NumMs2Scans, na.rm = TRUE),2)
        intBorder = round((max(qc_data$NumMs2Scans, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs2Scans)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Number of MS2 Scans', title = 'MS2 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 5) {
        minBorder = 0
        maxBorder = round(max(qc_data$NumMs3Scans, na.rm = TRUE),2)
        intBorder = round((max(qc_data$NumMs3Scans, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs3Scans)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Number of MS3 Scans', title = 'MS3 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 6) {
        minBorder = 0
        maxBorder = round(max(qc_data$Ms1ScanRate..s., na.rm = TRUE),2)
        intBorder = round((max(qc_data$Ms1ScanRate..s., na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms1ScanRate..s.)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'MS1 Scan Rate (Hz)', title = 'MS1 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 7) {
        minBorder = 0
        maxBorder = round(max(qc_data$Ms2ScanRate..s., na.rm = TRUE),2)
        intBorder = round((max(qc_data$Ms2ScanRate..s., na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms2ScanRate..s.)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'MS2 Scan Rate (Hz)', title = 'MS2 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 8) {
        minBorder = 0
        maxBorder = round(max(qc_data$Ms3ScanRate..s., na.rm = TRUE),2)
        intBorder = round((max(qc_data$Ms3ScanRate..s., na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), abs(Ms3ScanRate..s.))) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'MS3 Scan Rate (Hz)', title = 'MS3 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 9) {
        minBorder = 0
        maxBorder = round(max(qc_data$MeanDutyCycle.s., na.rm = TRUE),2)
        intBorder = round((max(qc_data$MeanDutyCycle.s., na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanDutyCycle.s.)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Mean Duty Cycle (s)', title = 'Duty Cycle') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 10) {
        minBorder = 0
        maxBorder = round(max(qc_data$MeanMs2TriggerRate..Ms1Scan., na.rm = TRUE),2)
        intBorder = round((max(qc_data$MeanMs2TriggerRate..Ms1Scan., na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanMs2TriggerRate..Ms1Scan.)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Mean Number of MS2 Triggered per MS1', title = 'MS2 Trigger Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 11) {
        minBorder = 0
        maxBorder = round(max(log10(qc_data$Ms1MedianSummedIntensity), na.rm = TRUE),2)
        intBorder = round((max(log10(qc_data$Ms1MedianSummedIntensity), na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms1MedianSummedIntensity))) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS1 Intensity)', title = 'MS1 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 12) {
        minBorder = 0
        maxBorder = round(max(log10(qc_data$Ms2MedianSummedIntensity), na.rm = TRUE),2)
        intBorder = round((max(log10(qc_data$Ms2MedianSummedIntensity), na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms2MedianSummedIntensity))) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS2 Intensity)', title = 'MS2 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 13) {
        minBorder = 0
        maxBorder = round(max(log10(qc_data$MedianPrecursorIntensity), na.rm = TRUE),2)
        intBorder = round((max(log10(qc_data$MedianPrecursorIntensity), na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(MedianPrecursorIntensity))) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'log10(Median Precursor Intensity)', title = 'Precursor Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 14) {
        minBorder = 0
        maxBorder = round(max(qc_data$MedianMs2PeakFractionConsumingTop80PercentTotalIntensity, na.rm = TRUE),2)
        intBorder = round((max(qc_data$MedianMs2PeakFractionConsumingTop80PercentTotalIntensity, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2PeakFractionConsumingTop80PercentTotalIntensity)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Fraction of MS2 Peaks', title = 'MS2 Peak Fraction') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 15) {
        minBorder = 0
        maxBorder = round(max(qc_data$EsiInstabilityFlags.count., na.rm = TRUE),2)
        intBorder = round((max(qc_data$EsiInstabilityFlags.count., na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), EsiInstabilityFlags.count.)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Number of Scans with >10% Deviation from Bordering', title = 'Electrospray Stability') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 16) {
        minBorder = 0
        maxBorder = round(max(qc_data$MedianMassDrift.ppm., na.rm = TRUE),2)
        intBorder = round((max(qc_data$MedianMassDrift.ppm., na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), abs(MedianMassDrift.ppm.))) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Mass Drift (ppm)', title = 'Mass Analyzer Drift') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 17) {
        minBorder = 0
        maxBorder = round(max(qc_data$IdentificationRate.IDs.Ms2Scan., na.rm = TRUE),2)
        intBorder = round((max(qc_data$IdentificationRate.IDs.Ms2Scan., na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdentificationRate.IDs.Ms2Scan.)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Spectra Identification Rate', title = 'Spectral Identification Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 18) {
        minBorder = 0
        maxBorder = round(max(qc_data$DigestionEfficiency, na.rm = TRUE),2)
        intBorder = round((max(qc_data$DigestionEfficiency, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), DigestionEfficiency)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Digestion Efficiency', title = 'Digestion Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 19) {
        minBorder = 0
        maxBorder = round(max(qc_data$MissedCleavageRate..PSM., na.rm = TRUE),2)
        intBorder = round((max(qc_data$MissedCleavageRate..PSM., na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MissedCleavageRate..PSM.)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Missed Cleavage Rate', title = 'Missed Cleavage Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 20) {
        minBorder = 0
        maxBorder = round(max(qc_data$ModificationFrequencyAtNTerm, na.rm = TRUE),2)
        intBorder = round((max(qc_data$ModificationFrequencyAtNTerm, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtNTerm)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'N-term Labeling Efficiency', title = 'N-term Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 21) {
        minBorder = 0
        maxBorder = round(max(qc_data$ModificationFrequencyAtK, na.rm = TRUE),2)
        intBorder = round((max(qc_data$ModificationFrequencyAtK, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtK)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Lysine Labeling Efficiency', title = 'Lysine Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 22) {
        minBorder = 0
        maxBorder = round(max(qc_data$ModificationFrequencyAtX, na.rm = TRUE),2)
        intBorder = round((max(qc_data$ModificationFrequencyAtX, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtX)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Modification Labeling Efficiency', title = 'Specified Modification Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 23) {
        minBorder = 0
        maxBorder = round(max(qc_data$MedianMsFillTime, na.rm = TRUE),2)
        intBorder = round((max(qc_data$MedianMsFillTime, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMsFillTime)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Median MS1 Ion Injection Time', title = 'MS1 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 24) {
        minBorder = 0
        maxBorder = round(max(qc_data$MedianMs2FillTime, na.rm = TRUE),2)
        intBorder = round((max(qc_data$MedianMs2FillTime, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2FillTime)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS2 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 25) {
        minBorder = 0
        maxBorder = round(max(qc_data$MedianMs3FillTime, na.rm = TRUE),2)
        intBorder = round((max(qc_data$MedianMs3FillTime, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs3FillTime)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS3 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 26) {
        minBorder = 0
        maxBorder = round(max(qc_data$WidthAt10.H.s., na.rm = TRUE),2)
        intBorder = round((max(qc_data$WidthAt10.H.s., na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt10.H.s.)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Peak Width at 10% Height', title = 'Peak Width at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 27) {
        minBorder = 0
        maxBorder = round(max(qc_data$WidthAt50.H.s., na.rm = TRUE),2)
        intBorder = round((max(qc_data$WidthAt50.H.s., na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt50.H.s.)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Peak Width at 50% Height', title = 'Peak Width at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 28) {
        minBorder = 0
        maxBorder = round(max(qc_data$AsymmetryAt10.H, na.rm = TRUE),2)
        intBorder = round((max(qc_data$AsymmetryAt10.H, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt10.H)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 10% Height', title = 'Peak Asymmetry at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 29) {
        minBorder = 0
        maxBorder = round(max(qc_data$AsymmetryAt50.H, na.rm = TRUE),2)
        intBorder = round((max(qc_data$AsymmetryAt50.H, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt50.H)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 50% Height', title = 'Peak Asymmetry at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 30) {
        minBorder = 0
        maxBorder = round(max(qc_data$PeakCapacity, na.rm = TRUE),2)
        intBorder = round((max(qc_data$PeakCapacity, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), PeakCapacity)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Column Peak Capacity', title = 'Column Peak Capacity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 31) {
        minBorder = 0
        maxBorder = round(max(qc_data$TimeBeforeFirstExceedanceOfTotalIntensityQ1, na.rm = TRUE),2)
        intBorder = round((max(qc_data$TimeBeforeFirstExceedanceOfTotalIntensityQ1, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeBeforeFirstExceedanceOfTotalIntensityQ1)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time Before First Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 32) {
        minBorder = 0
        maxBorder = round(max(qc_data$TimeAfterLastExceedanceOfTotalIntensityQ1, na.rm = TRUE),2)
        intBorder = round((max(qc_data$TimeAfterLastExceedanceOfTotalIntensityQ1, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), PeakCapacity)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time After Last Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 33) {
        minBorder = 0
        maxBorder = round(max(qc_data$FractionOfRunAboveTotalIntensityQ1, na.rm = TRUE),2)
        intBorder = round((max(qc_data$FractionOfRunAboveTotalIntensityQ1, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), FractionOfRunAboveTotalIntensityQ1)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Run Fraction', title = 'Fraction of Run With Eluting Peaks') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 34) {
        minBorder = 0
        maxBorder = round(max(qc_data$IdChargeRatio3to2, na.rm = TRUE),2)
        intBorder = round((max(qc_data$IdChargeRatio3to2, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio3to2)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 3 to 2)', title = 'Charge Ratio of z = 3 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 35) {
        minBorder = 0
        maxBorder = round(max(qc_data$IdChargeRatio4to2, na.rm = TRUE),2)
        intBorder = round((max(qc_data$IdChargeRatio4to2, na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio4to2)) +
          geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 4 to 2)', title = 'Charge Ratio of z = 4 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
    }
    
    else if (input$yaxis_radio == 2){
        
      qcFile = input$qc_file
        
      if (is.null(qcFile))
          return(NULL)
        
        qc_data = read.table(qcFile$datapath, header = TRUE, sep = ',')
        qc_data$injection = factor(seq(1:nrow(qc_data)))
        
        minBorder = input$yaxis_min
        maxBorder = input$yaxis_max
        intBorder = input$yaxis_int
        
        xaxis_first = input$xaxis_min
        xaxis_last = input$xaxis_max
        xaxis_interval = input$xaxis_int
        
        if (input$qc_plot_id == 2) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), TotalAnalysisTime)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Total Analysis Time (minutes)', title = 'Total Analysis Time') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 3) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs1Scans)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Number of MS1 Scans', title = 'MS1 Scans') +
            theme(axis.text.x = element_text(size = 12)) +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 4) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs2Scans)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Number of MS2 Scans', title = 'MS2 Scans') +
            theme(axis.text.x = element_text(size = 12)) +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 5) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs3Scans)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Number of MS3 Scans', title = 'MS3 Scans') +
            theme(axis.text.x = element_text(size = 12)) +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 6) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms1ScanRate..s.)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'MS1 Scan Rate (Hz)', title = 'MS1 Scan Rate') +
            theme(axis.text.x = element_text(size = 12)) +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 7) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms2ScanRate..s.)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'MS2 Scan Rate (Hz)', title = 'MS2 Scan Rate') +
            theme(axis.text.x = element_text(size = 12)) +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 8) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms3ScanRate..s.)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'MS3 Scan Rate (Hz)', title = 'MS3 Scan Rate') +
            theme(axis.text.x = element_text(size = 12)) +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 9) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanDutyCycle.s.)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Mean Duty Cycle (s)', title = 'Duty Cycle') +
            theme(axis.text.x = element_text(size = 12)) +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 10) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanMs2TriggerRate..Ms1Scan.)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Mean Number of MS2 Triggered per MS1', title = 'MS2 Trigger Rate') +
            theme(axis.text.x = element_text(size = 12)) +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 11) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms1MedianSummedIntensity))) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'log10(Median Summed MS1 Intensity)', title = 'MS1 Intensity') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 12) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms2MedianSummedIntensity))) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'log10(Median Summed MS2 Intensity)', title = 'MS2 Intensity') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 13) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(MedianPrecursorIntensity))) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'log10(Median Precursor Intensity)', title = 'Precursor Intensity') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 14) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2PeakFractionConsumingTop80PercentTotalIntensity)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Fraction of MS2 Peaks', title = 'MS2 Peak Fraction') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 15) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), EsiInstabilityFlags.count.)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Number of Scans with >10% Deviation from Bordering', title = 'Electrospray Stability') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 16) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMassDrift.ppm.)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Mass Drift (ppm)', title = 'Mass Analyzer Drift') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 17) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), IdentificationRate.IDs.Ms2Scan.)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Spectra Identification Rate', title = 'Spectral Identification Rate') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 18) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), DigestionEfficiency)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Digestion Efficiency', title = 'Digestion Efficiency') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 19) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), MissedCleavageRate..PSM.)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Missed Cleavage Rate', title = 'Missed Cleavage Rate') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 20) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtNTerm)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'N-term Labeling Efficiency', title = 'N-term Labeling Efficiency') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 21) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtK)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Lysine Labeling Efficiency', title = 'Lysine Labeling Efficiency') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 22) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtX)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Modification Labeling Efficiency', title = 'Specified Modification Labeling Efficiency') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 23) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMsFillTime)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Median MS1 Ion Injection Time', title = 'MS1 Ion Injection Time') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 24) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2FillTime)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS2 Ion Injection Time') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 25) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs3FillTime)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS3 Ion Injection Time') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 26) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt10.H.s.)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Peak Width at 10% Height', title = 'Peak Width at 10% Height') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 27) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt50.H.s.)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Peak Width at 50% Height', title = 'Peak Width at 50% Height') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 28) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt10.H)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Peak Asymmetry at 10% Height', title = 'Peak Asymmetry at 10% Height') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 29) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt50.H)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Peak Asymmetry at 50% Height', title = 'Peak Asymmetry at 50% Height') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 30) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), PeakCapacity)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Column Peak Capacity', title = 'Column Peak Capacity') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 31) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeBeforeFirstExceedanceOfTotalIntensityQ1)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time Before First Peak Elution') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 32) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeAfterLastExceedanceOfTotalIntensityQ1)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time After Last Peak Elution') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 33) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), FractionOfRunAboveTotalIntensityQ1)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Run Fraction', title = 'Fraction of Run With Eluting Peaks') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 34) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio3to2)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Charge Ratio (z = 3 to 2)', title = 'Charge Ratio of z = 3 to 2') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
        
        else if (input$qc_plot_id == 35) {
          output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio4to2)) +
            geom_point(fill = brewer.pal(3,'Set3')[1], pch = 21, size = 3, colour = 'black', alpha = 0.8) +
            labs(x = "Injection Number", y = 'Charge Ratio (z = 4 to 2)', title = 'Charge Ratio of z = 4 to 2') +
            theme(axis.text.x = element_text(size = 12), legend.position="none") +
            scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
            scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
          output_plot}
      }
    })
    
  output$qc_descriptions = renderText({
      if (input$qc_plot_id == 2) {
        paste('This is a scatter plot of the total time in minutes used for data acquisition for all of the files within the range selected.')}
      else if (input$qc_plot_id == 3) {
        paste('This plot shows the number of MS1 scans acquired in each of the data files within the range selected.')}
      else if (input$qc_plot_id == 4) {
        paste('This plot shows the number of MS2 scans acquired in each of the data files within the range selected.')}
      else if (input$qc_plot_id == 5) {
        paste('This plot shows the number of MS3 scans acquired in each of the data files within the range selected.')}
      else if (input$qc_plot_id == 6) {
        paste('This plot shows the MS1 scan acquisition rate per second (Hz). For each file, this value is calculated as a mean of
              all MS1 scans acquired per second values across the entire run length.')}
      else if (input$qc_plot_id == 7) {
        paste('This plot shows the MS2 scan acquisition rate per second (Hz). For each file, this value is calculated as a mean of
              all MS2 scans acquired per second values across the entire run length.')}
      else if (input$qc_plot_id == 8) {
        paste('This plot shows the MS3 scan acquisition rate per second (Hz). For each file, this value is calculated as a mean of
              all MS3 scans acquired per second values across the entire run length.')}
      else if (input$qc_plot_id == 9) {
        paste('This plot shows the amount of time required to complete a duty cycle in seconds. For the purposes
              of this calculation we define a duty cycle as the time between two neighbouring MS1 scans. The value
              for each file is the mean of all duty cycles performed across the entire run length.')}
      else if (input$qc_plot_id == 10) {
        paste('This is a plot of the number of MS2 scans triggered from an MS1. For each file, the mean value of
              all MS1 events where an MS2 was triggered across the entire run length is calculated.')}
      else if (input$qc_plot_id == 11) {
        paste('This is a plot of the signal intensity observed in MS1 scans. For each file, the median value of
              across the entire acquisition length of the total summed intensity in each MS1 scan is calculated. Values
              are plotted on a logarithmic scale.')}
      else if (input$qc_plot_id == 12) {
        paste('This is a plot of the signal intensity observed in MS2 scans. For each file, the median value of
              across the entire acquisition length of the total summed intensity in each MS2 scan is calculated. Values
              are plotted on a logarithmic scale.')}
      else if (input$qc_plot_id == 13) {
        paste('This is a plot of the signal intensity observed for precursors. For the purpose of this plot, we define
              a precursor as a parent ion mass that triggered an MS2 event. For each file, the median value of
              across the entire acquisition length of the precursor intensities is calculated. Values
              are plotted on a logarithmic scale.')}
      else if (input$qc_plot_id == 14) {
        paste('This is a plot of the amount of fragments in an MS2 scan that make up the observed signal. For each MS2 scan,
              the fraction of all observed fragment ions required to consume 80% of the total observed ion intensity is
              calculated. For each file the value is represented as a median of all MS2 scans across the entire acquisition length. 
              This metric can be used as a rough way of looking at fragmentation efficiency.')}
      else if (input$qc_plot_id == 15) {
        paste('This plot displays electrspray stability. Stability is calculated as the number of MS1 scan events where
              the signal difference in comparison to neighbouring scans is greater than 10%. A higher value indicates
              greater instability.')}
      else if (input$qc_plot_id == 16) {
        paste('This is a plot of the drift in mass analyzer accuracy. Using the IdentiPy search results, the deviation
              of peptide matches from the expected masses is calculated for all hits.')}
      else if (input$qc_plot_id == 17) {
        paste('This plot displays the spectra identification rate. For each file, the number of target peptide identifications
              extracted from the IdentiPy results compared with the number of MS2 scans used as input for the search is 
              calculated.')}
      else if (input$qc_plot_id == 18) {
        paste('This is a plot of enzyme digestion efficiency. From the IdentiPy search results, peptide sequences of target
              database hits are extracted. Digestion efficiency is calculated as a proportion of the number of cuts made 
              compared to the number of available cut sites. A higher value indicates a greater efficiency in proteolysis.')}
      else if (input$qc_plot_id == 19) {
        paste('This is a plot of the missed cleavages observed per peptide. From the IdentiPy search results, peptide
              sequences are examined to count the numbers of missed cleavage sites. The total number of missed cleavages
              compared to the total number of target peptide matches is compared to calculate the output value.')}
      else if (input$qc_plot_id == 20) {
        paste('This is a plot of the labeling efficiency of an N-terminal modification. From the IdentiPy search results,
              peptide sequences are examined to determine the frequency of the presence of a user-specified N-terminal
              modification.')}
      else if (input$qc_plot_id == 21) {
        paste('This is a plot of the labeling efficiency of a lysine modification. From the IdentiPy search results,
              peptide sequences are examined to determine the frequency of the presence of a user-specified lysine
              modification.')}
      else if (input$qc_plot_id == 22) {
        paste('This is a plot of the labeling efficiency of a user-specified modification. From the IdentiPy search results,
              peptide sequences are examined to determine the frequency of the presence of a user-specified modification. In
              the case of the example data set, oxidation of methionine is the specified modification.')}
      else if (input$qc_plot_id == 23) {
        paste('This is a plot of the ion injection times for MS1 scans. Values are represented as the median across all MS1
              scan events in a given acquisition.')}
      else if (input$qc_plot_id == 24) {
        paste('This is a plot of the ion injection times for MS2 scans. Values are represented as the median across all MS2
              scan events in a given acquisition.')}
      else if (input$qc_plot_id == 25) {
        paste('This is a plot of the ion injection times for MS3 scans. Values are represented as the median across all MS3
              scan events in a given acquisition.')}
      else if (input$qc_plot_id == 26) {
        paste('This is a plot of the peak width for observed precursors in MS1 scans in a given run. The peak width of a precursor
              is calculated by performing a windowed search in MS1 scans prior to and following the MS1 event where the precursor
              triggered an MS2 event. Peak width is calculated based on a curve fit to the intensity data across the determined elution window. 
              The output value is the mean across all precursors observed in an acquisition. Values are for the width at 10% of the maximum
              peak height.')}
      else if (input$qc_plot_id == 27) {
        paste('This is a plot of the peak width for observed precursors in MS1 scans in a given run. The peak width of a precursor
              is calculated by performing a windowed search in MS1 scans prior to and following the MS1 event where the precursor
              triggered an MS2 event. Peak width is calculated based on a curve fit to the intensity data across the determined elution window. 
              The output value is the mean across all precursors observed in an acquisition. Values are for the width at 50% of the maximum
              peak height.')}
      else if (input$qc_plot_id == 28) {
        paste('This is a plot of the peak asymmetry for precursors observed in an acquisition. The asymmetry of a precursor
              is calculated by performing a windowed search in MS1 scans prior to and following the MS1 event where the precursor
              triggered an MS2 event. A curve is fit to the elution profile and the asymmetry calculated at 10% of the maximum peak height. 
              The output value is the mean across all precursors observed in an acquisition.')}
      else if (input$qc_plot_id == 29) {
        paste('This is a plot of the peak asymmetry for precursors observed in an acquisition. The asymmetry of a precursor
              is calculated by performing a windowed search in MS1 scans prior to and following the MS1 event where the precursor
              triggered an MS2 event. A curve is fit to the elution profile and the asymmetry calculated at 50% of the maximum peak height. 
              The output value is the mean across all precursors observed in an acquisition.')}
      else if (input$qc_plot_id == 30) {
        paste('This is a plot of the peak capacity of the chromatography column used in an analysis. Peak capacity is calculated using 
              a standard formula making use of peak widths at 4-sigma of the peak height, along with the gradient length.')}
      else if (input$qc_plot_id == 31) {
        paste('This is a plot of the amount of time in minutes before the first peaks are observed to elute from the chromatography
              column. The appearance of peaks is defined by looking for when a threshold of 10% of the total summed intensity of the 
              most intense MS1 scan is exceeded. A moving average with a window of 20 MS1 scans is used to determine when the 
              threshold is surpassed.')}
      else if (input$qc_plot_id == 32) {
        paste('This is a plot of the amount of time in minutes after the last peaks are observed to elute from the chromatography
              column. The appearance of peaks is defined by looking for when a threshold of 10% of the total summed intensity of the 
              most intense MS1 scan is exceeded. A moving average with a window of 20 MS1 scans is used to determine when the 
              threshold is surpassed.')}
      else if (input$qc_plot_id == 33) {
        paste('This is a plot of the fraction of the acquisition where there are peaks eluting. The appearance of 
              peaks is defined by looking for when a threshold of 10% of the total summed intensity of the 
              most intense MS1 scan is exceeded. A moving average with a window of 20 MS1 scans is used to determine when the 
              threshold is surpassed.')}
      else if (input$qc_plot_id == 34) {
        paste('This is a plot of the number of z = 3 precursors observed compared to z = 2. Data are taken from the IdentiPy results 
              for this calculation.')}
      else if (input$qc_plot_id == 35) {
        paste('This is a plot of the number of z = 4 precursors observed compared to z = 2. Data are taken from the IdentiPy results 
              for this calculation.')}
      
    })

})



#####################################################################################################
#Run the application 
shinyApp(ui = ui, server = server)

