namespace PuppyData.Types

module NetNatureHelper =


    open System.Collections.Generic
    open FsCommon
    open FsCommon.Validations
    open System

    let private commonToStr o = o.ToString()
    let buildInt = {
        NetTypeName = typeof<int>.Name
        FromText = Validations.ToInt >>> box
        ToText = commonToStr
    }
    let buildDecimal = {
        NetTypeName = typeof<decimal>.Name
        FromText = Validations.ToDecimal >>> box
        ToText = commonToStr
    }
    let buildDate = {
        NetTypeName = typeof<DateTime>.Name
        FromText = Validations.ToDateTime >>> box
        ToText = commonToStr
    }
    let buildText = {
        NetTypeName = typeof<string>.Name
        FromText = (fun s -> pass s) >>> box
        ToText = commonToStr
    }
    let typeBuildersNetTypes : IDictionary< string, NetNature> = 
         dict [
            typeof<int>.Name, buildInt
            typeof<decimal>.Name, buildDecimal
            typeof<DateTime>.Name, buildDate
            typeof<string>.Name, buildText
         ]
    let GetNature (netTypeName:string) =
        typeBuildersNetTypes.[netTypeName] 

