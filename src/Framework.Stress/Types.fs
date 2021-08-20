namespace Framework.Stress

module Forms =
  open System.Drawing
  open System.Windows.Forms
  open System.Windows.Forms.DataVisualization.Charting

  /// A form used as the target of invoke calls
  type MyForm () as self =
    inherit Form ()
    let myLabel = new Label ()
    let mutable labelUpdated = 0
    let mutable chartXs: int list = []
    let myChart = new Chart ()
    let myChartArea = new ChartArea ()
    let myLegend = new Legend ()
    let mySeries = new Series ()

    let initializeComponent () =
      self.SuspendLayout ()
      // myLabel
      myLabel.AutoSize <- true
      myLabel.Location <- Point (13, 13)
      myLabel.Name <- "myLabel"
      myLabel.Size <- Size (62, 13)
      myLabel.TabIndex <- 0
      myLabel.Text <- "Hello, World!"

      // myChart
      myChart.Location <- Point (16, 42)
      myChart.Name <- "MyChart"
      myChart.Size <- Size (300, 300)
      myChart.Text <- "MyChart"

      myChartArea.Name <- "MyChartArea"
      myChart.ChartAreas.Add myChartArea

      myLegend.Name <- "MyLegend"
      myChart.Legends.Add myLegend

      mySeries.ChartArea <- "MyChartArea"
      mySeries.Legend <- "MyLegend"
      mySeries.Name <- "MySeries"
      mySeries.ChartType <- SeriesChartType.Line
      myChart.Series.Add mySeries

      // myForm
      self.AutoScaleDimensions <- SizeF (6f, 13f)
      self.AutoScaleMode <- AutoScaleMode.Font
      self.ClientSize <- Size (800, 450)
      self.Name <- "MyForm"
      self.Text <- "MyForm"

      self.Controls.Add myLabel
      self.Controls.Add myChart
      self.ResumeLayout false
      self.PerformLayout ()

    do initializeComponent ()

    /// Method to update label text (low resource intensity)
    member _.BumpLabelText () =
      labelUpdated <- labelUpdated + 1
      myLabel.Text <- $"Ping #{labelUpdated}"

    /// Method to update chart data (medium resource intensity)
    member _.BumpChart () =
      let x =
        match chartXs with
        | [] -> 0
        | x::_ -> x + 1
      chartXs <- x :: chartXs
      myChart.Series.[0].Points.AddXY (x, x) |> ignore

    /// Method to rebuild the chart (high resource intensity)
    member _.RebuildChart () =
      let x =
        match chartXs with
        | [] -> 0
        | x::_ -> x + 1
      chartXs <- x :: chartXs
      myChart.Series.[0].Points.DataBindXY (chartXs, chartXs) |> ignore

/// Contains functions for pinging repeated invokes on a form
module Stressor =
  open System.ComponentModel
  open System.Threading
  open Forms

  type UpdateDelegate = delegate of unit -> unit

  /// Calls Invoke on the form with the given delegate at the specified frequency
  let RunInvokeStressor testLength pingRate (myForm: MyForm) (updateDelegate: UpdateDelegate) =
    let doWork _ =
      let nPings = pingRate * testLength
      let delay = 1000. / pingRate
      Thread.Sleep(500)
      [ 0. .. nPings ]
      |> Seq.iter
          ( fun i ->
              myForm.Invoke updateDelegate |> ignore
              Thread.Sleep (int delay) )
    let cleanup _ = myForm.Invoke (UpdateDelegate (fun () -> myForm.Close ())) |> ignore
    use bgw = new BackgroundWorker ()
    bgw.DoWork.Add doWork
    bgw.RunWorkerCompleted.Add cleanup
    bgw.RunWorkerAsync ()

  /// Calls BeginInvoke on the form with the given delegate at the specified frequency
  let RunBeginInvokeStressor testLength pingRate (myForm: MyForm) (updateDelegate: UpdateDelegate) =
    let doWork _ =
      let nPings = pingRate * testLength
      let delay = 1000. / pingRate
      Thread.Sleep(500)
      [ 0. .. nPings ]
      |> Seq.iter
          ( fun i ->
              myForm.BeginInvoke updateDelegate |> ignore
              Thread.Sleep (int delay) )
    let cleanup _ = myForm.Invoke (UpdateDelegate (fun () -> myForm.Close ())) |> ignore
    use bgw = new BackgroundWorker ()
    bgw.DoWork.Add doWork
    bgw.RunWorkerCompleted.Add cleanup
    bgw.RunWorkerAsync ()

  /// Calls BeginInvoke on the form with the given delegate at the specified frequency, but with every tenth ping skipping the delay
  let RunIrregularBeginInvokeStressor testLength pingRate (myForm: MyForm) (updateDelegate: UpdateDelegate) =
    let doWork _ =
      let nPings = pingRate * testLength
      let delay = 1000. / pingRate
      Thread.Sleep(500)
      [ 0. .. nPings ]
      |> Seq.iter
          ( fun i ->
              myForm.BeginInvoke updateDelegate |> ignore
              if i % 10. <> 0. then Thread.Sleep (int delay) ) // for every 10th ping, send the next immediately
    let cleanup _ = myForm.Invoke (UpdateDelegate (fun () -> myForm.Close ())) |> ignore
    use bgw = new BackgroundWorker ()
    bgw.DoWork.Add doWork
    bgw.RunWorkerCompleted.Add cleanup
    bgw.RunWorkerAsync ()
