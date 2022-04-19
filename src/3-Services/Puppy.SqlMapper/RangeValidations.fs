namespace FsCommon

open Puppy.SqlMapper

module RangeValidations = 
    open System
    open FsCommon.Validations
    open PuppyData.Types
    
    let private isMoreThanOrEqual msg  (value: Object) (f:Object): Result<Object, string> =
        match value with
        | :? IComparable as comp ->
            match f with 
            | :? IComparable as vl  when comp < vl ->
                fail (defaultArg msg ("Must be greater than or equal to " + f.ToString()) )
            | :? IComparable   -> pass f
            | _ -> fail ( (f.GetType().FullName ) + " must be comparable"  )
        | _ -> fail ( (value.GetType().FullName ) + " must be comparable"  )
    let private isLessThanOrEqual msg  (value: Object) (f:Object): Result<Object, string> = 
        match value with
        | :? IComparable as comp ->
            match f with 
            | :? IComparable as vl  when comp > vl ->
                fail (defaultArg msg ("Must be less than or equal to " + f.ToString()) )
            | :? IComparable   -> pass value
            | _ -> fail ( (f.GetType().FullName ) + " must be comparable"  )
        | _ -> fail ( (value.GetType().FullName ) + " must be comparable"  )
    let private isBetween msg  (value: Object) (f1:Result<Object, string>) (f2:Result<Object, string>) : Result<Object, string> =
        match value with
        | :? IComparable as comp ->
            match (f1, f2) with 
            | (Ok f1v, Ok f2v) ->
                match (f1v, f2v) with 
                | ((:? IComparable as vl1), (:? IComparable as vl2))  when comp > vl2 || comp < vl1 ->
                    fail (defaultArg msg ("Must be between " + vl1.ToString() + " and " + vl2.ToString() )  )
                | (:? IComparable as vl1, :? IComparable as vl2)   -> pass value
                | _ -> fail ( (f1v.GetType().FullName ) + " and " + (f2v.GetType().FullName ) + " must be comparable"  )
            | (Error err1, Error err2) -> fail ( err1 + ". " +  err2 )
            | (Error err1, _) -> fail ( err1 )
            | (_, Error err2) -> fail ( err2  )
        | _ -> fail ( (value.GetType().FullName ) + " must be comparable"  )
    let isInRange (nature:NetNature)   (value: Object) (range: RangeType) : Result<Object, string> =
        match range with 
        | LowerLimit (fromVal,msg) -> 
            fromVal
            |> nature.FromText 
            >>= isMoreThanOrEqual msg value
        | UpperLimit (toVal,msg) -> 
            toVal
            |> nature.FromText 
            >>= isLessThanOrEqual msg value
        | BetweenLimit (fromVal,toVal, msg) -> 
            let fromValR = nature.FromText fromVal
            let toValR = nature.FromText toVal
            isBetween msg value fromValR toValR

    let isInRangeT (nature:NetNature)   (value: 'T) (range: RangeType) : Result<'T, string> =
        match range with 
        | LowerLimit (fromVal,msg) -> 
            fromVal
            |> nature.FromText 
            >>= (fun f -> 
                let nf = f :?> 'T
                if value < nf then
                    fail (defaultArg msg ("Must be greater than or equal to " + f.ToString()) )
                else 
                    pass value
                )
        | UpperLimit (toVal,msg) -> 
            toVal
            |> nature.FromText 
            >>= (fun f -> 
                let nf = f :?> 'T
                if value > nf then
                    fail (defaultArg msg ("Must be less than or equal to " + f.ToString()) )
                else 
                    pass value
                )
        | BetweenLimit (fromVal,toVal, msg) -> 
            let fromValR = nature.FromText fromVal
            let toValR = nature.FromText toVal
            match (fromValR, toValR) with 
            | (Ok f1v, Ok f2v) ->
                let nf1 = f1v :?> 'T
                let nf2 = f2v :?> 'T
                if value < nf1 || value > nf2 then
                    fail (defaultArg msg ("Must be between " + nf1.ToString() + " and " + nf2.ToString() )  )
                else 
                    pass value
            | (Error err1, Error err2) -> fail ( err1 + ". " +  err2 )
            | (Error err1, _) -> fail ( err1 )
            | (_, Error err2) -> fail ( err2  )

    //let ValidateRange (typeInfo: PuppySqlTypeInfo) (value: Object) =
    //    match value with 
    //    | :? IComparable as comp ->
    //        isInRange typeInfo.Nature range comp

    let canItBeConvertedFromtext typeInfo = 
        let nature = SqlTypeTranslator.GetNature typeInfo.Nature
        testForFail nature.FromText //(r:Result<string, string>) = 
    
        //match typeInfo.Nature with 
        //| Text ->
        //    value
        //    |> isFilled typeInfo.Required
        //    >>=  isCorrectLenght 0 typeInfo.Length
        //| Decimal ->
        //    value
        //    |> isFilled typeInfo.Required
        //    >>= MustBeDecimal
        //    >>= HasCorrectTotalDecimalDigits typeInfo.Decimals
        //    >=> RemoveChar "."
        //    >>= isCorrectLenght 0 typeInfo.Length
        //    >>= ToDecimal
        //    >=> box
        //| Int ->
        //    value
        //    |> isFilled typeInfo.Required
        //    >>= MustBeDecimal
        //    >>= HasCorrectTotalDecimalDigits typeInfo.Decimals
        //    >=> RemoveChar "."
        //    >>= isCorrectLenght 0 typeInfo.Length
        // TODO add validations for ranges and for patterns. To keep it simple, leave the result as Result<Object, string>