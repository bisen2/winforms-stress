namespace Framework.Stress

module TestRunner =
  open System.Windows.Forms
  open Types
  open XPlot.Plotly

  let labelUpdateTest testLength pingRate =
    // run application and stressor in parallel
    use myForm = new MyForm ()
    let stressor = Stressor (myForm, testLength, pingRate)
    stressor.RunLabelStressor ()
    Application.Run myForm
    // Check with user if application stuttered
    let resp = MessageBox.Show ("Did this form stutter?", "Stutter Query", MessageBoxButtons.YesNo)
    resp = DialogResult.Yes

  let chartUpdateTest testLength pingRate =
    // run application and stressor in parallel
    use myForm = new MyForm ()
    let stressor = Stressor (myForm, testLength, pingRate)
    stressor.RunChartStressor ()
    Application.Run myForm
    // Check with user if application stuttered
    let resp = MessageBox.Show ("Did this form stutter?", "Stutter Query", MessageBoxButtons.YesNo)
    resp = DialogResult.Yes

  let chartRebuildTest testLength pingRate =
    // run application and stressor in parallel
    use myForm = new MyForm ()
    let stressor = Stressor (myForm, testLength, pingRate)
    stressor.RunChartRebuildStressor ()
    Application.Run myForm
    // Check with user if application stuttered
    let resp = MessageBox.Show ("Did this form stutter?", "Stutter Query", MessageBoxButtons.YesNo)
    resp = DialogResult.Yes

  let generateTrace name data =
    Scatter (
      x = (data |> List.map (fun (x, _) -> x)),
      y = (data |> List.map (fun (_, y) -> y)),
      name = name,
      mode = "markers",
      marker = Marker ( size = 10 ) )

  let generatePlot (traces: Trace list) =
    let layout =
      Layout (
        title = "Application stutter as a function of ping rate",
        xaxis = Xaxis (
          title = "Ping rate",
          ``type`` = "log" ),
        yaxis = Yaxis (
          title = "Stutter" ) )
    traces
    |> Chart.Plot
    |> Chart.WithLayout layout

  let runTests testLength pingRates =
    let labelUpdateResults =
      pingRates
      |> Seq.map (fun pingRate -> (pingRate, labelUpdateTest testLength pingRate))
      |> Seq.toList
    let chartUpdateResults =
      pingRates
      |> Seq.map (fun pingRate -> (pingRate, chartUpdateTest testLength pingRate))
      |> Seq.toList
    let chartRebuildResults =
      pingRates
      |> Seq.map (fun pingRate -> (pingRate, chartRebuildTest testLength pingRate))
      |> Seq.toList
    let labelUpdateTrace = generateTrace "Update Label text" labelUpdateResults
    let chartUpdateTrace = generateTrace "Update Chart" chartUpdateResults
    let chartRebuildTrace = generateTrace "Rebuild Chart" chartRebuildResults
    generatePlot [ labelUpdateTrace; chartUpdateTrace; chartRebuildTrace ]
