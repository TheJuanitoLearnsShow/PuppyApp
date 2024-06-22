namespace Puppy.SqlViewModels

open System.Collections.Generic
open PuppyData.SqlMapper
open PuppyData.Types

type DraftValue = {
    fieldDefinition: ParamHelper
    CurrValue: string
    IsValid: bool
    Errors: string seq
}
type RequestFormViewModel(fieldDefinitions: ParamHelper[], initialValues: IDictionary<string,string>) as x=
    
    let validateValue spParamHelper =
        StoredProcProcessor.ValidateSpParam spParamHelper 
    let setValue (field:DraftValue) (newValue:string) =
        match validateValue field.fieldDefinition newValue with
        | Error e -> 
            { field with CurrValue = newValue; Errors = e.ErrorMessage } 
        | _ -> 
            { field with CurrValue = newValue; Errors = Seq.empty } 
    
    let currDraftValues = new Dictionary<string, DraftValue>(fieldDefinitions.Length)
    let loadInitialValues () =
        fieldDefinitions
                |> Seq.map (fun f -> {
                    fieldDefinition = f
                    CurrValue = "" 
                    IsValid = true
                    Errors = Seq.empty
                })
                |> Seq.map (fun d ->
                    let pName = d.fieldDefinition.SpParamName
                    let initialVal = if initialValues.ContainsKey pName then initialValues[pName] else ""
                    setValue d initialVal
                    )
                |>  Seq.iter (fun kvp -> currDraftValues.[kvp.fieldDefinition.SpParamName] <- kvp)
    
    member val DraftFields = currDraftValues.Values
    
    member x.SetFieldValue (fieldName:string) (newValue:string) =
        let d = currDraftValues.[fieldName]
        currDraftValues.[fieldName] <- setValue d newValue
        