namespace Puppy.SqlViewModels

open System.ComponentModel
open PuppyData.SqlMapper
open PuppyData.Types
open System.Collections.Generic
open System.Threading.Tasks
open System.Collections.ObjectModel
open FSharp.Control.Reactive
open System.Reactive.Concurrency
open System
open System.Threading
open System.Reflection.Emit
open System.Windows.Input

type SearchCommand () = 
    let canExecuteChanged = new Event<_>()
    // interface ICommand with
    //     member this.CanExecute (obj) = true
    //     // member this.Execute (obj) = reload_tests()
    //     [<CLIEvent>]
    //     member this.CanExecuteChanged = true //canExecuteChanged.Publish


type LookupParameterViewModel(lkpInfo: LookupInfo, connStr: string, valueSelectedFunc: string->unit) as this =
    let ev = new Event<_,_>()
    let evErr = new Event<_,_>()
    let mutable _errors = Seq.empty;

    let mutable _searchQuery = ""
    let mutable _label = ""
    let mutable _value = { Value =""; Label =""}
    let mutable _showResults = false
    let execLookupSql = StoredProcProcessor.ExecuteLookupSql connStr lkpInfo
    
    let _searchResults = new ObservableCollection<_>()

    let updateSearchResult(searchQuery) = 
        task {
            let! rows = execLookupSql searchQuery
            return rows
        }
    
    let results = ev.Publish  |> Observable.throttle (TimeSpan.FromSeconds(0.5)) 
                            |> Observable.filter (fun (evt:PropertyChangedEventArgs) -> evt.PropertyName = "SearchQuery")
                            |> Observable.flatmapTask (fun (evt:PropertyChangedEventArgs) -> updateSearchResult(_searchQuery))
                            |> Observable.observeOnContext(SynchronizationContext.Current)

    do
        results |> Observable.subscribe( fun rows -> this.UpdateSearchResult(rows) ) |> ignore

    member x.UpdateSearchResult(rows) = 
        _searchResults.Clear()
        for r in rows do
            _searchResults.Add(r)
        x.ShowResults <- true

    member x.IsValid = _errors |> Seq.isEmpty
    member val SearchResults = _searchResults with get, set
    

    member x.ShowResults  
        with get () = _showResults
        and set (value) = 
            if (_showResults <> value) then
                _showResults <- value
                ev.Trigger(x, PropertyChangedEventArgs("ShowResults"))

    member x.SearchQuery  
        with get () = _searchQuery
        and set (value) = 
            if (_searchQuery <> value) then
                _searchQuery <- value
                ev.Trigger(x, PropertyChangedEventArgs("SearchQuery"))
                
                
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
                valueSelectedFunc(_value.Value)
                

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

