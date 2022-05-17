module SpRequestMvu


open System
open System.Linq
open Elmish.WPF
open PuppyData.Types
open PuppyData.SqlMapper

open Serilog
open Serilog.Extensions.Logging
open System.Data.SqlClient

type Model =
    { 
        SpParams: SpParamMvu.Model seq
    }

let init (spParams: SpParamMvu.Model seq) () =
  { SpParams = spParams }

type Msg =
  | SubmitRequest
  | NewValue of (string*string)

let update msg m =
  match msg with
  | SubmitRequest  -> m
  | NewValue (paramName,newValue) -> 
        let newParams = m.SpParams |> Seq.map (fun p -> 
            if p.SpParamHelper.SpParamName = paramName then
                { p with Value = newValue}
            else 
                p
        )
        { m with SpParams = newParams}


let validateValue (_,m:SpParamMvu.Model) =
  match StoredProcProcessor.ValidateSpParam m.SpParamHelper m.Value with
    | Error e -> 
        e.ErrorMessage |> Seq.toList
    | _ -> 
        List.empty

let paramBindings () : Binding<Model * SpParamMvu.Model, Msg> list = [
  "Value"
    |> Binding.twoWay((fun (_,m:SpParamMvu.Model) -> m.Value), (fun v (n,m:SpParamMvu.Model) -> NewValue (m.SpParamHelper.SpParamName, v) ) )
    |> Binding.addValidation(validateValue)
  "Label"
    |> Binding.oneWay (fun (_,m:SpParamMvu.Model) -> m.Label )
    ]

let bindings () : Binding<Model, Msg> list = [
  "CallParameters" 
   |> Binding.subModelSeq(
    (fun m -> m.SpParams),
    (fun e -> e.SpParamHelper.SpParamName),
    paramBindings
    //SpParamMvu.bindings
    //(fun () -> [
    //  "Name" |> Binding.oneWay (fun (_, e) -> e.Name)
    //  "SelectedLabel" |> Binding.oneWay (fun (m, e) -> if m.Selected = Some e.Id then " - SELECTED" else "")
    //])
    )


]

let main window (connStr: string) =

    let logger =
        LoggerConfiguration()
          .MinimumLevel.Override("Elmish.WPF.Update", Events.LogEventLevel.Verbose)
          .MinimumLevel.Override("Elmish.WPF.Bindings", Events.LogEventLevel.Verbose)
          .MinimumLevel.Override("Elmish.WPF.Performance", Events.LogEventLevel.Verbose)
          .WriteTo.Console()
          .CreateLogger()
    async {
      let conn = new SqlConnection(connStr);
      conn.Open()
      let! spParams = 
        (PuppyData.SqlMapper.StoredProcProcessor.GetParamHelpers conn "spEnrollStudent")
      let initialModel = init (spParams |> Seq.map (SpParamMvu.init))
      WpfProgram.mkSimple initialModel update bindings
      |> WpfProgram.withLogger (new SerilogLoggerFactory(logger))
      |> WpfProgram.startElmishLoop window
    } |> Async.StartImmediate
      