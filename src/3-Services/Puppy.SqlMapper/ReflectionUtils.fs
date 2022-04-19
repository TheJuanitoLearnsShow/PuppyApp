namespace Puppy.SqlMapper

open System
open System.Reflection
open System.Collections.Generic
open FSharp.Core
open Microsoft.FSharp.Reflection
open FSharpReflectionExtensions
open System.Linq

module ReflectionUtils =
    let IsOptionType (obj: Object) = 
        let optType = obj.GetType()
        optType.Name.StartsWith("Microsoft.FSharp.Core.FSharpOption")
    let ToOptionObj2 (optType: System.Type) (obj: Object) : Object = 
        if obj = null then
            None |> box
        else 
            //TODO must use cache for someInfo
            let cases = FSharpType.GetUnionCases(optType)
            printfn "%A" cases
            let someInfo = cases |> Seq.find (fun c -> c.Name = "Some")
            FSharpValue.MakeUnion(someInfo,[| obj |])

            
    let MapOptionIfAny (obj: Object) : Object = 
        if isNull obj then
            null
        else
            let optType = obj.GetType()
            if  optType |> FSharpType.IsUnion then
                //let cases = FSharpType.GetUnionCases(optType)
                let (someInfo,v) : UnionCaseInfo * (obj []) = FSharpValue.GetUnionFields(obj, optType)
                let nullableV = 
                    match someInfo.Name with
                    | "Some" -> v.[0] |> box
                    | _ -> null
                nullableV
            else
                obj
    
    let private CachedConstructorParams = new Dictionary<Type, ParameterInfo array>()
    let GetConstructorParams (recordType:Type) = 
        match CachedConstructorParams.TryGetValue recordType with
        | true, p -> p
        | false, _ ->
            recordType.GetConstructors().First().GetParameters()
    let OrderedValuesFromDict (constructorParams: ParameterInfo seq) (inputDict:IDictionary<string,obj>) : Object[] = 
        constructorParams
        |> Seq.map 
            (fun p ->
                let propName = p.Name 
                let key = inputDict.Keys |> Seq.tryFind (fun s -> s.ToLower() = propName.ToLower())
                match key with
                | None -> failwith (propName + " was not found as a key in the input dictionary")
                | Some k ->
                    let dictV =inputDict.[k]
                    if p.ParameterType |> FSharpType.IsUnion 
                        && ( (dictV |> isNull) || (dictV.GetType()  |> FSharpType.IsUnion |> not) ) then 
                         dictV |> ToOptionObj2 p.ParameterType
                    else 
                        dictV
            )
        |> Seq.toArray
        
    let ObjectFromFromDict (recordType:Type) (inputDict:IDictionary<string,obj>) =
        let orderedValuesFromDict = OrderedValuesFromDict (GetConstructorParams recordType) inputDict
        FSharpValue.MakeRecord(recordType , orderedValuesFromDict)

    let TypedObjectFromFromDict<'T>  (inputDict:IDictionary<string,obj>) =
        let recordType= typeof<'T>
        let orderedValuesFromDict = OrderedValuesFromDict (GetConstructorParams recordType) inputDict
        FSharpValue.MakeRecord(recordType , orderedValuesFromDict) :?> 'T