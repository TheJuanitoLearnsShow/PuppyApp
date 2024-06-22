namespace PuppyData.Types

open System.Collections.Generic
open System

type DataRows = IDictionary<string,obj> array


[<CLIMutable>]
type NetNature = {
    NetTypeName: string
    FromText: string->Result<obj, string>
    ToText: obj->string
}
    
type RangeType = 
    | LowerLimit of string * (string option)
    | UpperLimit of string * (string option)
    | BetweenLimit of string * string * (string option)

[<CLIMutable>]
type ValidValueRanges = {
    UdfName: string
    Ranges: RangeType seq
}

[<CLIMutable>]
type ValueLabelPair = {Value:string; Label:string}



[<CLIMutable>]
type LookupInfo = {ObjectForSearch:string; SearchParameterName:string; IdColumnName:string; LabelColumnName:string; IsStoredProc:bool}

type LookUpInfo =
    | LkupInfo of LookupInfo
    | PossibleValues of ValueLabelPair seq
    
[<CLIMutable>]
type PossibleValuesInfo = {
    UdfName: string
    PossibleValues: LookUpInfo
}

type NetNatureName = string

[<CLIMutable>]
type PuppySqlTypeInfo = {
    Required: bool
    Length: int
    Nature: NetNatureName
    Decimals: int
    Ranges: RangeType seq
    LookupInfo: LookUpInfo option
} with
    static member CreateText (isRequired: bool) (maxLength: int) = {
                Required = isRequired
                Length = maxLength
                Nature = typeof<string>.Name
                Decimals = 0
                Ranges = Seq.empty
                LookupInfo = None
            }
    static member CreateInt (isRequired: bool) (maxLength: int) = {
                Required = isRequired
                Length = maxLength
                Nature = typeof<int>.Name
                Decimals = 0
                Ranges = Seq.empty
                LookupInfo = None
            }
    static member CreateDecimal (isRequired: bool) (maxLength: int) (decimals: int) = {
                Required = isRequired
                Length = maxLength
                Nature = typeof<decimal>.Name
                Decimals = decimals
                Ranges = Seq.empty
                LookupInfo = None
            }

[<CLIMutable>]
type SpParamInfo = {
    ParameterName: string
    TypeName: string
    BaseSqlTypeName: string
    MaxLen: int
    Prec: int
    Scale: int
    ParamOrder: int
    IsNumericType: bool
}
with member x.SimpleName() = x.ParameterName.[1..]
        
[<CLIMutable>]
type ResultColumnInfo = {
    Name: string
    FriendlyName: string
    NetType: string
}

type Rows = IDictionary<string, Object> array

type ResultsMetadata = ResultColumnInfo array

type SqlExecResult = ResultsMetadata * Rows


[<CLIMutable>]
type ParamHelper =
    {
        SpParamName: string
        FriendlyName: string
        PuppyInfo: PuppySqlTypeInfo
    }
with member x.SimpleName() = x.SpParamName.[1..]


[<CLIMutable>]
type SpResultsMetada = {
    SpName: string
    Headings: string[]
    ActionTypes: string[]
    ActionURITemplates: string[]
    ActionDataTemplates: string[]
}