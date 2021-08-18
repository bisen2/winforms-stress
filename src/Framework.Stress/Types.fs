namespace Framework.Stress

module Types =
  open System.ComponentModel
  open System.Drawing
  open System.Threading
  open System.Windows.Forms

  type MyForm () as self =
    inherit Form ()

    let myLabel = new Label ()

    let initializeComponent () =
      self.SuspendLayout ()
      // myLabel
      myLabel.AutoSize <- true
      myLabel.Location <- Point (13, 13)
      myLabel.Name <- "myLabel"
      myLabel.Size <- Size (62, 13)
      myLabel.TabIndex <- 0
      myLabel.Text <- "Hello, World!"
      // myForm
      self.AutoScaleDimensions <- SizeF (6f, 13f)
      self.AutoScaleMode <- AutoScaleMode.Font
      self.ClientSize <- Size (800, 450)
      self.Name <- "MyForm"
      self.Text <- "MyForm"

      self.Controls.Add myLabel
      self.ResumeLayout false
      self.PerformLayout ()

    do initializeComponent ()

    member _.SetLabelText text =
      myLabel.Text <- text

  type UpdateDelegate = delegate of unit -> unit

  type Stressor (myForm: MyForm, testLength, pingRate) =

    let bgw = new BackgroundWorker ()

    let bgwDoWork args =
      let nPings = pingRate * testLength
      let delay = 1000. / pingRate
      Thread.Sleep 500
      [ 0. .. nPings ]
      |> Seq.iter
        ( fun i ->
            myForm.BeginInvoke (UpdateDelegate (fun _ -> myForm.SetLabelText $"Ping #{i}")) |> ignore
            Thread.Sleep (int delay) )

    let bgwDone args =
      myForm.Invoke (UpdateDelegate (fun _ -> myForm.Close ())) |> ignore
      ()

    do
      bgw.DoWork.Add bgwDoWork
      bgw.RunWorkerCompleted.Add bgwDone

    member _.RunStressor () =
      bgw.RunWorkerAsync ()
