namespace Framework.Stress

module Program =
  open TestRunner
  open XPlot.Plotly

  let test () =
    let stopwatch = System.Diagnostics.Stopwatch ()
    stopwatch.Start ()
    let xs = [ 1 .. 1000000 ] |> List.map (fun x -> x * x)
    stopwatch.Stop ()
    stopwatch.ElapsedMilliseconds

  [<EntryPoint>]
  let main _ =
    // use myForm = new MyForm ()
    // Application.Run myForm
    let testLength = 2.
    // let pingRates = seq { 0. .. 0.5 .. 4. } |> Seq.map ((fun x -> 10. ** x) >> int >> float)
    let pingRates = seq { 1000. .. 100. .. 1500. } // this appears to be the regime where it breaks down
    // let pingRates = seq { 0. .. 0.5 .. 1. } |> Seq.map ((fun x -> 10. ** x) >> int >> float)
    let chart = runTests testLength pingRates
    Chart.Show chart
    0
