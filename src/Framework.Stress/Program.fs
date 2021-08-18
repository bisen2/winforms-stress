namespace Framework.Stress

module Program =
  open System.Diagnostics
  open System.Windows.Forms
  open Types
  open XPlot.Plotly

  let runTest testLength pingRate =
    // run application and stressor in parallel, timing execution
    use myForm = new MyForm ()
    let stressor = Stressor (myForm, testLength, pingRate)
    stressor.RunStressor ()
    Application.Run(myForm)
    // Check with user if application stuttered
    let resp = MessageBox.Show ("Did this form stutter?", "Stutter Query", MessageBoxButtons.YesNo)
    resp = DialogResult.Yes

  let generatePlot results =
    let trace =
      Scatter (
        x = (results |> List.map (fun (x, _) -> x)),
        y = (results |> List.map (fun (_, y) -> y)),
        mode = "markers",
        marker = Marker (
          color = "rgb (164, 194, 244",
          size = 10 ) )
    let layout =
      Layout (
        title = "Application stutter as a function of ping rate",
        xaxis = Xaxis (
          title = "Ping rate",
          ``type`` = "log" ),
        yaxis = Yaxis (
          title = "Stutter (0=false, 1=true)" ) )
    trace
    |> Chart.Plot
    |> Chart.WithLayout layout

  [<EntryPoint>]
  let main _ =
    let testLength = 2.
    let pingRates = seq { 0. .. 0.5 .. 6. } |> Seq.map ((fun x -> 10. ** x) >> int >> float)
    let results =
      pingRates
      |> Seq.map
          (fun pingRate ->
            (pingRate, runTest testLength pingRate))
      |> Seq.toList
    let chart = generatePlot results
    Chart.Show chart
    0
