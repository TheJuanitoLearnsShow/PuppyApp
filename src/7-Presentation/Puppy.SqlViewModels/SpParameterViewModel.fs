namespace Puppy.SqlViewModels

open System.ComponentModel
open PuppyData.SqlMapper
open PuppyData.Types
open System.Collections.Generic

type SpParameterViewModel(spParamHelper: ParamHelper, initialValue: string) =
    let ev = new Event<_,_>()
    let _errorsByPropertyName = new Dictionary<string, List<string>>();
    let puppyInfo = spParamHelper.PuppyInfo
    let mutable _isRequired = puppyInfo.Required
    let mutable _label = spParamHelper.FriendlyName
    let mutable _value = initialValue
    

    let validateValue =
        StoredProcProcessor.ValidateSpParam spParamHelper 

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
                _errorsByPropertyName.Clear
                match validateValue _value with
                | Error e -> 
                    _errorsByPropertyName.[]
                | _ -> None

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member x.PropertyChanged = ev.Publish

    interface INotifyDataErrorInfo with
        [<CLIEvent>]
        member this.ErrorsChanged: IEvent<System.EventHandler<DataErrorsChangedEventArgs>,DataErrorsChangedEventArgs> = 
            raise (System.NotImplementedException())
        member this.HasErrors: bool = 
            _errorsByPropertyName.Count > 0

        member x.GetErrors(propertyName:string) =
            //get it from the sql module
            Seq.empty

