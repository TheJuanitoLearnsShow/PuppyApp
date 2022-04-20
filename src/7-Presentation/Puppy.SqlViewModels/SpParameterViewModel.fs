namespace Puppy.SqlViewModels

open System.ComponentModel
open PuppyData.SqlMapper
open PuppyData.Types
open System.Collections.Generic

type SpParameterViewModel(spParamHelper: ParamHelper, initialValue: string) =
    let ev = new Event<_,_>()
    let evErr = new Event<_,_>()
    let mutable _errors = Seq.empty;
    let puppyInfo = spParamHelper.PuppyInfo
    let mutable _isRequired = puppyInfo.Required
    let mutable _label = spParamHelper.FriendlyName
    let mutable _value = initialValue

    let validateValue =
        StoredProcProcessor.ValidateSpParam spParamHelper 

    member val NetNature = puppyInfo.Nature

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
        with get () = _value
        and set (value) = 
            if (_value <> value) then
                _value <- value
                ev.Trigger(x, PropertyChangedEventArgs("Value"))
                match validateValue _value with
                | Error e -> 
                    _errors <- e.ErrorMessage
                | _ -> 
                    _errors <- Seq.empty
                evErr.Trigger(x, DataErrorsChangedEventArgs("Value"))

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

