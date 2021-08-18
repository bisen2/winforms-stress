namespace Framework.Stress

module Program =
  open TestRunner
  open XPlot.Plotly

  [<EntryPoint>]
  let main _ =
    let testLength = 2.
    let pingRates = seq { 0. .. 0.5 .. 5. } |> Seq.map ((fun x -> 10. ** x) >> int >> float)
    let chart = runTests testLength pingRates
    Chart.Show chart
    0
