module SpParamMvu


open System
open System.Linq
open Elmish.WPF
open PuppyData.Types
open PuppyData.SqlMapper

type Model =
  { 
    SpParamHelper: ParamHelper
    Value: string 
    Label: string
    }

let init (spParamHelper: ParamHelper) =
  { SpParamHelper = spParamHelper
    Value = ""
    Label = spParamHelper.FriendlyName }

type Msg =
  | NewValue of string

let update msg m =
  match msg with
  | NewValue x -> { m with Value = x }


