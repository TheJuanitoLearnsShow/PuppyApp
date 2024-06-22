namespace Puppy.SqlViewModels

open System.ComponentModel
open PuppyData.SqlMapper
open PuppyData.Types
open System.Collections.Generic
open FSharp.Control.Reactive
open System.Reactive
open System

type SpParameterViewModel(spParamHelper: ParamHelper, initialValue: string, connStr: string) as this=
    let ev = new Event<_,_>()
    let evErr = new Event<_,_>()
    let mutable _errors = Seq.empty;
    let puppyInfo = spParamHelper.PuppyInfo
    let _lookup = 
        match puppyInfo.LookupInfo with
        | Some (LkupInfo l) ->
            let vmLkp = LookupParameterViewModel(l, connStr, fun (s) -> this.Value <- s)
            vmLkp |> Some
        | _ ->
            None
    let mutable _isRequired = puppyInfo.Required
    let mutable _label = spParamHelper.FriendlyName
    let mutable _value = initialValue


    let validateValue =
        StoredProcProcessor.ValidateSpParam spParamHelper 
        
    member x.IsValid = _errors |> Seq.isEmpty
    member val NetNature = puppyInfo.Nature
    member val SpParamName = spParamHelper.SpParamName

    member x.HasLookup 
        with get() =
            _lookup |> Option.isSome

    member x.Lookup 
        with get() =
            _lookup.Value

    member x.IsRequired  
        with get () = _isRequired
        and set (value) = 
            if (_isRequired <> value) then
                _isRequired <- value
                ev.Trigger(x, PropertyChangedEventArgs("IsRequired"))
                
    member x.Label  
        with get () = _label
        and set (value) = 
            if (_label <> value) then
                _label <- value
                ev.Trigger(x, PropertyChangedEventArgs("Label"))
                
    member x.Value 
        with get () : string  = _value
        and set (value) = 
            if (_value <> value) then
                _value <- value
                match validateValue _value with
                | Error e -> 
                    _errors <- e.ErrorMessage
                | _ -> 
                    _errors <- Seq.empty
                ev.Trigger(x, PropertyChangedEventArgs("Value"))
                evErr.Trigger(x, DataErrorsChangedEventArgs("Value"))
                match _lookup with
                | Some l ->
                    l.ShowResults <- false
                | _ ->
                    ()


    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member x.PropertyChanged = ev.Publish

    interface INotifyDataErrorInfo with
        [<CLIEvent>]
        member this.ErrorsChanged: IEvent<System.EventHandler<DataErrorsChangedEventArgs>,DataErrorsChangedEventArgs> = 
            evErr.Publish
        member this.HasErrors: bool = 
            _errors |> Seq.isEmpty |> not
        member x.GetErrors(propertyName:string) =
            _errors

