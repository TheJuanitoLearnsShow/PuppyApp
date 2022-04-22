namespace Puppy.SqlViewModels

open System.ComponentModel
open PuppyData.SqlMapper
open PuppyData.Types
open System.Collections.Generic
open System.Threading.Tasks
open System.Collections.ObjectModel

type LookupParameterViewModel(lkpInfo: LookupInfo, connStr: string) =
    let ev = new Event<_,_>()
    let evErr = new Event<_,_>()
    let mutable _errors = Seq.empty;

    let mutable _searchQuery = ""
    let mutable _label = ""
    let mutable _value = ""
    let execLookupSql = StoredProcProcessor.ExecuteLookupSql connStr lkpInfo
    
    let _searchResults = new ObservableCollection<_>()
    let UpdateSearchResult() = 
        task {
            let! rows = execLookupSql _searchQuery
            _searchResults.Clear()
            for r in rows do
                _searchResults.Add(r)
        }
    member x.IsValid = _errors |> Seq.isEmpty
    member val SearchResults = _searchResults with get, set
    

    member x.SearchQuery  
        with get () = _searchQuery
        and set (value) = 
            if (_searchQuery <> value) then
                _searchQuery <- value
                ev.Trigger(x, PropertyChangedEventArgs("SearchQuery"))
                UpdateSearchResult().Start()
                
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

