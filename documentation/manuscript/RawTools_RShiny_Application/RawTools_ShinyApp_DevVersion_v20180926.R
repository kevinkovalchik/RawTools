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
library(data.table)
library(shinydashboard)

#####################################################################################################
#UI
ui = shinyUI(

  dashboardPage(
    dashboardHeader(title = 'RawTools'),
    dashboardSidebar(
      sidebarMenu(
        menuItem("Scan Matrix", tabName = "scan_tab", icon = icon("equalizer", lib = "glyphicon")),
        menuItem("Quality Control", tabName = "qc_tab", icon = icon("wrench", lib = "glyphicon")),
        menuItem("Chromatograms", tabName = "chro_tab", icon = icon("pencil", lib = "glyphicon"))
      )
    ),
    dashboardBody(
      tabItems(
        tabItem(tabName = "scan_tab",
                fluidRow(
                  column(6,
                         tabBox(title = "1. Data Input", id = "scan_tabset1", width = NULL, side = 'right',  
                                tabPanel('File Input',
                                         fileInput('scan_file', label = NULL, accept=c('.txt')),
                                         tags$p('Select a scan Matrix file output from RawTools as your input. This should be a file
                                                ending in Matrix.txt generated using the RawTools parse function.')),
                                tabPanel("Example Data",
                                         tags$p('No data set yet? Download the example data. This is a single injection
                                                  of a HeLa digest on an Orbitrap Velos system.'),
                                         tags$div(class="header", checked=NA,
                                                  tags$a(href="https://github.com/kevinkovalchik/RawTools/blob/master/documentation/scan-matrix_example-data/ScanData_Matrix.txt", 
                                                         "Download Example Data"))
                                ))),
                column(6,
                       tabBox(title = '2. Set the plotting options', id = 'scan_tabset2', width = NULL, side = 'right', 
                              tabPanel('x-axis',
                                       tags$p('Adjust the retention time range that will be plotted. The values in the boxes
                                              are the default setting. You can start with the pre-determined values
                                              and switch to custom values in real time.'),
                                       radioButtons('scan_xaxis_radio', label = NULL, choices = list('Use Pre-determined Values' = 1, 'Use Custom Values' = 2), selected = 1),
                                       fluidRow(
                                         column(4,
                                                h6('min'),
                                                numericInput('scan_xaxis_min', label = NULL, 0, 0, 10000)),
                                         column(4,
                                                h6('max'),
                                                numericInput('scan_xaxis_max', label = NULL, 30, 1, 10000)),
                                         column(4,
                                                h6('interval'),
                                                numericInput('scan_xaxis_int', label = NULL, 5, 1, 1000))))))),
                fluidRow(
                  column(12,
                  box(title = 'Plots', width = NULL, solidHeader = FALSE, status = "primary", collapsible = TRUE,
                     helpText('Select the plot you are interested in seeing using the drop down menu.'),
                     selectInput('scan_plot_id', label = NULL, choices = list('None' = 1,
                                                                            'MS2 Scans Triggered Per MS1' = 2,
                                                                            'Duty Cycle Duration' = 3,
                                                                            'Parent Ion Mass' = 4,
                                                                            'Parent Ion Charge' = 5,
                                                                            'Parent Peak Area' = 6,
                                                                            'Parent Baseline Peak Width' = 7,
                                                                            'MS1 Ion Injection Time' = 8,
                                                                            'MS2 Ion Injection Time' = 9,
                                                                            'MS2 Scan Rate' = 10,
                                                                            'Parent Ion Purity' = 11), 
                                 selected = 1), 
                     br(),
                     plotOutput('scan_plots'),
                     br(),
                     tags$b('Plot Description'),
                     textOutput('scan_descriptions'))))),
        
    
        tabItem(tabName = "qc_tab",
                fluidRow(
                  column(6,
                         tabBox(title = "1. Data Input", id = "qc_tabset1", width = NULL, side = 'right',  
                                tabPanel('File Input',
                                         fileInput('qc_file', label = NULL, accept=c('.csv')),
                                         tags$p('Select a quality control file output from RawTools as your input. This 
                                  should be a CSV file that can be found in the directory specified by the -q flag 
                                  in RawTools when using the QC module.')),
                                tabPanel("Example Data",
                                         tags$p('No data set yet? Download the example data. This is a set of 140 replicate 
                                                  injections of a HeLa digest on an Orbitrap Velos system. The data were searched 
                                                  using IdentiPy in RawTools, where oxidation of methionine was used as a variable 
                                                  modification with -xmod.'),
                                         tags$div(class="header", checked=NA,
                                                  tags$a(href="https://github.com/kevinkovalchik/RawTools/blob/master/documentation/QC_example-data/QcDataTable.csv", 
                                                         "Download Example Data"))
                                ))),
                  column(6,
                         tabBox(title = '2. Set the plotting options', id = 'qc_tabset2', width = NULL, side = 'right', 
                                tabPanel('x-axis',
                                  tags$p('Adjust the values to set the range of the x-axis. The values will set the minimum, 
                                  maximum, and the interval between tick marks on the x-axis. If you only want to display 
                                  a specific set of samples, use these values to adjust which are displayed. The values 
                                  in the boxes are the preset defaults. These values can be changed in real-time and the 
                                  plots will update.'),
                                  radioButtons('xaxis_radio', label = NULL, choices = list('Use Pre-determined Values' = 1, 'Use Custom Values' = 2), selected = 1),
                                  fluidRow(
                                    column(4,
                                      h6('min'),
                                      numericInput('xaxis_min', label = NULL, 1, 1, 10000)),
                                     column(4,
                                     h6('max'),
                                      numericInput('xaxis_max', label = NULL, 10, 1, 10000)),
                                    column(4,
                                      h6('interval'),
                                      numericInput('xaxis_int', label = NULL, 1, 1, 1000)))),
                         
                                tabPanel('y-axis',
                                  tags$p('Adjust the values to set the range of the y-axis. The values will set the minimum, 
                                  maximum, and the interval between tick marks on the y-axis. Select the Use Custom Values 
                                  radio button to activate your input values. The values in the boxes are the preset 
                                  defaults.'),
                                  radioButtons('yaxis_radio', label = NULL, choices = list('Use Pre-determined Values' = 1, 'Use Custom Values' = 2), selected = 1),
                                  fluidRow(
                                    column(4,
                                      h6('min'),
                                      numericInput('yaxis_min', label = NULL, 0, 0, 1e8)),
                                    column(4,
                                     h6('max'),
                                      numericInput('yaxis_max', label = NULL, 100, 1, 1e8)),
                                    column(4,
                                      h6('interval'),
                                      numericInput('yaxis_int', label = NULL, 1000, 1, 1e8)))),
                                
                                tabPanel('outliers',
                                  tags$p('Input an optimum value and the percentage of difference from this value you consider 
                                  to be acceptable. Data points that extend beyond the allowable percentage of the input 
                                  optimum value will be marked as outliers on the generated plots.'),
                                  radioButtons('outlier_radio', label = NULL, choices = list('Do not mark outliers' = 1, 'Mark outliers' = 2), selected = 1),
                                  fluidRow(
                                    column(4,
                                      h6('value'),
                                      numericInput('out_val', label = NULL, 5, 0, 1e10)),
                                    column(4,
                                      h6('range (%)'),
                                      numericInput('range_val', label = NULL, 100, 0, 1000))))
                  ))),
                fluidRow(
                  column(12,
                         box(title = 'Plots', width = NULL, solidHeader = FALSE, status = "primary", collapsible = TRUE,
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
                                                                                'Ratio of z = 4 to 2' = 35), selected = 1), 
                         br(),
                         plotOutput('qc_plots'),
                         br(),
                         tags$b('Plot Description'),
                         textOutput('qc_descriptions')))
                         )
                ),
        tabItem(tabName = "chro_tab",
                fluidRow(
                  column(6,
                         tabBox(title = "1. Data Input", id = "chro_tabset1", width = NULL, side = 'right',  
                                tabPanel('File Input',
                                         fileInput('chro_file', label = NULL, accept=c('.txt')),
                                         tags$p('Select a chromatogram file. This should be a file
                                                ending in chromatogram.txt generated using the RawTools parse function with the --chro flag.')),
                                tabPanel("Example Data",
                                         tags$p('No data set yet? Download the example data. This is a single injection
                                                  of a HeLa digest on an Orbitrap Velos system.'),
                                         tags$div(class="header", checked=NA,
                                                  tags$a(href="https://github.com/kevinkovalchik/RawTools/blob/master/documentation/chromatogram_example-data/ChroData_chromatogram.txt", 
                                                         "Download Example Data"))
                                ))),
                  column(6,
                         tabBox(title = '2. Set the plotting options', id = 'chro_tabset2', width = NULL, side = 'right', 
                                tabPanel('x-axis',
                                         tags$p('Adjust the retention time range that will be plotted. The values in the boxes
                                              are the default setting. You can start with the pre-determined values
                                              and switch to custom values in real time.'),
                                         radioButtons('chro_xaxis_radio', label = NULL, choices = list('Use Pre-determined Values' = 1, 'Use Custom Values' = 2), selected = 1),
                                         fluidRow(
                                           column(4,
                                                  h6('min'),
                                                  numericInput('chro_xaxis_min', label = NULL, 0, 0, 10000)),
                                           column(4,
                                                  h6('max'),
                                                  numericInput('chro_xaxis_max', label = NULL, 30, 1, 10000)),
                                           column(4,
                                                  h6('interval'),
                                                  numericInput('chro_xaxis_int', label = NULL, 5, 1, 1000))))))),
                fluidRow(
                  column(12,
                         box(title = 'Plots', width = NULL, solidHeader = FALSE, status = "primary", collapsible = TRUE,
                         plotOutput('chro_plots'))))
                )
        )
      )
    )
)

#####################################################################################################
#Server
server = shinyServer(function(input, output) {

  output$qc_plots = renderPlot({
    
    if ((input$xaxis_radio == 1) & (input$yaxis_radio == 1) & (input$outlier_radio == 1)){
      qcFile = input$qc_file
      
      if (is.null(qcFile))
        return(NULL)
      
      qc_data = read.table(qcFile$datapath, header = TRUE, sep = ',')
      qc_data$injection = factor(seq(1:nrow(qc_data)))
      
      xaxis_first = 1
      xaxis_last = nrow(qc_data)
      xaxis_interval = nrow(qc_data) / 10
      
      outlier_optimum_value = input$out_val
      outlier_range = input$range_val
      
      if (input$qc_plot_id == 2) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'TotalAnalysisTime'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'TotalAnalysisTime'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'TotalAnalysisTime'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TotalAnalysisTime)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Total Analysis Time (minutes)', title = 'Total Analysis Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 3) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'NumMs1Scans'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'NumMs1Scans'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'NumMs1Scans'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs1Scans)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS1 Scans', title = 'MS1 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 4) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'NumMs2Scans'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'NumMs2Scans'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'NumMs2Scans'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs2Scans)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS2 Scans', title = 'MS2 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 5) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'NumMs3Scans'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'NumMs3Scans'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'NumMs3Scans'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs3Scans)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS3 Scans', title = 'MS3 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 6) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'Ms1ScanRate..s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'Ms1ScanRate..s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'Ms1ScanRate..s.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms1ScanRate..s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS1 Scan Rate (Hz)', title = 'MS1 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 7) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'Ms2ScanRate..s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'Ms2ScanRate..s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'Ms2ScanRate..s.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms2ScanRate..s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS2 Scan Rate (Hz)', title = 'MS2 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 8) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'Ms3ScanRate..s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'Ms3ScanRate..s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'Ms3ScanRate..s.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), abs(Ms3ScanRate..s.))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS3 Scan Rate (Hz)', title = 'MS3 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 9) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MeanDutyCycle.s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MeanDutyCycle.s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MeanDutyCycle.s.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanDutyCycle.s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mean Duty Cycle (s)', title = 'Duty Cycle') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 10) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MeanMs2TriggerRate..Ms1Scan.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MeanMs2TriggerRate..Ms1Scan.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MeanMs2TriggerRate..Ms1Scan.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanMs2TriggerRate..Ms1Scan.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mean Number of MS2 Triggered per MS1', title = 'MS2 Trigger Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 11) {
        minBorder = 0
        maxBorder = round(max(log10(qc_data[xaxis_first:xaxis_last,'Ms1MedianSummedIntensity']), na.rm = TRUE) + (max(log10(qc_data[xaxis_first:xaxis_last,'Ms1MedianSummedIntensity']), na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(log10(qc_data[xaxis_first:xaxis_last,'Ms1MedianSummedIntensity']), na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms1MedianSummedIntensity))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS1 Intensity)', title = 'MS1 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 12) {
        minBorder = 0
        maxBorder = round(max(log10(qc_data[xaxis_first:xaxis_last,'Ms2MedianSummedIntensity']), na.rm = TRUE) + (max(log10(qc_data[xaxis_first:xaxis_last,'Ms2MedianSummedIntensity']), na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(log10(qc_data[xaxis_first:xaxis_last,'Ms2MedianSummedIntensity']), na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms2MedianSummedIntensity))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS2 Intensity)', title = 'MS2 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 13) {
        minBorder = 0
        maxBorder = round(max(log10(qc_data[xaxis_first:xaxis_last,'MedianPrecursorIntensity']), na.rm = TRUE) + (max(log10(qc_data[xaxis_first:xaxis_last,'MedianPrecursorIntensity']), na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(log10(qc_data[xaxis_first:xaxis_last,'MedianPrecursorIntensity']), na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(MedianPrecursorIntensity))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Precursor Intensity)', title = 'Precursor Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 14) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMs2PeakFractionConsumingTop80PercentTotalIntensity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMs2PeakFractionConsumingTop80PercentTotalIntensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMs2PeakFractionConsumingTop80PercentTotalIntensity'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2PeakFractionConsumingTop80PercentTotalIntensity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Fraction of MS2 Peaks', title = 'MS2 Peak Fraction') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 15) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'EsiInstabilityFlags.count.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'EsiInstabilityFlags.count.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'EsiInstabilityFlags.count.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), EsiInstabilityFlags.count.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of Scans with >10% Deviation from Bordering', title = 'Electrospray Stability') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 16) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMassDrift.ppm.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMassDrift.ppm.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMassDrift.ppm.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), abs(MedianMassDrift.ppm.))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mass Drift (ppm)', title = 'Mass Analyzer Drift') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 17) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'IdentificationRate.IDs.Ms2Scan.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'IdentificationRate.IDs.Ms2Scan.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'IdentificationRate.IDs.Ms2Scan.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdentificationRate.IDs.Ms2Scan.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Spectra Identification Rate', title = 'Spectral Identification Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 18) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'DigestionEfficiency'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'DigestionEfficiency'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'DigestionEfficiency'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), DigestionEfficiency)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Digestion Efficiency', title = 'Digestion Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 19) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MissedCleavageRate..PSM.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MissedCleavageRate..PSM.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MissedCleavageRate..PSM.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MissedCleavageRate..PSM.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Missed Cleavage Rate', title = 'Missed Cleavage Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 20) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtNTerm'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtNTerm'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtNTerm'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtNTerm)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'N-term Labeling Efficiency', title = 'N-term Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 21) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtK'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtK'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtK'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtK)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Lysine Labeling Efficiency', title = 'Lysine Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 22) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtX'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtX'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtX'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtX)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Modification Labeling Efficiency', title = 'Specified Modification Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 23) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMsFillTime.ms.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMsFillTime.ms.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMsFillTime.ms.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMsFillTime.ms.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS1 Ion Injection Time', title = 'MS1 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 24) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMs2FillTime.ms.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMs2FillTime.ms.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMs2FillTime.ms.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2FillTime.ms.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS2 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 25) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMs3FillTime.ms.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMs3FillTime.ms.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMs3FillTime.ms.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs3FillTime.ms.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS3 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 26) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'WidthAt10.H.s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'WidthAt10.H.s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'WidthAt10.H.s.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt10.H.s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Width at 10% Height', title = 'Peak Width at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 27) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'WidthAt50.H.s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'WidthAt50.H.s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'WidthAt50.H.s.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt50.H.s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Width at 50% Height', title = 'Peak Width at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 28) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt10.H'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt10.H'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt10.H'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt10.H)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 10% Height', title = 'Peak Asymmetry at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 29) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt50.H'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt50.H'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt50.H'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt50.H)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 50% Height', title = 'Peak Asymmetry at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 30) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'PeakCapacity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'PeakCapacity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'PeakCapacity'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), PeakCapacity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Column Peak Capacity', title = 'Column Peak Capacity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 31) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'TimeBeforeFirstExceedanceOf10.MaxIntensity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'TimeBeforeFirstExceedanceOf10.MaxIntensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'TimeBeforeFirstExceedanceOf10.MaxIntensity'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeBeforeFirstExceedanceOf10.MaxIntensity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time Before First Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 32) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'TimeAfterLastExceedanceOf10.MaxIntensity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'TimeAfterLastExceedanceOf10.MaxIntensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'TimeAfterLastExceedanceOf10.MaxIntensity'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeAfterLastExceedanceOf10.MaxIntensity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time After Last Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 33) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'FractionOfRunAbove10.MaxIntensity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'FractionOfRunAbove10.MaxIntensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'FractionOfRunAbove10.MaxIntensity'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), FractionOfRunAbove10.MaxIntensity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Run Fraction', title = 'Fraction of Run With Eluting Peaks') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 34) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio3to2'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio3to2'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio3to2'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio3to2)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 3 to 2)', title = 'Charge Ratio of z = 3 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 35) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio4to2'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio4to2'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio4to2'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio4to2)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 4 to 2)', title = 'Charge Ratio of z = 4 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
    }
    
    else if ((input$xaxis_radio == 1) & (input$yaxis_radio == 1) & (input$outlier_radio == 2)){
      qcFile = input$qc_file
      
      if (is.null(qcFile))
        return(NULL)
      
      qc_data = read.table(qcFile$datapath, header = TRUE, sep = ',')
      qc_data$injection = factor(seq(1:nrow(qc_data)))
      
      xaxis_first = 1
      xaxis_last = nrow(qc_data)
      xaxis_interval = nrow(qc_data) / 10
      
      outlier_optimum_value = input$out_val
      outlier_range = input$range_val
      
      if (input$qc_plot_id == 2) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'TotalAnalysisTime'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'TotalAnalysisTime'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'TotalAnalysisTime'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$TotalAnalysisTime - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TotalAnalysisTime)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Total Analysis Time (minutes)', title = 'Total Analysis Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 3) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'NumMs1Scans'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'NumMs1Scans'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'NumMs1Scans'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$NumMs1Scans - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs1Scans)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS1 Scans', title = 'MS1 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 4) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'NumMs2Scans'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'NumMs2Scans'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'NumMs2Scans'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$NumMs2Scans - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs2Scans)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS2 Scans', title = 'MS2 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 5) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'NumMs3Scans'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'NumMs3Scans'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'NumMs3Scans'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$NumMs3Scans - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs3Scans)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS3 Scans', title = 'MS3 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 6) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'Ms1ScanRate..s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'Ms1ScanRate..s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'Ms1ScanRate..s.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$Ms1ScanRate..s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms1ScanRate..s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS1 Scan Rate (Hz)', title = 'MS1 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 7) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'Ms2ScanRate..s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'Ms2ScanRate..s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'Ms2ScanRate..s.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$Ms2ScanRate..s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms2ScanRate..s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS2 Scan Rate (Hz)', title = 'MS2 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 8) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'Ms3ScanRate..s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'Ms3ScanRate..s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'Ms3ScanRate..s.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$Ms3ScanRate..s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), abs(Ms3ScanRate..s.))) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS3 Scan Rate (Hz)', title = 'MS3 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 9) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MeanDutyCycle.s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MeanDutyCycle.s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MeanDutyCycle.s.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MeanDutyCycle.s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanDutyCycle.s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mean Duty Cycle (s)', title = 'Duty Cycle') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 10) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MeanMs2TriggerRate..Ms1Scan.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MeanMs2TriggerRate..Ms1Scan.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MeanMs2TriggerRate..Ms1Scan.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MeanMs2TriggerRate..Ms1Scan. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanMs2TriggerRate..Ms1Scan.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mean Number of MS2 Triggered per MS1', title = 'MS2 Trigger Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 11) {
        minBorder = 0
        maxBorder = round(max(log10(qc_data[xaxis_first:xaxis_last,'Ms1MedianSummedIntensity']), na.rm = TRUE) + (max(log10(qc_data[xaxis_first:xaxis_last,'Ms1MedianSummedIntensity']), na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(log10(qc_data[xaxis_first:xaxis_last,'Ms1MedianSummedIntensity']), na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$Ms1MedianSummedIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms1MedianSummedIntensity))) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS1 Intensity)', title = 'MS1 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 12) {
        minBorder = 0
        maxBorder = round(max(log10(qc_data[xaxis_first:xaxis_last,'Ms2MedianSummedIntensity']), na.rm = TRUE) + (max(log10(qc_data[xaxis_first:xaxis_last,'Ms2MedianSummedIntensity']), na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(log10(qc_data[xaxis_first:xaxis_last,'Ms2MedianSummedIntensity']), na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$Ms2MedianSummedIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms2MedianSummedIntensity))) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS2 Intensity)', title = 'MS2 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 13) {
        minBorder = 0
        maxBorder = round(max(log10(qc_data[xaxis_first:xaxis_last,'MedianPrecursorIntensity']), na.rm = TRUE) + (max(log10(qc_data[xaxis_first:xaxis_last,'MedianPrecursorIntensity']), na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(log10(qc_data[xaxis_first:xaxis_last,'MedianPrecursorIntensity']), na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MedianPrecursorIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(MedianPrecursorIntensity))) +
          geom_point(fill =qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Precursor Intensity)', title = 'Precursor Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 14) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMs2PeakFractionConsumingTop80PercentTotalIntensity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMs2PeakFractionConsumingTop80PercentTotalIntensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMs2PeakFractionConsumingTop80PercentTotalIntensity'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MedianMs2PeakFractionConsumingTop80PercentTotalIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2PeakFractionConsumingTop80PercentTotalIntensity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Fraction of MS2 Peaks', title = 'MS2 Peak Fraction') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 15) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'EsiInstabilityFlags.count.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'EsiInstabilityFlags.count.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'EsiInstabilityFlags.count.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$EsiInstabilityFlags.count. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), EsiInstabilityFlags.count.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of Scans with >10% Deviation from Bordering', title = 'Electrospray Stability') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 16) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMassDrift.ppm.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMassDrift.ppm.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMassDrift.ppm.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MedianMassDrift.ppm. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), abs(MedianMassDrift.ppm.))) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mass Drift (ppm)', title = 'Mass Analyzer Drift') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 17) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'IdentificationRate.IDs.Ms2Scan.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'IdentificationRate.IDs.Ms2Scan.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'IdentificationRate.IDs.Ms2Scan.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$IdentificationRate.IDs.Ms2Scan. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdentificationRate.IDs.Ms2Scan.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Spectra Identification Rate', title = 'Spectral Identification Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 18) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'DigestionEfficiency'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'DigestionEfficiency'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'DigestionEfficiency'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$DigestionEfficiency - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), DigestionEfficiency)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Digestion Efficiency', title = 'Digestion Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 19) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MissedCleavageRate..PSM.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MissedCleavageRate..PSM.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MissedCleavageRate..PSM.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MissedCleavageRate..PSM. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MissedCleavageRate..PSM.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Missed Cleavage Rate', title = 'Missed Cleavage Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 20) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtNTerm'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtNTerm'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtNTerm'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$ModificationFrequencyAtNTerm - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtNTerm)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'N-term Labeling Efficiency', title = 'N-term Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 21) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtK'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtK'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtK'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$ModificationFrequencyAtK - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtK)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Lysine Labeling Efficiency', title = 'Lysine Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 22) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtX'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtX'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtX'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$ModificationFrequencyAtX - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtX)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Modification Labeling Efficiency', title = 'Specified Modification Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 23) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMsFillTime.ms.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMsFillTime.ms.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMsFillTime.ms.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MedianMsFillTime.ms. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMsFillTime.ms.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS1 Ion Injection Time', title = 'MS1 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 24) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMs2FillTime.ms.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMs2FillTime.ms.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMs2FillTime.ms.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MedianMs2FillTime.ms. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2FillTime.ms.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS2 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 25) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMs3FillTime.ms.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMs3FillTime.ms.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMs3FillTime.ms.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MedianMs3FillTime.ms. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs3FillTime.ms.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS3 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 26) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'WidthAt10.H.s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'WidthAt10.H.s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'WidthAt10.H.s.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$WidthAt10.H.s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt10.H.s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Width at 10% Height', title = 'Peak Width at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 27) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'WidthAt50.H.s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'WidthAt50.H.s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'WidthAt50.H.s.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$WidthAt50.H.s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt50.H.s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Width at 50% Height', title = 'Peak Width at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 28) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt10.H'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt10.H'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt10.H'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$AsymmetryAt10.H - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt10.H)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 10% Height', title = 'Peak Asymmetry at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 29) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt50.H'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt50.H'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt50.H'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$AsymmetryAt50.H - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt50.H)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 50% Height', title = 'Peak Asymmetry at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 30) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'PeakCapacity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'PeakCapacity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'PeakCapacity'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$PeakCapacity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), PeakCapacity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Column Peak Capacity', title = 'Column Peak Capacity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 31) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'TimeBeforeFirstExceedanceOf10.MaxIntensity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'TimeBeforeFirstExceedanceOf10.MaxIntensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'TimeBeforeFirstExceedanceOf10.MaxIntensity'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$TimeBeforeFirstExceedanceOf10.MaxIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeBeforeFirstExceedanceOf10.MaxIntensity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time Before First Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 32) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'TimeAfterLastExceedanceOf10.MaxIntensity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'TimeAfterLastExceedanceOf10.MaxIntensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'TimeAfterLastExceedanceOf10.MaxIntensity'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$TimeAfterLastExceedanceOf10.MaxIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeAfterLastExceedanceOf10.MaxIntensity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time After Last Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 33) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'FractionOfRunAbove10.MaxIntensity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'FractionOfRunAbove10.MaxIntensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'FractionOfRunAbove10.MaxIntensity'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$FractionOfRunAbove10.MaxIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), FractionOfRunAbove10.MaxIntensity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Run Fraction', title = 'Fraction of Run With Eluting Peaks') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 34) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio3to2'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio3to2'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio3to2'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$IdChargeRatio3to2 - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio3to2)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 3 to 2)', title = 'Charge Ratio of z = 3 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 35) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio4to2'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio4to2'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio4to2'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$IdChargeRatio4to2 - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio4to2)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 4 to 2)', title = 'Charge Ratio of z = 4 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
    }
    
    else if ((input$xaxis_radio == 1) & (input$yaxis_radio == 2) & (input$outlier_radio == 1)){
      
      qcFile = input$qc_file
      
      if (is.null(qcFile))
        return(NULL)
      
      qc_data = read.table(qcFile$datapath, header = TRUE, sep = ',')
      qc_data$injection = factor(seq(1:nrow(qc_data)))
      
      minBorder = input$yaxis_min
      maxBorder = input$yaxis_max
      intBorder = input$yaxis_int
      
      xaxis_first = 1
      xaxis_last = nrow(qc_data)
      xaxis_interval = nrow(qc_data) / 10
      
      if (input$qc_plot_id == 2) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TotalAnalysisTime)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Total Analysis Time (minutes)', title = 'Total Analysis Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 3) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs1Scans)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS1 Scans', title = 'MS1 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 4) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs2Scans)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS2 Scans', title = 'MS2 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 5) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs3Scans)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS3 Scans', title = 'MS3 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 6) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms1ScanRate..s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS1 Scan Rate (Hz)', title = 'MS1 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 7) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms2ScanRate..s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS2 Scan Rate (Hz)', title = 'MS2 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 8) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms3ScanRate..s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS3 Scan Rate (Hz)', title = 'MS3 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 9) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanDutyCycle.s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mean Duty Cycle (s)', title = 'Duty Cycle') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 10) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanMs2TriggerRate..Ms1Scan.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mean Number of MS2 Triggered per MS1', title = 'MS2 Trigger Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 11) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms1MedianSummedIntensity))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS1 Intensity)', title = 'MS1 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 12) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms2MedianSummedIntensity))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS2 Intensity)', title = 'MS2 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 13) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(MedianPrecursorIntensity))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Precursor Intensity)', title = 'Precursor Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 14) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2PeakFractionConsumingTop80PercentTotalIntensity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Fraction of MS2 Peaks', title = 'MS2 Peak Fraction') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 15) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), EsiInstabilityFlags.count.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of Scans with >10% Deviation from Bordering', title = 'Electrospray Stability') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 16) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMassDrift.ppm.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mass Drift (ppm)', title = 'Mass Analyzer Drift') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 17) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdentificationRate.IDs.Ms2Scan.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Spectra Identification Rate', title = 'Spectral Identification Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 18) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), DigestionEfficiency)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Digestion Efficiency', title = 'Digestion Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 19) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MissedCleavageRate..PSM.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Missed Cleavage Rate', title = 'Missed Cleavage Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 20) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtNTerm)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'N-term Labeling Efficiency', title = 'N-term Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 21) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtK)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Lysine Labeling Efficiency', title = 'Lysine Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 22) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtX)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Modification Labeling Efficiency', title = 'Specified Modification Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 23) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMsFillTime.ms.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS1 Ion Injection Time', title = 'MS1 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 24) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2FillTime.ms.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS2 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 25) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs3FillTime.ms.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS3 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 26) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt10.H.s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Width at 10% Height', title = 'Peak Width at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 27) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt50.H.s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Width at 50% Height', title = 'Peak Width at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 28) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt10.H)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 10% Height', title = 'Peak Asymmetry at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 29) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt50.H)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 50% Height', title = 'Peak Asymmetry at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 30) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), PeakCapacity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Column Peak Capacity', title = 'Column Peak Capacity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 31) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeBeforeFirstExceedanceOf10.MaxIntensity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time Before First Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 32) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeAfterLastExceedanceOf10.MaxIntensity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time After Last Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 33) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), FractionOfRunAbove10.MaxIntensity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Run Fraction', title = 'Fraction of Run With Eluting Peaks') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 34) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio3to2)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 3 to 2)', title = 'Charge Ratio of z = 3 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 35) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio4to2)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 4 to 2)', title = 'Charge Ratio of z = 4 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
    }
    
    else if ((input$xaxis_radio == 1) & (input$yaxis_radio == 2) & (input$outlier_radio == 2)){
      
      qcFile = input$qc_file
      
      if (is.null(qcFile))
        return(NULL)
      
      qc_data = read.table(qcFile$datapath, header = TRUE, sep = ',')
      qc_data$injection = factor(seq(1:nrow(qc_data)))
      
      minBorder = input$yaxis_min
      maxBorder = input$yaxis_max
      intBorder = input$yaxis_int
      
      xaxis_first = 1
      xaxis_last = nrow(qc_data)
      xaxis_interval = nrow(qc_data) / 10
      
      outlier_optimum_value = input$out_val
      outlier_range = input$range_val
      
      if (input$qc_plot_id == 2) {
        qc_data$outlier_diff = abs(qc_data$TotalAnalysisTime - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TotalAnalysisTime)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Total Analysis Time (minutes)', title = 'Total Analysis Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 3) {
        qc_data$outlier_diff = abs(qc_data$NumMs1Scans - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs1Scans)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS1 Scans', title = 'MS1 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 4) {
        qc_data$outlier_diff = abs(qc_data$NumMs2Scans - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs2Scans)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS2 Scans', title = 'MS2 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 5) {
        qc_data$outlier_diff = abs(qc_data$NumMs3Scans - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs3Scans)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS3 Scans', title = 'MS3 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 6) {
        qc_data$outlier_diff = abs(qc_data$Ms1ScanRate..s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms1ScanRate..s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS1 Scan Rate (Hz)', title = 'MS1 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 7) {
        qc_data$outlier_diff = abs(qc_data$Ms2ScanRate..s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms2ScanRate..s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS2 Scan Rate (Hz)', title = 'MS2 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 8) {
        qc_data$outlier_diff = abs(qc_data$Ms3ScanRate..s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms3ScanRate..s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS3 Scan Rate (Hz)', title = 'MS3 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 9) {
        qc_data$outlier_diff = abs(qc_data$MeanDutyCycle.s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanDutyCycle.s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mean Duty Cycle (s)', title = 'Duty Cycle') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 10) {
        qc_data$outlier_diff = abs(qc_data$MeanMs2TriggerRate..Ms1Scan. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanMs2TriggerRate..Ms1Scan.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mean Number of MS2 Triggered per MS1', title = 'MS2 Trigger Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 11) {
        qc_data$outlier_diff = abs(log10(qc_data$Ms1MedianSummedIntensity) - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms1MedianSummedIntensity))) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS1 Intensity)', title = 'MS1 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 12) {
        qc_data$outlier_diff = abs(log10(qc_data$Ms2MedianSummedIntensity) - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms2MedianSummedIntensity))) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS2 Intensity)', title = 'MS2 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 13) {
        qc_data$outlier_diff = abs(log10(qc_data$MedianPrecursorIntensity) - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(MedianPrecursorIntensity))) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Precursor Intensity)', title = 'Precursor Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 14) {
        qc_data$outlier_diff = abs(qc_data$MedianMs2PeakFractionConsumingTop80PercentTotalIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2PeakFractionConsumingTop80PercentTotalIntensity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Fraction of MS2 Peaks', title = 'MS2 Peak Fraction') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 15) {
        qc_data$outlier_diff = abs(qc_data$EsiInstabilityFlags.count. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), EsiInstabilityFlags.count.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of Scans with >10% Deviation from Bordering', title = 'Electrospray Stability') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 16) {
        qc_data$outlier_diff = abs(qc_data$MedianMassDrift.ppm. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMassDrift.ppm.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mass Drift (ppm)', title = 'Mass Analyzer Drift') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 17) {
        qc_data$outlier_diff = abs(qc_data$IdentificationRate.IDs.Ms2Scan. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdentificationRate.IDs.Ms2Scan.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Spectra Identification Rate', title = 'Spectral Identification Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 18) {
        qc_data$outlier_diff = abs(qc_data$DigestionEfficiency - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), DigestionEfficiency)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Digestion Efficiency', title = 'Digestion Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 19) {
        qc_data$outlier_diff = abs(qc_data$MissedCleavageRate..PSM. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MissedCleavageRate..PSM.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Missed Cleavage Rate', title = 'Missed Cleavage Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 20) {
        qc_data$outlier_diff = abs(qc_data$ModificationFrequencyAtNTerm - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtNTerm)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'N-term Labeling Efficiency', title = 'N-term Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 21) {
        qc_data$outlier_diff = abs(qc_data$ModificationFrequencyAtK - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtK)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Lysine Labeling Efficiency', title = 'Lysine Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 22) {
        qc_data$outlier_diff = abs(qc_data$ModificationFrequencyAtX - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtX)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Modification Labeling Efficiency', title = 'Specified Modification Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 23) {
        qc_data$outlier_diff = abs(qc_data$MedianMsFillTime.ms. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMsFillTime.ms.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS1 Ion Injection Time', title = 'MS1 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 24) {
        qc_data$outlier_diff = abs(qc_data$MedianMs2FillTime.ms. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2FillTime.ms.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS2 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 25) {
        qc_data$outlier_diff = abs(qc_data$MedianMs3FillTime.ms. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs3FillTime.ms.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS3 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 26) {
        qc_data$outlier_diff = abs(qc_data$WidthAt10.H.s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt10.H.s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Width at 10% Height', title = 'Peak Width at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 27) {
        qc_data$outlier_diff = abs(qc_data$ WidthAt50.H.s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt50.H.s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Width at 50% Height', title = 'Peak Width at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 28) {
        qc_data$outlier_diff = abs(qc_data$AsymmetryAt10.H - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt10.H)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 10% Height', title = 'Peak Asymmetry at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 29) {
        qc_data$outlier_diff = abs(qc_data$AsymmetryAt50.H - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt50.H)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 50% Height', title = 'Peak Asymmetry at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 30) {
        qc_data$outlier_diff = abs(qc_data$PeakCapacity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), PeakCapacity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Column Peak Capacity', title = 'Column Peak Capacity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 31) {
        qc_data$outlier_diff = abs(qc_data$TimeBeforeFirstExceedanceOf10.MaxIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeBeforeFirstExceedanceOf10.MaxIntensity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time Before First Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 32) {
        qc_data$outlier_diff = abs(qc_data$TimeAfterLastExceedanceOf10.MaxIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeAfterLastExceedanceOf10.MaxIntensity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time After Last Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 33) {
        qc_data$outlier_diff = abs(qc_data$FractionOfRunAbove10.MaxIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), FractionOfRunAbove10.MaxIntensity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Run Fraction', title = 'Fraction of Run With Eluting Peaks') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 34) {
        qc_data$outlier_diff = abs(qc_data$IdChargeRatio3to2 - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio3to2)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 3 to 2)', title = 'Charge Ratio of z = 3 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 35) {
        qc_data$outlier_diff = abs(qc_data$IdChargeRatio4to2 - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio4to2)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 4 to 2)', title = 'Charge Ratio of z = 4 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
    }
    
    else if ((input$xaxis_radio == 2) & (input$yaxis_radio == 1) & (input$outlier_radio == 1)){
      qcFile = input$qc_file
      
      if (is.null(qcFile))
        return(NULL)
      
      qc_data = read.table(qcFile$datapath, header = TRUE, sep = ',')
      qc_data$injection = factor(seq(1:nrow(qc_data)))
      
      xaxis_first = input$xaxis_min
      xaxis_last = input$xaxis_max
      xaxis_interval = input$xaxis_int
      
      outlier_optimum_value = input$out_val
      outlier_range = input$range_val
      
      if (input$qc_plot_id == 2) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'TotalAnalysisTime'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'TotalAnalysisTime'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'TotalAnalysisTime'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TotalAnalysisTime)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Total Analysis Time (minutes)', title = 'Total Analysis Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 3) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'NumMs1Scans'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'NumMs1Scans'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'NumMs1Scans'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs1Scans)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS1 Scans', title = 'MS1 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 4) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'NumMs2Scans'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'NumMs2Scans'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'NumMs2Scans'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs2Scans)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS2 Scans', title = 'MS2 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 5) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'NumMs3Scans'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'NumMs3Scans'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'NumMs3Scans'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs3Scans)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS3 Scans', title = 'MS3 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 6) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'Ms1ScanRate..s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'Ms1ScanRate..s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'Ms1ScanRate..s.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms1ScanRate..s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS1 Scan Rate (Hz)', title = 'MS1 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 7) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'Ms2ScanRate..s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'Ms2ScanRate..s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'Ms2ScanRate..s.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms2ScanRate..s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS2 Scan Rate (Hz)', title = 'MS2 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 8) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'Ms3ScanRate..s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'Ms3ScanRate..s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'Ms3ScanRate..s.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), abs(Ms3ScanRate..s.))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS3 Scan Rate (Hz)', title = 'MS3 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 9) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MeanDutyCycle.s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MeanDutyCycle.s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MeanDutyCycle.s.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanDutyCycle.s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mean Duty Cycle (s)', title = 'Duty Cycle') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 10) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MeanMs2TriggerRate..Ms1Scan.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MeanMs2TriggerRate..Ms1Scan.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MeanMs2TriggerRate..Ms1Scan.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanMs2TriggerRate..Ms1Scan.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mean Number of MS2 Triggered per MS1', title = 'MS2 Trigger Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 11) {
        minBorder = 0
        maxBorder = round(max(log10(qc_data[xaxis_first:xaxis_last,'Ms1MedianSummedIntensity']), na.rm = TRUE) + (max(log10(qc_data[xaxis_first:xaxis_last,'Ms1MedianSummedIntensity']), na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(log10(qc_data[xaxis_first:xaxis_last,'Ms1MedianSummedIntensity']), na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms1MedianSummedIntensity))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS1 Intensity)', title = 'MS1 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 12) {
        minBorder = 0
        maxBorder = round(max(log10(qc_data[xaxis_first:xaxis_last,'Ms2MedianSummedIntensity']), na.rm = TRUE) + (max(log10(qc_data[xaxis_first:xaxis_last,'Ms2MedianSummedIntensity']), na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(log10(qc_data[xaxis_first:xaxis_last,'Ms2MedianSummedIntensity']), na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms2MedianSummedIntensity))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS2 Intensity)', title = 'MS2 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 13) {
        minBorder = 0
        maxBorder = round(max(log10(qc_data[xaxis_first:xaxis_last,'MedianPrecursorIntensity']), na.rm = TRUE) + (max(log10(qc_data[xaxis_first:xaxis_last,'MedianPrecursorIntensity']), na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(log10(qc_data[xaxis_first:xaxis_last,'MedianPrecursorIntensity']), na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(MedianPrecursorIntensity))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Precursor Intensity)', title = 'Precursor Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 14) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMs2PeakFractionConsumingTop80PercentTotalIntensity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMs2PeakFractionConsumingTop80PercentTotalIntensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMs2PeakFractionConsumingTop80PercentTotalIntensity'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2PeakFractionConsumingTop80PercentTotalIntensity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Fraction of MS2 Peaks', title = 'MS2 Peak Fraction') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 15) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'EsiInstabilityFlags.count.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'EsiInstabilityFlags.count.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'EsiInstabilityFlags.count.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), EsiInstabilityFlags.count.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of Scans with >10% Deviation from Bordering', title = 'Electrospray Stability') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 16) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMassDrift.ppm.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMassDrift.ppm.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMassDrift.ppm.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), abs(MedianMassDrift.ppm.))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mass Drift (ppm)', title = 'Mass Analyzer Drift') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 17) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'IdentificationRate.IDs.Ms2Scan.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'IdentificationRate.IDs.Ms2Scan.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'IdentificationRate.IDs.Ms2Scan.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdentificationRate.IDs.Ms2Scan.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Spectra Identification Rate', title = 'Spectral Identification Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 18) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'DigestionEfficiency'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'DigestionEfficiency'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'DigestionEfficiency'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), DigestionEfficiency)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Digestion Efficiency', title = 'Digestion Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 19) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MissedCleavageRate..PSM.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MissedCleavageRate..PSM.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MissedCleavageRate..PSM.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MissedCleavageRate..PSM.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Missed Cleavage Rate', title = 'Missed Cleavage Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 20) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtNTerm'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtNTerm'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtNTerm'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtNTerm)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'N-term Labeling Efficiency', title = 'N-term Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 21) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtK'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtK'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtK'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtK)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Lysine Labeling Efficiency', title = 'Lysine Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 22) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtX'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtX'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtX'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtX)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Modification Labeling Efficiency', title = 'Specified Modification Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 23) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMsFillTime.ms.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMsFillTime.ms.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMsFillTime.ms.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMsFillTime.ms.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS1 Ion Injection Time', title = 'MS1 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 24) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMs2FillTime.ms.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMs2FillTime.ms.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMs2FillTime.ms.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2FillTime.ms.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS2 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 25) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMs3FillTime.ms.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMs3FillTime.ms.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMs3FillTime.ms.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs3FillTime.ms.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS3 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 26) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'WidthAt10.H.s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'WidthAt10.H.s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'WidthAt10.H.s.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt10.H.s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Width at 10% Height', title = 'Peak Width at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 27) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'WidthAt50.H.s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'WidthAt50.H.s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'WidthAt50.H.s.'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt50.H.s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Width at 50% Height', title = 'Peak Width at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 28) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt10.H'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt10.H'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt10.H'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt10.H)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 10% Height', title = 'Peak Asymmetry at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 29) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt50.H'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt50.H'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt50.H'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt50.H)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 50% Height', title = 'Peak Asymmetry at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 30) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'PeakCapacity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'PeakCapacity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'PeakCapacity'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), PeakCapacity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Column Peak Capacity', title = 'Column Peak Capacity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 31) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'TimeBeforeFirstExceedanceOf10.MaxIntensity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'TimeBeforeFirstExceedanceOf10.MaxIntensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'TimeBeforeFirstExceedanceOf10.MaxIntensity'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeBeforeFirstExceedanceOf10.MaxIntensity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time Before First Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 32) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'TimeAfterLastExceedanceOf10.MaxIntensity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'TimeAfterLastExceedanceOf10.MaxIntensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'TimeAfterLastExceedanceOf10.MaxIntensity'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeAfterLastExceedanceOf10.MaxIntensity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time After Last Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 33) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'FractionOfRunAbove10.MaxIntensity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'FractionOfRunAbove10.MaxIntensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'FractionOfRunAbove10.MaxIntensity'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), FractionOfRunAbove10.MaxIntensity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Run Fraction', title = 'Fraction of Run With Eluting Peaks') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 34) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio3to2'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio3to2'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio3to2'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio3to2)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 3 to 2)', title = 'Charge Ratio of z = 3 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 35) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio4to2'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio4to2'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio4to2'], na.rm = TRUE) / 10), 5)
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio4to2)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 4 to 2)', title = 'Charge Ratio of z = 4 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
    }
    
    else if ((input$xaxis_radio == 2) & (input$yaxis_radio == 1) & (input$outlier_radio == 2)){
      qcFile = input$qc_file
      
      if (is.null(qcFile))
        return(NULL)
      
      qc_data = read.table(qcFile$datapath, header = TRUE, sep = ',')
      qc_data$injection = factor(seq(1:nrow(qc_data)))
      
      xaxis_first = input$xaxis_min
      xaxis_last = input$xaxis_max
      xaxis_interval = input$xaxis_int
      
      outlier_optimum_value = input$out_val
      outlier_range = input$range_val
      
      if (input$qc_plot_id == 2) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'TotalAnalysisTime'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'TotalAnalysisTime'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'TotalAnalysisTime'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$TotalAnalysisTime - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TotalAnalysisTime)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Total Analysis Time (minutes)', title = 'Total Analysis Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 3) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'NumMs1Scans'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'NumMs1Scans'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'NumMs1Scans'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$NumMs1Scans - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs1Scans)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS1 Scans', title = 'MS1 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 4) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'NumMs2Scans'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'NumMs2Scans'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'NumMs2Scans'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$NumMs2Scans - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs2Scans)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS2 Scans', title = 'MS2 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 5) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'NumMs3Scans'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'NumMs3Scans'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'NumMs3Scans'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$NumMs3Scans - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs3Scans)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS3 Scans', title = 'MS3 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 6) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'Ms1ScanRate..s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'Ms1ScanRate..s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'Ms1ScanRate..s.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$Ms1ScanRate..s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms1ScanRate..s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS1 Scan Rate (Hz)', title = 'MS1 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 7) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'Ms2ScanRate..s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'Ms2ScanRate..s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'Ms2ScanRate..s.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$Ms2ScanRate..s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms2ScanRate..s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS2 Scan Rate (Hz)', title = 'MS2 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 8) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'Ms3ScanRate..s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'Ms3ScanRate..s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'Ms3ScanRate..s.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$Ms3ScanRate..s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), abs(Ms3ScanRate..s.))) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS3 Scan Rate (Hz)', title = 'MS3 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 9) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MeanDutyCycle.s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MeanDutyCycle.s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MeanDutyCycle.s.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MeanDutyCycle.s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanDutyCycle.s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mean Duty Cycle (s)', title = 'Duty Cycle') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 10) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MeanMs2TriggerRate..Ms1Scan.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MeanMs2TriggerRate..Ms1Scan.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MeanMs2TriggerRate..Ms1Scan.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MeanMs2TriggerRate..Ms1Scan. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanMs2TriggerRate..Ms1Scan.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mean Number of MS2 Triggered per MS1', title = 'MS2 Trigger Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 11) {
        minBorder = 0
        maxBorder = round(max(log10(qc_data[xaxis_first:xaxis_last,'Ms1MedianSummedIntensity']), na.rm = TRUE) + (max(log10(qc_data[xaxis_first:xaxis_last,'Ms1MedianSummedIntensity']), na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(log10(qc_data[xaxis_first:xaxis_last,'Ms1MedianSummedIntensity']), na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$Ms1MedianSummedIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms1MedianSummedIntensity))) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS1 Intensity)', title = 'MS1 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 12) {
        minBorder = 0
        maxBorder = round(max(log10(qc_data[xaxis_first:xaxis_last,'Ms2MedianSummedIntensity']), na.rm = TRUE) + (max(log10(qc_data[xaxis_first:xaxis_last,'Ms2MedianSummedIntensity']), na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(log10(qc_data[xaxis_first:xaxis_last,'Ms2MedianSummedIntensity']), na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$Ms2MedianSummedIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms2MedianSummedIntensity))) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS2 Intensity)', title = 'MS2 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 13) {
        minBorder = 0
        maxBorder = round(max(log10(qc_data[xaxis_first:xaxis_last,'MedianPrecursorIntensity']), na.rm = TRUE) + (max(log10(qc_data[xaxis_first:xaxis_last,'MedianPrecursorIntensity']), na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(log10(qc_data[xaxis_first:xaxis_last,'MedianPrecursorIntensity']), na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MedianPrecursorIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(MedianPrecursorIntensity))) +
          geom_point(fill =qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Precursor Intensity)', title = 'Precursor Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 14) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMs2PeakFractionConsumingTop80PercentTotalIntensity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMs2PeakFractionConsumingTop80PercentTotalIntensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMs2PeakFractionConsumingTop80PercentTotalIntensity'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MedianMs2PeakFractionConsumingTop80PercentTotalIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2PeakFractionConsumingTop80PercentTotalIntensity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Fraction of MS2 Peaks', title = 'MS2 Peak Fraction') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 15) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'EsiInstabilityFlags.count.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'EsiInstabilityFlags.count.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'EsiInstabilityFlags.count.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$EsiInstabilityFlags.count. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), EsiInstabilityFlags.count.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of Scans with >10% Deviation from Bordering', title = 'Electrospray Stability') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 16) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMassDrift.ppm.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMassDrift.ppm.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMassDrift.ppm.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MedianMassDrift.ppm. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), abs(MedianMassDrift.ppm.))) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mass Drift (ppm)', title = 'Mass Analyzer Drift') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 17) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'IdentificationRate.IDs.Ms2Scan.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'IdentificationRate.IDs.Ms2Scan.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'IdentificationRate.IDs.Ms2Scan.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$IdentificationRate.IDs.Ms2Scan. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdentificationRate.IDs.Ms2Scan.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Spectra Identification Rate', title = 'Spectral Identification Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 18) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'DigestionEfficiency'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'DigestionEfficiency'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'DigestionEfficiency'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$DigestionEfficiency - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), DigestionEfficiency)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Digestion Efficiency', title = 'Digestion Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 19) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MissedCleavageRate..PSM.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MissedCleavageRate..PSM.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MissedCleavageRate..PSM.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MissedCleavageRate..PSM. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MissedCleavageRate..PSM.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Missed Cleavage Rate', title = 'Missed Cleavage Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 20) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtNTerm'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtNTerm'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtNTerm'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$ModificationFrequencyAtNTerm - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtNTerm)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'N-term Labeling Efficiency', title = 'N-term Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 21) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtK'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtK'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtK'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$ModificationFrequencyAtK - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtK)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Lysine Labeling Efficiency', title = 'Lysine Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 22) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtX'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtX'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'ModificationFrequencyAtX'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$ModificationFrequencyAtX - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtX)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Modification Labeling Efficiency', title = 'Specified Modification Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 23) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMsFillTime.ms.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMsFillTime.ms.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMsFillTime.ms.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MedianMsFillTime.ms. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMsFillTime.ms.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS1 Ion Injection Time', title = 'MS1 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 24) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMs2FillTime.ms.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMs2FillTime.ms.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMs2FillTime.ms.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MedianMs2FillTime.ms. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2FillTime.ms.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS2 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 25) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'MedianMs3FillTime.ms.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'MedianMs3FillTime.ms.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'MedianMs3FillTime.ms.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$MedianMs3FillTime.ms. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs3FillTime.ms.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS3 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 26) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'WidthAt10.H.s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'WidthAt10.H.s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'WidthAt10.H.s.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$WidthAt10.H.s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt10.H.s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Width at 10% Height', title = 'Peak Width at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 27) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'WidthAt50.H.s.'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'WidthAt50.H.s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'WidthAt50.H.s.'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$WidthAt50.H.s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt50.H.s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Width at 50% Height', title = 'Peak Width at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 28) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt10.H'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt10.H'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt10.H'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$AsymmetryAt10.H - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt10.H)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 10% Height', title = 'Peak Asymmetry at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 29) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt50.H'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt50.H'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'AsymmetryAt50.H'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$AsymmetryAt50.H - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt50.H)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 50% Height', title = 'Peak Asymmetry at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 30) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'PeakCapacity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'PeakCapacity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'PeakCapacity'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$PeakCapacity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), PeakCapacity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Column Peak Capacity', title = 'Column Peak Capacity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 31) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'TimeBeforeFirstExceedanceOf10.MaxIntensity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'TimeBeforeFirstExceedanceOf10.MaxIntensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'TimeBeforeFirstExceedanceOf10.MaxIntensity'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$TimeBeforeFirstExceedanceOf10.MaxIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeBeforeFirstExceedanceOf10.MaxIntensity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time Before First Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 32) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'TimeAfterLastExceedanceOf10.MaxIntensity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'TimeAfterLastExceedanceOf10.MaxIntensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'TimeAfterLastExceedanceOf10.MaxIntensity'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$TimeAfterLastExceedanceOf10.MaxIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeAfterLastExceedanceOf10.MaxIntensity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time After Last Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 33) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'FractionOfRunAbove10.MaxIntensity'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'FractionOfRunAbove10.MaxIntensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'FractionOfRunAbove10.MaxIntensity'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$FractionOfRunAbove10.MaxIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), FractionOfRunAbove10.MaxIntensity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Run Fraction', title = 'Fraction of Run With Eluting Peaks') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 34) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio3to2'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio3to2'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio3to2'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$IdChargeRatio3to2 - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio3to2)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 3 to 2)', title = 'Charge Ratio of z = 3 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 35) {
        minBorder = 0
        maxBorder = round(max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio4to2'], na.rm = TRUE) + (max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio4to2'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(qc_data[xaxis_first:xaxis_last,'IdChargeRatio4to2'], na.rm = TRUE) / 10), 5)
        qc_data$outlier_diff = abs(qc_data$IdChargeRatio4to2 - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio4to2)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 4 to 2)', title = 'Charge Ratio of z = 4 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
    }
    
    else if ((input$xaxis_radio == 2) & (input$yaxis_radio == 2) & (input$outlier_radio == 1)){
      
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
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Total Analysis Time (minutes)', title = 'Total Analysis Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 3) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs1Scans)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS1 Scans', title = 'MS1 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 4) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs2Scans)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS2 Scans', title = 'MS2 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 5) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs3Scans)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS3 Scans', title = 'MS3 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 6) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms1ScanRate..s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS1 Scan Rate (Hz)', title = 'MS1 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 7) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms2ScanRate..s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS2 Scan Rate (Hz)', title = 'MS2 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 8) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms3ScanRate..s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS3 Scan Rate (Hz)', title = 'MS3 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 9) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanDutyCycle.s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mean Duty Cycle (s)', title = 'Duty Cycle') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 10) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanMs2TriggerRate..Ms1Scan.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mean Number of MS2 Triggered per MS1', title = 'MS2 Trigger Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 11) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms1MedianSummedIntensity))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS1 Intensity)', title = 'MS1 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 12) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms2MedianSummedIntensity))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS2 Intensity)', title = 'MS2 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 13) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(MedianPrecursorIntensity))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Precursor Intensity)', title = 'Precursor Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 14) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2PeakFractionConsumingTop80PercentTotalIntensity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Fraction of MS2 Peaks', title = 'MS2 Peak Fraction') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 15) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), EsiInstabilityFlags.count.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of Scans with >10% Deviation from Bordering', title = 'Electrospray Stability') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 16) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMassDrift.ppm.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mass Drift (ppm)', title = 'Mass Analyzer Drift') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 17) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdentificationRate.IDs.Ms2Scan.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Spectra Identification Rate', title = 'Spectral Identification Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 18) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), DigestionEfficiency)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Digestion Efficiency', title = 'Digestion Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 19) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MissedCleavageRate..PSM.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Missed Cleavage Rate', title = 'Missed Cleavage Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 20) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtNTerm)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'N-term Labeling Efficiency', title = 'N-term Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 21) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtK)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Lysine Labeling Efficiency', title = 'Lysine Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 22) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtX)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Modification Labeling Efficiency', title = 'Specified Modification Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 23) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMsFillTime.ms.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS1 Ion Injection Time', title = 'MS1 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 24) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2FillTime.ms.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS2 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 25) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs3FillTime.ms.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS3 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 26) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt10.H.s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Width at 10% Height', title = 'Peak Width at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 27) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt50.H.s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Width at 50% Height', title = 'Peak Width at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 28) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt10.H)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 10% Height', title = 'Peak Asymmetry at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 29) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt50.H)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 50% Height', title = 'Peak Asymmetry at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 30) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), PeakCapacity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Column Peak Capacity', title = 'Column Peak Capacity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 31) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeBeforeFirstExceedanceOf10.MaxIntensity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time Before First Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 32) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeAfterLastExceedanceOf10.MaxIntensity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time After Last Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 33) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), FractionOfRunAbove10.MaxIntensity)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Run Fraction', title = 'Fraction of Run With Eluting Peaks') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 34) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio3to2)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 3 to 2)', title = 'Charge Ratio of z = 3 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 35) {
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio4to2)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 4 to 2)', title = 'Charge Ratio of z = 4 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
    }
    
    else if ((input$xaxis_radio == 2) & (input$yaxis_radio == 2) & (input$outlier_radio == 2)){
      
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
      
      outlier_optimum_value = input$out_val
      outlier_range = input$range_val
      
      if (input$qc_plot_id == 2) {
        qc_data$outlier_diff = abs(qc_data$TotalAnalysisTime - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TotalAnalysisTime)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Total Analysis Time (minutes)', title = 'Total Analysis Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 3) {
        qc_data$outlier_diff = abs(qc_data$NumMs1Scans - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs1Scans)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS1 Scans', title = 'MS1 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 4) {
        qc_data$outlier_diff = abs(qc_data$NumMs2Scans - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs2Scans)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS2 Scans', title = 'MS2 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 5) {
        qc_data$outlier_diff = abs(qc_data$NumMs3Scans - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), NumMs3Scans)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of MS3 Scans', title = 'MS3 Scans') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 6) {
        qc_data$outlier_diff = abs(qc_data$Ms1ScanRate..s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms1ScanRate..s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS1 Scan Rate (Hz)', title = 'MS1 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 7) {
        qc_data$outlier_diff = abs(qc_data$Ms2ScanRate..s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms2ScanRate..s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS2 Scan Rate (Hz)', title = 'MS2 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 8) {
        qc_data$outlier_diff = abs(qc_data$Ms3ScanRate..s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), Ms3ScanRate..s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'MS3 Scan Rate (Hz)', title = 'MS3 Scan Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 9) {
        qc_data$outlier_diff = abs(qc_data$MeanDutyCycle.s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanDutyCycle.s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mean Duty Cycle (s)', title = 'Duty Cycle') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 10) {
        qc_data$outlier_diff = abs(qc_data$MeanMs2TriggerRate..Ms1Scan. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MeanMs2TriggerRate..Ms1Scan.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mean Number of MS2 Triggered per MS1', title = 'MS2 Trigger Rate') +
          theme(axis.text.x = element_text(size = 12)) +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 11) {
        qc_data$outlier_diff = abs(log10(qc_data$Ms1MedianSummedIntensity) - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms1MedianSummedIntensity))) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS1 Intensity)', title = 'MS1 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 12) {
        qc_data$outlier_diff = abs(log10(qc_data$Ms2MedianSummedIntensity) - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(Ms2MedianSummedIntensity))) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Summed MS2 Intensity)', title = 'MS2 Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 13) {
        qc_data$outlier_diff = abs(log10(qc_data$MedianPrecursorIntensity) - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), log10(MedianPrecursorIntensity))) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'log10(Median Precursor Intensity)', title = 'Precursor Intensity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 14) {
        qc_data$outlier_diff = abs(qc_data$MedianMs2PeakFractionConsumingTop80PercentTotalIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2PeakFractionConsumingTop80PercentTotalIntensity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Fraction of MS2 Peaks', title = 'MS2 Peak Fraction') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 15) {
        qc_data$outlier_diff = abs(qc_data$EsiInstabilityFlags.count. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), EsiInstabilityFlags.count.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Number of Scans with >10% Deviation from Bordering', title = 'Electrospray Stability') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 16) {
        qc_data$outlier_diff = abs(qc_data$MedianMassDrift.ppm. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMassDrift.ppm.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Mass Drift (ppm)', title = 'Mass Analyzer Drift') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 17) {
        qc_data$outlier_diff = abs(qc_data$IdentificationRate.IDs.Ms2Scan. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdentificationRate.IDs.Ms2Scan.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Spectra Identification Rate', title = 'Spectral Identification Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 18) {
        qc_data$outlier_diff = abs(qc_data$DigestionEfficiency - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), DigestionEfficiency)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Digestion Efficiency', title = 'Digestion Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 19) {
        qc_data$outlier_diff = abs(qc_data$MissedCleavageRate..PSM. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MissedCleavageRate..PSM.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Missed Cleavage Rate', title = 'Missed Cleavage Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 20) {
        qc_data$outlier_diff = abs(qc_data$ModificationFrequencyAtNTerm - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtNTerm)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'N-term Labeling Efficiency', title = 'N-term Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 21) {
        qc_data$outlier_diff = abs(qc_data$ModificationFrequencyAtK - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtK)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Lysine Labeling Efficiency', title = 'Lysine Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 22) {
        qc_data$outlier_diff = abs(qc_data$ModificationFrequencyAtX - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), ModificationFrequencyAtX)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Modification Labeling Efficiency', title = 'Specified Modification Labeling Efficiency') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 23) {
        qc_data$outlier_diff = abs(qc_data$MedianMsFillTime.ms. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMsFillTime.ms.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS1 Ion Injection Time', title = 'MS1 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 24) {
        qc_data$outlier_diff = abs(qc_data$MedianMs2FillTime.ms. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs2FillTime.ms.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS2 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 25) {
        qc_data$outlier_diff = abs(qc_data$MedianMs3FillTime.ms. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), MedianMs3FillTime.ms.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Median MS2 Ion Injection Time', title = 'MS3 Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 26) {
        qc_data$outlier_diff = abs(qc_data$WidthAt10.H.s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt10.H.s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Width at 10% Height', title = 'Peak Width at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 27) {
        qc_data$outlier_diff = abs(qc_data$ WidthAt50.H.s. - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), WidthAt50.H.s.)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Width at 50% Height', title = 'Peak Width at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 28) {
        qc_data$outlier_diff = abs(qc_data$AsymmetryAt10.H - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt10.H)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 10% Height', title = 'Peak Asymmetry at 10% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 29) {
        qc_data$outlier_diff = abs(qc_data$AsymmetryAt50.H - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), AsymmetryAt50.H)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Peak Asymmetry at 50% Height', title = 'Peak Asymmetry at 50% Height') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 30) {
        qc_data$outlier_diff = abs(qc_data$PeakCapacity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), PeakCapacity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Column Peak Capacity', title = 'Column Peak Capacity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 31) {
        qc_data$outlier_diff = abs(qc_data$TimeBeforeFirstExceedanceOf10.MaxIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeBeforeFirstExceedanceOf10.MaxIntensity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time Before First Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 32) {
        qc_data$outlier_diff = abs(qc_data$TimeAfterLastExceedanceOf10.MaxIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), TimeAfterLastExceedanceOf10.MaxIntensity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Time (minutes)', title = 'Time After Last Peak Elution') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 33) {
        qc_data$outlier_diff = abs(qc_data$FractionOfRunAbove10.MaxIntensity - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), FractionOfRunAbove10.MaxIntensity)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Run Fraction', title = 'Fraction of Run With Eluting Peaks') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 34) {
        qc_data$outlier_diff = abs(qc_data$IdChargeRatio3to2 - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio3to2)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
          labs(x = "Injection Number", y = 'Charge Ratio (z = 3 to 2)', title = 'Charge Ratio of z = 3 to 2') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_x_continuous(limits = c(xaxis_first,xaxis_last), breaks = seq(xaxis_first,xaxis_last,xaxis_interval))
        output_plot}
      
      else if (input$qc_plot_id == 35) {
        qc_data$outlier_diff = abs(qc_data$IdChargeRatio4to2 - outlier_optimum_value)
        qc_data$colors = ifelse(qc_data$outlier_diff > (outlier_optimum_value * (outlier_range / 100)), 'red', brewer.pal(3,'PuBuGn')[3]) 
        output_plot = ggplot(qc_data, aes(as.numeric(injection), IdChargeRatio4to2)) +
          geom_point(fill = qc_data$colors, pch = 21, size = 3, colour = 'black', alpha = 0.8, stroke = 1) +
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


  output$scan_plots = renderPlot({
    
    scanFile = input$scan_file
    
    if (is.null(scanFile))
      return(NULL)
    
    scan_data = read.table(scanFile$datapath, header = TRUE, sep = '\t')
    
    
    if (input$scan_xaxis_radio == 1){
      
      scan_xaxis_minborder = 0
      scan_xaxis_maxborder = max(scan_data$ParentScanRetTime, na.rm=TRUE)
      scan_xaxis_interval = max(scan_data$ParentScanRetTime, na.rm=TRUE) / 10
      
      if (input$scan_plot_id == 2) {
        minBorder = 0
        maxBorder = round(max(scan_data[,'MS2ScansPerCycle'], na.rm = TRUE) + (max(scan_data[,'MS2ScansPerCycle'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data[,'MS2ScansPerCycle'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, MS2ScansPerCycle)) +
          geom_line(colour = brewer.pal(3,'PuBuGn')[3], size = 1.5, alpha = 0.75) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Number of Triggered MS2 Scans', title = 'MS2 Scans Triggered per MS1') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 3) {
        minBorder = 0
        maxBorder = round(max(scan_data[,'DutyCycle'], na.rm = TRUE) + (max(scan_data[,'DutyCycle'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data[,'DutyCycle'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, DutyCycle)) +
          geom_line(colour = brewer.pal(3,'PuBuGn')[3], size = 1.5, alpha = 0.75) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Duty Cycle Duration (s)', title = 'Duty Cycle Duration') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 4) {
        minBorder = 0
        maxBorder = round(max(scan_data[,'ParentIonMass'], na.rm = TRUE) + (max(scan_data[,'ParentIonMass'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data[,'ParentIonMass'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, ParentIonMass)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Mass to Charge of Parent Ion', title = 'Parent Ion Mass') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 5) {
        minBorder = 0
        maxBorder = round(max(scan_data[,'PrecursorCharge'], na.rm = TRUE) + (max(scan_data[,'PrecursorCharge'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data[,'PrecursorCharge'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, PrecursorCharge)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Charge State', title = 'Charge State of Precursor Ions') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 6) {
        minBorder = 0
        maxBorder = round(max(log10(scan_data[,'ParentPeakArea']), na.rm = TRUE) + (max(log10(scan_data[,'ParentPeakArea']), na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(log10(scan_data[,'ParentPeakArea']), na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, log10(ParentPeakArea))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'log10(Peak Area)', title = 'Parent Peak Area at Selection') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 7) {
        minBorder = 0
        maxBorder = round(max(scan_data[,'BaseLinePeakWidth.s.'], na.rm = TRUE) + (max(scan_data[,'BaseLinePeakWidth.s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data[,'BaseLinePeakWidth.s.'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, BaseLinePeakWidth.s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Baseline Peak Width (min)', title = 'Baseline Parent Peak Width') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 8) {
        minBorder = 0
        maxBorder = round(max(scan_data[,'MS1IonInjectionTime'], na.rm = TRUE) + (max(scan_data[,'MS1IonInjectionTime'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data[,'MS1IonInjectionTime'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, MS1IonInjectionTime)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Ion Injection Time (ms)', title = 'MS1 Scan Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 9) {
        minBorder = 0
        maxBorder = round(max(scan_data[,'MS2IonInjectionTime'], na.rm = TRUE) + (max(scan_data[,'MS2IonInjectionTime'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data[,'MS2IonInjectionTime'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, MS2IonInjectionTime)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Ion Injection Time (ms)', title = 'MS2 Scan Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 10) {
        scan_data$bin_group = cut(scan_data$ParentScanRetTime, breaks = seq(min(scan_data$ParentScanRetTime, na.rm = TRUE), max(scan_data$ParentScanRetTime, na.rm = TRUE), 0.5))
        scan_data$numMS2 = 1
        scan_data_compress = setDT(scan_data)[,lapply(.SD,sum,na.rm=TRUE),by=.(bin_group),.SDcols='numMS2']
        setDF(scan_data_compress)
        scan_data_ordered = scan_data_compress[order(scan_data_compress$bin_group),]
        scan_data_ordered$bin_start = as.numeric(sub('\\((.*?)\\,.*','\\1',scan_data_ordered$bin_group))
        scan_data_ordered$bin_end = as.numeric(sub('.*\\,(.*?)\\].*','\\1',scan_data_ordered$bin_group))
        scan_data_ordered$hertz = scan_data_ordered$numMS2 / 30
        
        minBorder = 0
        maxBorder = round(max(scan_data_ordered[,'hertz'], na.rm = TRUE) + (max(scan_data_ordered[,'hertz'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data_ordered[,'hertz'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data_ordered, aes(bin_end, hertz)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.9, stroke = 1) +
          labs(x = "Retention Time (s)", y = 'Scan Rate (Hz)', title = 'MS2 Scan Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 11) {
        minBorder = 0
        maxBorder = round(max(scan_data[,'MS1IsolationInterference'], na.rm = TRUE) + (max(scan_data[,'MS1IsolationInterference'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data[,'MS1IsolationInterference'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, MS1IsolationInterference)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Isolation Interference', title = 'Parent Ion Purity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
    }
    
    else if (input$scan_xaxis_radio == 2){
      
      scan_xaxis_minborder = input$scan_xaxis_min
      scan_xaxis_maxborder = input$scan_xaxis_max
      scan_xaxis_interval = input$scan_xaxis_int
      
      if (input$scan_plot_id == 2) {
        minBorder = 0
        maxBorder = round(max(scan_data[,'MS2ScansPerCycle'], na.rm = TRUE) + (max(scan_data[,'MS2ScansPerCycle'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data[,'MS2ScansPerCycle'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, MS2ScansPerCycle)) +
          geom_line(colour = brewer.pal(3,'PuBuGn')[3], size = 1.5, alpha = 0.75) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Number of Triggered MS2 Scans', title = 'MS2 Scans Triggered per MS1') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 3) {
        minBorder = 0
        maxBorder = round(max(scan_data[,'DutyCycle'], na.rm = TRUE) + (max(scan_data[,'DutyCycle'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data[,'DutyCycle'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, DutyCycle)) +
          geom_line(colour = brewer.pal(3,'PuBuGn')[3], size = 1.5, alpha = 0.75) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Duty Cycle Duration (s)', title = 'Duty Cycle Duration') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 4) {
        minBorder = 0
        maxBorder = round(max(scan_data[,'ParentIonMass'], na.rm = TRUE) + (max(scan_data[,'ParentIonMass'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data[,'ParentIonMass'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, ParentIonMass)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Mass to Charge of Parent Ion', title = 'Parent Ion Mass') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 5) {
        minBorder = 0
        maxBorder = round(max(scan_data[,'PrecursorCharge'], na.rm = TRUE) + (max(scan_data[,'PrecursorCharge'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data[,'PrecursorCharge'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, PrecursorCharge)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Charge State', title = 'Charge State of Precursor Ions') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 6) {
        minBorder = 0
        maxBorder = round(max(log10(scan_data[,'ParentPeakArea']), na.rm = TRUE) + (max(log10(scan_data[,'ParentPeakArea']), na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(log10(scan_data[,'ParentPeakArea']), na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, log10(ParentPeakArea))) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'log10(Peak Area)', title = 'Parent Peak Area at Selection') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 7) {
        minBorder = 0
        maxBorder = round(max(scan_data[,'BaseLinePeakWidth.s.'], na.rm = TRUE) + (max(scan_data[,'BaseLinePeakWidth.s.'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data[,'BaseLinePeakWidth.s.'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, BaseLinePeakWidth.s.)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Baseline Peak Width (min)', title = 'Baseline Parent Peak Width') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 8) {
        minBorder = 0
        maxBorder = round(max(scan_data[,'MS1IonInjectionTime'], na.rm = TRUE) + (max(scan_data[,'MS1IonInjectionTime'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data[,'MS1IonInjectionTime'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, MS1IonInjectionTime)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Ion Injection Time (ms)', title = 'MS1 Scan Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 9) {
        minBorder = 0
        maxBorder = round(max(scan_data[,'MS2IonInjectionTime'], na.rm = TRUE) + (max(scan_data[,'MS2IonInjectionTime'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data[,'MS2IonInjectionTime'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, MS2IonInjectionTime)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Ion Injection Time (ms)', title = 'MS2 Scan Ion Injection Time') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 10) {
        scan_data$bin_group = cut(scan_data$ParentScanRetTime, breaks = seq(min(scan_data$ParentScanRetTime, na.rm = TRUE), max(scan_data$ParentScanRetTime, na.rm = TRUE), 0.5))
        scan_data$numMS2 = 1
        scan_data_compress = setDT(scan_data)[,lapply(.SD,sum,na.rm=TRUE),by=.(bin_group),.SDcols='numMS2']
        setDF(scan_data_compress)
        scan_data_ordered = scan_data_compress[order(scan_data_compress$bin_group),]
        scan_data_ordered$bin_start = as.numeric(sub('\\((.*?)\\,.*','\\1',scan_data_ordered$bin_group))
        scan_data_ordered$bin_end = as.numeric(sub('.*\\,(.*?)\\].*','\\1',scan_data_ordered$bin_group))
        scan_data_ordered$hertz = scan_data_ordered$numMS2 / 30
        
        minBorder = 0
        maxBorder = round(max(scan_data_ordered[,'hertz'], na.rm = TRUE) + (max(scan_data_ordered[,'hertz'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data_ordered[,'hertz'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data_ordered, aes(bin_end, hertz)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.9, stroke = 1) +
          labs(x = "Retention Time (s)", y = 'Scan Rate (Hz)', title = 'MS2 Scan Rate') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
      
      else if (input$scan_plot_id == 11) {
        minBorder = 0
        maxBorder = round(max(scan_data[,'MS1IsolationInterference'], na.rm = TRUE) + (max(scan_data[,'MS1IsolationInterference'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(scan_data[,'MS1IsolationInterference'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(scan_data, aes(ParentScanRetTime, MS1IsolationInterference)) +
          geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Isolation Interference', title = 'Parent Ion Purity') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(scan_xaxis_minborder,scan_xaxis_maxborder), breaks = seq(scan_xaxis_minborder,scan_xaxis_maxborder,scan_xaxis_interval))
        output_plot}
    }
    
  })
  
  output$scan_descriptions = renderText({
    
    if (input$scan_plot_id == 2) {
      paste('This is an overlaid scatter and line plot depicting the number of MS2 scan events triggered by a given MS1 scan (topN). Only MS1
            scans where an MS2 event was triggered are plotted.')}
    else if (input$scan_plot_id == 3) {
      paste('This is an overlaid scatter and line plot depicting the duty cycle (in seconds) for cycles where an MS2 event was triggered. Duty cycle
            is calculated as a time duration from an MS1 event and all subsequent triggered MS2 events until the next MS1 scan.')}
    else if (input$scan_plot_id == 4) {
      paste('This is a scatter plot of the observed mass-to-charge values for precursors that triggered MS2 scans during the acquisition.')}
    else if (input$scan_plot_id == 5) {
      paste('This is a scatter plot of the observed charge states for precursors that triggered MS2 scans during the acquisition.')}
    else if (input$scan_plot_id == 6) {
      paste('This is a scatter plot of the area of the precursor peaks that triggered MS2 scans across the entire acquisition window. The area
            is calculated by searching in the preceeding and following scans where a precursor was triggered to get an elution window that is then
            fitted with a curve. The area under the fitted curve is used as the precursor area.')}
    else if (input$scan_plot_id == 7) {
      paste('This is a scatter plot of the peak width at baseline (10% peak height). Peak widths for all precursor masses that triggered an MS2
            scan are plotted.')}
    else if (input$scan_plot_id == 8) {
      paste('This is a scatter plot of the injection times of MS1 scan events across the acquisition window. Values are presented as milliseconds.')}
    else if (input$scan_plot_id == 9) {
      paste('This is a scatter plot of the injection times of MS2 scan events across the acquisition window. Values are presented as milliseconds.')}
    else if (input$scan_plot_id == 10) {
      paste('This is a scatter plot of the MS2 scan rate across the entire acquisition window. Scan rate is calculated as the number of MS2 scans per second (Hz). Scan
            rates are presented as binned values across 30 second windows spanning the acquisition length.')}
    else if (input$scan_plot_id == 11) {
      paste('This is a scatter plot of the purity of precursors selected for MS2 scans. Purity is defined as the percentage of signal in a defined
            isolation window (pulled from the instrument method by RawTools) that belongs to the selected parent ion. The values range from 0 (0%) to 1
            (100%) on the plot.')}
    
      
  })
  
  
  output$chro_plots = renderPlot({
    
    chroFile = input$chro_file
    
    if (is.null(chroFile))
      return(NULL)
    
    chro_data = read.table(chroFile$datapath, header = TRUE, sep = '\t')
    
    
    if (input$chro_xaxis_radio == 1){
      
      chro_xaxis_minborder = 0
      chro_xaxis_maxborder = max(chro_data$RetentionTime, na.rm=TRUE)
      chro_xaxis_interval = max(chro_data$RetentionTime, na.rm=TRUE) / 10
      
        minBorder = 0
        maxBorder = round(max(chro_data[,'Intensity'], na.rm = TRUE) + (max(chro_data[,'Intensity'], na.rm = TRUE)*0.1), 2) 
        intBorder = round((max(chro_data[,'Intensity'], na.rm = TRUE) / 10), 2)
        output_plot = ggplot(chro_data, aes(RetentionTime, Intensity)) +
          geom_line(colour = brewer.pal(3,'PuBuGn')[3], size = 1.5, alpha = 0.75) +
          #geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
          labs(x = "Retention Time (min)", y = 'Intensity', title = 'Selected Chromatogram') +
          theme(axis.text.x = element_text(size = 12), legend.position="none") +
          scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
          scale_x_continuous(limits = c(chro_xaxis_minborder,chro_xaxis_maxborder), breaks = seq(chro_xaxis_minborder,chro_xaxis_maxborder,chro_xaxis_interval))
        output_plot}
    
    else if (input$chro_xaxis_radio == 2){
      
      chro_xaxis_minborder = input$chro_xaxis_min
      chro_xaxis_maxborder = input$chro_xaxis_max
      chro_xaxis_interval = input$chro_xaxis_int
      
      minBorder = 0
      maxBorder = round(max(chro_data[,'Intensity'], na.rm = TRUE) + (max(chro_data[,'Intensity'], na.rm = TRUE)*0.1), 2) 
      intBorder = round((max(chro_data[,'Intensity'], na.rm = TRUE) / 10), 2)
      output_plot = ggplot(chro_data, aes(RetentionTime, Intensity)) +
        geom_line(colour = brewer.pal(3,'PuBuGn')[3], size = 1.5, alpha = 0.75) +
        #geom_point(fill = brewer.pal(3,'PuBuGn')[3], pch = 21, size = 3, colour = 'black', alpha = 0.2, stroke = 0.1) +
        labs(x = "Retention Time (min)", y = 'Intensity', title = 'Selected Chromatogram') +
        theme(axis.text.x = element_text(size = 12), legend.position="none") +
        scale_y_continuous(limits = c(minBorder,maxBorder), breaks = seq(minBorder,maxBorder,intBorder)) +
        scale_x_continuous(limits = c(chro_xaxis_minborder,chro_xaxis_maxborder), breaks = seq(chro_xaxis_minborder,chro_xaxis_maxborder,chro_xaxis_interval))
      output_plot}

    })

})



#####################################################################################################
#Run the application 
shinyApp(ui = ui, server = server)

