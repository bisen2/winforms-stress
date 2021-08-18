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

    let bgw = new BackgroundWorker ()

    let bgwDoWork (args: DoWorkEventArgs) =
      let updateDelegate = args.Argument :?> UpdateDelegate
      let nPings = pingRate * testLength
      let delay = 1000. / pingRate
      Thread.Sleep 500
      [ 0. .. nPings ]
      |> Seq.iter
        ( fun i ->
            myForm.BeginInvoke updateDelegate |> ignore
            Thread.Sleep (int delay) )

    let bgwDone args =
      myForm.Invoke (UpdateDelegate (fun _ -> myForm.Close ())) |> ignore
      ()

    do
      bgw.DoWork.Add bgwDoWork
      bgw.RunWorkerCompleted.Add bgwDone

    member _.RunLabelStressor () =
      let updateDelegate = UpdateDelegate (fun _ -> myForm.BumpLabelText ())
      bgw.RunWorkerAsync updateDelegate

    member _.RunChartStressor () =
      let updateDelegate = UpdateDelegate (fun _ -> myForm.BumpChart ())
      bgw.RunWorkerAsync updateDelegate

    member _.RunChartRebuildStressor () =
      let updateDelegate = UpdateDelegate (fun _ -> myForm.RebuildChart ())
      bgw.RunWorkerAsync updateDelegate
