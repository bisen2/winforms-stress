namespace Framework.Stress

module TestRunner =
  open System.Windows.Forms
  open Types
  open XPlot.Plotly

  let labelUpdateBeginInvokeTest testLength pingRate =
    // run application and stressor in parallel
    use myForm = new MyForm ()
    let stressor = Stressor (myForm, testLength, pingRate)
    stressor.RunLabelUpdateBeginInvokeStressor ()
    Application.Run myForm
    // Check with user if application stuttered
    let resp = MessageBox.Show ("Did this form stutter?", "Stutter Query", MessageBoxButtons.YesNo)
    resp = DialogResult.Yes

  let labelUpdateInvokeTest testLength pingRate =
    use myForm = new MyForm ()
    let stressor = Stressor (myForm, testLength, pingRate)
    stressor.RunLabelUpdateInvokeStressor ()
    Application.Run myForm
    let resp = MessageBox.Show ("Did this form stutter?", "Stutter Query", MessageBoxButtons.YesNo)
    resp = DialogResult.Yes

  let chartUpdateBeginInvokeTest testLength pingRate =
    use myForm = new MyForm ()
    let stressor = Stressor (myForm, testLength, pingRate)
    stressor.RunChartUpdateBeginInvokeStressor ()
    Application.Run myForm
    let resp = MessageBox.Show ("Did this form stutter?", "Stutter Query", MessageBoxButtons.YesNo)
    resp = DialogResult.Yes

  let chartUpdateInvokeTest testLength pingRate =
    use myForm = new MyForm ()
    let stressor = Stressor (myForm, testLength, pingRate)
    stressor.RunChartUpdateInvokeStressor ()
    Application.Run myForm
    let resp = MessageBox.Show ("Did this form stutter?", "Stutter Query", MessageBoxButtons.YesNo)
    resp = DialogResult.Yes

  let chartRebuildBeginInvokeTest testLength pingRate =
    use myForm = new MyForm ()
    let stressor = Stressor (myForm, testLength, pingRate)
    stressor.RunChartRebuildBeginInvokeStressor ()
    Application.Run myForm
    let resp = MessageBox.Show ("Did this form stutter?", "Stutter Query", MessageBoxButtons.YesNo)
    resp = DialogResult.Yes

  let chartRebuildInvokeTest testLength pingRate =
    use myForm = new MyForm ()
    let stressor = Stressor (myForm, testLength, pingRate)
    stressor.RunChartRebuildInvokeStressor ()
    Application.Run myForm
    let resp = MessageBox.Show ("Did this form stutter?", "Stutter Query", MessageBoxButtons.YesNo)
    resp = DialogResult.Yes

  let runTests testLength pingRates =

    let generateTrace name data =
      Scatter (
        x = (data |> List.map (fun (x, _) -> x)),
        y = (data |> List.map (fun (_, y) -> y)),
        name = name,
        mode = "markers",
        marker = Marker ( size = 10 ) )

    let generatePlot (traces: Scatter list) =
      let layout =
        Layout (
          title = "Application stutter as a function of ping rate",
          xaxis = Xaxis ( title = "Ping rate" ),
          yaxis = Yaxis ( title = "Stutter" ) )
      traces
      |> Chart.Plot
      |> Chart.WithLayout layout

    let runTest testName test =
      pingRates
      |> Seq.map (fun pingRates -> (pingRates, test testLength pingRates))
      |> Seq.toList
      |> fun result -> generateTrace testName result

    [ ( "Update label (BeginInvoke)",   labelUpdateBeginInvokeTest  );
      ( "Update chart (BeginInvoke)",   chartUpdateBeginInvokeTest  );
      ( "Rebuild chart (BeginInvoke)",  chartRebuildBeginInvokeTest );
      ( "Update label (Invoke)",        labelUpdateInvokeTest       );
      ( "Update chart (Invoke)",        chartUpdateInvokeTest       );
      ( "Rebuildchart (Invoke)",        chartRebuildInvokeTest      ) ]
    |> List.map (fun (testName, test) -> runTest testName test)
    |> generatePlot
