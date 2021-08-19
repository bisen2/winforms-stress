namespace Framework.Stress

module Types =
  open System.ComponentModel
  open System.Drawing
  open System.Threading
  open System.Windows.Forms
  open System.Windows.Forms.DataVisualization.Charting

  type MyForm () as self =
    inherit Form ()
    // label update test fields
    let myLabel = new Label ()
    let mutable labelUpdated = 0
    // chart update test fields
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

    member _.BumpLabelText () =
      labelUpdated <- labelUpdated + 1
      myLabel.Text <- $"Ping #{labelUpdated}"

    member _.BumpChart () =
      let x =
        match chartXs with
        | [] -> 0
        | x::_ -> x + 1
      chartXs <- x :: chartXs
      myChart.Series.[0].Points.AddXY (x, x) |> ignore

    member _.RebuildChart () =
      let x =
        match chartXs with
        | [] -> 0
        | x::_ -> x + 1
      chartXs <- x :: chartXs
      myChart.Series.[0].Points.DataBindXY (chartXs, chartXs) |> ignore

  type UpdateDelegate = delegate of unit -> unit

  type Stressor (myForm: MyForm, testLength, pingRate) =

    let beginInvokeWorker = new BackgroundWorker ()
    let invokeWorker = new BackgroundWorker ()

    let beginInvokeDoWork (args: DoWorkEventArgs) =
      let updateDelegate = args.Argument :?> UpdateDelegate
      let nPings = pingRate * testLength
      let delay = 1000. / pingRate
      Thread.Sleep 500
      [ 0. .. nPings ]
      |> Seq.iter
        ( fun i ->
            myForm.BeginInvoke updateDelegate |> ignore
            Thread.Sleep (int delay) )

    let invokeDoWork (args: DoWorkEventArgs) =
      let updateDelegate = args.Argument :?> UpdateDelegate
      let nPings = pingRate * testLength
      let delay = 1000. / pingRate
      Thread.Sleep(500)
      [ 0. .. nPings ]
      |> Seq.iter
          ( fun i ->
              myForm.Invoke updateDelegate |> ignore
              Thread.Sleep (int delay) )

    let cleanup args =
      myForm.Invoke (UpdateDelegate (fun _ -> myForm.Close ())) |> ignore
      ()

    do
      beginInvokeWorker.DoWork.Add beginInvokeDoWork
      beginInvokeWorker.RunWorkerCompleted.Add cleanup
      invokeWorker.DoWork.Add invokeDoWork
      invokeWorker.RunWorkerCompleted.Add cleanup

    member _.RunLabelUpdateBeginInvokeStressor () =
      let updateDelegate = UpdateDelegate (fun _ -> myForm.BumpLabelText ())
      beginInvokeWorker.RunWorkerAsync updateDelegate

    member _.RunChartUpdateBeginInvokeStressor () =
      let updateDelegate = UpdateDelegate (fun _ -> myForm.BumpChart ())
      beginInvokeWorker.RunWorkerAsync updateDelegate

    member _.RunChartRebuildBeginInvokeStressor () =
      let updateDelegate = UpdateDelegate (fun _ -> myForm.RebuildChart ())
      beginInvokeWorker.RunWorkerAsync updateDelegate

    member _.RunLabelUpdateInvokeStressor () =
      let updateDelegate = UpdateDelegate (fun _ -> myForm.BumpLabelText ())
      invokeWorker.RunWorkerAsync updateDelegate

    member _.RunChartUpdateInvokeStressor () =
      let updateDelegate = UpdateDelegate (fun _ -> myForm.BumpChart ())
      invokeWorker.RunWorkerAsync updateDelegate

    member _.RunChartRebuildInvokeStressor () =
      let updateDelegate = UpdateDelegate (fun _ -> myForm.RebuildChart ())
      invokeWorker.RunWorkerAsync updateDelegate
