namespace Framework.Stress

module Program =
  open TestRunner
  open XPlot.Plotly

  [<EntryPoint>]
  let main _ =
    let testLength = 1.
    let pingRates = seq { 900. .. 50. .. 1100.} // this appears to be the regime where it breaks down
    // let pingRates = seq { 0. .. 0.5 .. 1. } |> Seq.map ((fun x -> 10. ** x) >> int >> float)
    let chart = runTests testLength pingRates
    Chart.Show chart
    0
