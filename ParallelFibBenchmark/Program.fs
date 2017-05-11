open Hopac
open Hopac.Infixes
open System.Threading.Tasks
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open System
open BenchmarkDotNet.Attributes.Exporters

type FibBenchmarks() =
    let rec fib n = 
        if n < 2L then n
        else fib (n - 1L) + fib (n - 2L)
    
    let rec hfib (n, level) = Job.delay <| fun () ->
        if n < 2L then
            Job.result n
        elif n < level then Job.result (fib n)
        else
            hfib (n-2L, level) <*> hfib (n-1L, level) |> Job.map (fun (x, y) -> x + y)
    
    let rec afib (n, level) = async {
        if n < 2L then
            return n
        elif n < level then return fib n
        else
            let! n2a = afib (n-2L, level) |> Async.StartChild
            let! n1 = afib (n-1L, level)
            let! n2 = n2a
            return n2 + n1
    }
    
    let rec tfib (n, level) =
        if n < 2L then n
        elif n < level then fib n
        else
            let n2t = Task.Factory.StartNew (fun _ -> tfib (n-2L, level))
            let n1 = tfib (n-1L, level)
            n2t.Result + n1

    let n = 20L
    let level = 0L

    [<Benchmark>]
    member __.Fib() = fib n |> ignore

    [<Benchmark>]
    member __.HFib() = hfib (n, level) |> run |> ignore

    [<Benchmark>]
    member __.AFib() = afib (n, level) |> Async.RunSynchronously |> ignore

    [<Benchmark>]
    member __.TFib() = tfib (n, level) |> ignore

type RefRecord = { I: int; J: int }

[<Struct>]
type StructRecord = { I: int; J: int }

[<MarkdownExporter; AsciiDocExporter; HtmlExporter; RPlotExporter>]
type Benchmarks() =
    let rnd = Random()
    let next() = rnd.Next(Int32.MinValue, Int32.MaxValue)
    let n = 10000
    //let refTupleList = List.init n (fun _ -> next(), next())
    //let structTupleList = List.init n (fun _ -> struct (next(), next()))
    let refTupleArray = Array.init n (fun _ -> next(), next())
    //let getFreshRefTupleArray size = Array.init size (fun _ -> next(), next())
    //let structTupleArray = Array.init n (fun _ -> struct (next(), next()))
    //let getFreshStructTupleArray size = Array.init size (fun _ -> struct (next(), next()))

    //let refRecordList = List.init n (fun _ -> { RefRecord.I = next(); J = next() })
    //let structRecordList = List.init n (fun _ -> { StructRecord.I = next(); J = next() })
    //let refRecordArray = Array.init n (fun _ -> { RefRecord.I = next(); J = next() })
    //let refRecordArray1 = Array.copy refRecordArray
    //let structRecordArray = Array.init n (fun _ -> { StructRecord.I = next(); J = next() })
    //let structRecordArray1 = Array.copy structRecordArray

    //[<Params(10, 100, 1_000, 10_000, 100_000, 1_000_000)>]
    //[<Params(10_000)>]
    //member val Size: int = 0 with get, set

    //[<Benchmark>] member __.RefTupleList() = List.sortBy (fun (_, y) -> y) refTupleList |> ignore
    //[<Benchmark>] member __.StructTupleList() = List.sortBy (fun struct (_, y) -> y) structTupleList |> ignore
    //[<Benchmark(Baseline = true)>] member this.RefTupleArray() = Array.sort refTupleArray |> ignore
    //[<Benchmark>] member this.RefTupleArrayByLambda() = Array.sortBy (fun (_, y) -> y) refTupleArray |> ignore
    [<Benchmark>] member this.RefTupleArrayByFunc() = Array.sortBy snd refTupleArray |> ignore
    //[<Benchmark>] member this.RefTupleArrayInPlace() = Array.sortInPlaceBy (fun (_, y) -> y) (getFreshRefTupleArray this.Size)
    //[<Benchmark>] member this.StructTupleArray() = Array.sortBy (fun struct (_, y) -> y) (getFreshStructTupleArray this.Size) |> ignore
    //[<Benchmark>] member this.StructTupleArrayInPlace() = Array.sortInPlaceBy (fun struct (_, y) -> y) (getFreshStructTupleArray this.Size)

    //[<Benchmark>] member __.RefRecordList() = refRecordList |> List.sortBy (fun { J = j } -> j) |> ignore
    //[<Benchmark>] member __.StructRecordList() = structRecordList |> List.sortBy (fun { J = j } -> j) |> ignore
    //[<Benchmark>] member __.RefRecordArray() = refRecordArray |> Array.sortBy (fun { J = j } -> j) |> ignore
    //[<Benchmark>] member __.RefRecordArrayInPlace() = Array.sortInPlace refRecordArray1
    //[<Benchmark>] member __.StructRecordArray() = structRecordArray |> Array.sortBy (fun { J = j } -> j) |> ignore
    //[<Benchmark>] member __.StructRecordArrayInPlace() = Array.sortInPlace structRecordArray1

[<EntryPoint>]
let main _ =
    //BenchmarkRunner.Run<FibBenchmarks>() |> ignore
    BenchmarkRunner.Run<Benchmarks>() |> ignore
    0