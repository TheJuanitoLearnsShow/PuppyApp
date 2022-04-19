namespace PuppyData.Types

open System.Runtime.CompilerServices
open System.Collections.Generic
open System

[<Extension>]
type DictHelpers () =
    [<Extension>]
    static member inline GetString(xs: IDictionary<string, Object>, key: string) = 
        match xs.TryGetValue key with
        | true, v -> v.ToString()
        | false, _ -> ""
    [<Extension>]
    static member inline GetInt(xs: IDictionary<string, Object>, key: string) = 
        match xs.TryGetValue key with
        | true, v -> 
            Convert.ToInt32 v
        | false, _ -> 0
        
module ErrorHelpers = 
    let FromStr (errMsg:string)=
        {
            ErrorProperty = ""
            ErrorMessage = [| errMsg |]
        }

module Helpers =

    let EmptySpResult = { 
                Results =  Array.empty
                MetaData = Array.empty
                Errors =  Array.empty
            }
    let StandardErrorPropName = "ErrorProperty"
    let StandardErrorPropText = "ErrorMessage"
    let StandardErrMeta = [|
        { 
            Name = StandardErrorPropName
            FriendlyName = "Error"
            NetType = "string"
        }    
        { 
            Name = StandardErrorPropText
            FriendlyName = "Error"
            NetType = "string"
        }
    |]
    let ToErrorRendition  (data:Rows)=
        ( data |> Seq.map (fun (dict) -> 
            {
                ErrorProperty = dict.[StandardErrorPropName].ToString()
                ErrorMessage = [| dict.[StandardErrorPropText].ToString() |]
            }
        ))
    let SpResultWithError (err: string) = 
        let metaData = StandardErrMeta
        { 
                Results =  Array.empty
                MetaData = StandardErrMeta
                Errors =  [| {
                    ErrorProperty = ""
                    ErrorMessage = [| err |]
                } |]
        }
    //let EmptySpResultRendition :SpResultRendition = { 
    //            Data = EmptySpResult
    //            Errors =  Array.empty
    //        }
    let GetFirstResult (spResult: SpResult) =
        if spResult.Results.Length >= 1 then 
            spResult.Results.[0]
        else 
            Array.empty
    let SpResultFromDict (row: IDictionary<string,obj>)=
        { 
            Results =  [| [| row |] |]
            MetaData = Array.empty
            Errors =  Array.empty
        }

