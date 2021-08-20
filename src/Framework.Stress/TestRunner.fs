namespace Framework.Stress

module GenericTests =
  open System.Windows.Forms
  open Types

  let invokeTest testLength pingRate (myForm: MyForm) updateDelegate =
    // Run stressor and application in parallel
    Stressor.RunInvokeStressor testLength pingRate myForm updateDelegate
    Application.Run myForm
    // query user for if the application stuttered
    let resp = MessageBox.Show ("Did this form stutter?", "Stutter Query", MessageBoxButtons.YesNo)
    resp = DialogResult.Yes

  let beginInvokeTest testLength pingRate (myForm: MyForm) updateDelegate =
    Stressor.RunBeginInvokeStressor testLength pingRate myForm updateDelegate
    Application.Run myForm
    let resp = MessageBox.Show ("Did this form stutter?", "Stutter Query", MessageBoxButtons.YesNo)
    resp = DialogResult.Yes

  let irregularBeginInvokeTest testLength pingRate (myForm: MyForm) updateDelegate =
    Stressor.RunIrregularBeginInvokeStressor testLength pingRate myForm updateDelegate
    Application.Run myForm
    let resp = MessageBox.Show ("Did this form stutter?", "Stutter Query", MessageBoxButtons.YesNo)
    resp = DialogResult.Yes

module Tests =
  open GenericTests
  open Types

  let labelUpdateBeginInvokeTest testLength pingRate =
    use myForm = new MyForm ()
    beginInvokeTest testLength pingRate myForm (UpdateDelegate (fun () -> myForm.BumpLabelText ()))

  let labelUpdateInvokeTest testLength pingRate =
    use myForm = new MyForm ()
    invokeTest testLength pingRate myForm (UpdateDelegate (fun () -> myForm.BumpLabelText ()))

  let labelUpdateIrregularBeginInvokeTest testLength pingRate =
    use myForm = new MyForm ()
    irregularBeginInvokeTest testLength pingRate myForm (UpdateDelegate (fun () -> myForm.BumpLabelText ()))

  let chartUpdateBeginInvokeTest testLength pingRate =
    use myForm = new MyForm ()
    beginInvokeTest testLength pingRate myForm (UpdateDelegate (fun () -> myForm.BumpChart ()))

  let chartUpdateInvokeTest testLength pingRate =
    use myForm = new MyForm ()
    invokeTest testLength pingRate myForm (UpdateDelegate (fun () -> myForm.BumpChart ()))

  let chartUpdateIrregularBeginInvokeTest testLength pingRate =
    use myForm = new MyForm ()
    irregularBeginInvokeTest testLength pingRate myForm (UpdateDelegate (fun () -> myForm.BumpChart ()))

  let chartRebuildBeginInvokeTest testLength pingRate =
    use myForm = new MyForm ()
    beginInvokeTest testLength pingRate myForm (UpdateDelegate (fun () -> myForm.RebuildChart ()))

  let chartRebuildInvokeTest testLength pingRate =
    use myForm = new MyForm ()
    invokeTest testLength pingRate myForm (UpdateDelegate (fun () -> myForm.RebuildChart ()))

  let chartRebuildIrregularBeginInvokeTest testLength pingRate =
    use myForm = new MyForm ()
    irregularBeginInvokeTest testLength pingRate myForm (UpdateDelegate (fun () -> myForm.RebuildChart ()))

module TestRunner =
  open XPlot.Plotly
  open Tests

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

    [ ( "BI(Update label)",   labelUpdateBeginInvokeTest          );
      ( "BI(Update chart)",   chartUpdateBeginInvokeTest          );
      ( "BI(Rebuild chart)",  chartRebuildBeginInvokeTest         );
      ( "I(Update label)",    labelUpdateInvokeTest               );
      ( "I(Update chart)",    chartUpdateInvokeTest               );
      ( "I(Rebuildchart)",    chartRebuildInvokeTest              );
      ( "IRBI(Update label)", labelUpdateIrregularBeginInvokeTest );
      ( "IRBI(Update chart)", chartUpdateIrregularBeginInvokeTest );
      ( "IRBI(Rebuild chart)",chartRebuildIrregularBeginInvokeTest) ]
    |> List.map (fun (testName, test) -> runTest testName test)
    |> generatePlot
