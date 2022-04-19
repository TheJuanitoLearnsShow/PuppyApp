namespace FsCommon
   
open System
open System.Text.RegularExpressions


module ConversionHelpers =

    let tryParseWith tryParseFunc = tryParseFunc >> function
        | true, v    -> Some v
        | false, _   -> None

    let tryParseInt (n:string) = 
        match Int32.TryParse n with
        | true, v    -> Some v
        | false, _   -> None

    let tryParseDecimal (n:string) = 
        match Decimal.TryParse n with
        | true, v    -> Some v
        | false, _   -> None

module Validations =
    open System.Collections.Generic

    let inline ok<'TSuccess,'TMessage> (x:'TSuccess) : Result<'TSuccess,'TMessage> = Ok(x)

    /// Wraps a value in a Success
    let inline pass<'TSuccess,'TMessage> (x:'TSuccess) : Result<'TSuccess,'TMessage> = Ok(x)


    /// Wraps a message in a Failure
    let inline fail<'TSuccess,'Message> (msg:'Message) : Result<'TSuccess,'Message> = Error(msg )
    let inline apply wrappedFunction result = 
        match wrappedFunction, result with
        | Ok(f), Ok(x) -> ok(f x)
        | Error errs, Ok(_, _msgs) -> Error(errs)
        | Ok(_), Error errs -> Error(errs)
        | Error errs1, Error errs2 -> Error(errs1 @ errs2)

    let inline lift f result = 
        match result with
        | Ok success -> Ok (f success)
        | Error err -> Error err
    let inline bind f result = 
        match result with
        | Ok success -> f success
        | Error err -> Error err
    
    let inline testForFail f result = 
        match result with
        | Ok success -> 
            match f success with
            | Ok success -> result
            | Error err -> Error err
        | Error err -> Error err
        

    let inline (>>=) result f = bind f result
    let inline (>=>) result f  = lift f result
    let inline (>>>) ropF f  = ropF >> (fun res -> res >=> f)
    let inline (>>>=) ropF f  = ropF >> (fun res -> res >>= f)

    let inline GetFirstError<'T, 'E> (results:Result<'T, 'E> seq) =
        results
        |> Seq.filter 
            (fun r -> 
                match r with 
                | Error e -> true 
                | _ -> false
            )
        |> Seq.tryHead

    let inline ChooseErrors<'T, 'E> (results:Result<'T, 'E> seq) =
        results
        |> Seq.choose 
            (fun r -> 
                match r with 
                | Error e -> Some e 
                | _ -> None
            )
    let inline ChooseSucesses<'T, 'E> (results:Result<'T, 'E> seq) =
        results
        |> Seq.choose 
            (fun r -> 
                match r with 
                | Ok e -> Some e 
                | _ -> None
            )
    let inline CollectErrors<'T, 'E> (results:Result<'T, 'E seq> seq) =
        results
        |> Seq.choose 
            (fun r -> 
                match r with 
                | Error e -> Some e 
                | _ -> None
            )
        |> Seq.collect id
    let inline CollectSucesses<'T, 'E> (results:Result<'T, 'E seq> seq) =
        results
        |> Seq.choose 
            (fun r -> 
                match r with 
                | Ok e -> Some e 
                | _ -> None
            )
    
    let inline collectionMapPass (l:'A seq) f result = 
        match result with
        | Ok success -> 
            let fstRes =
                l
                |> Seq.map (f success)
                |> Seq.tryHead
            match fstRes with 
            | (Some (Error err))  -> Error err
            | _  -> Ok success
        | Error err -> Error err
    let inline TupleSeqToDict (tuples:(string*'T) seq) =
        let newDixct =new Dictionary<string, 'T>()
        for (k, e) in tuples do
            newDixct.Add(k, e)
        newDixct
    let inline CollectSucessesDict<'T, 'E> (results:(string * Result<'T, 'E seq>) seq) =
        results
        |> Seq.choose 
            (fun (k,r) -> 
                match r with 
                | Ok e -> Some (k, e) 
                | _ -> None
            )
        |> TupleSeqToDict
        
    let inline isDate (txt:string) =
        let couldParse, parsedDate = System.DateTime.TryParse(txt)
        let inRage = couldParse && (parsedDate.Year > 1900 && parsedDate.Year <= 9999)
        inRage
        
    let inline isDecimal (txt:string) =
        let couldParse, parsedValue = System.Decimal.TryParse(txt)
        couldParse

    let inline isInvalidDate txt =
        not (isDate txt)

    let inline isInvalidDecimal txt =
        not (isDecimal txt)

    let inline isBlank txt =
        System.String.IsNullOrWhiteSpace(txt);

    let inline isFilled isRequired txt =
        match isRequired , System.String.IsNullOrWhiteSpace(txt) with
        | true, true -> fail ("Must not be empty")
        | _ -> ok txt

    let inline isIn possibleChoices txt =
        possibleChoices |> Seq.contains txt

    let inline isNotIn possibleChoices txt =
        not (isIn possibleChoices txt)

    let inline isCorrectLenght minSize maxSize (txt:string) =
        match txt with
        | null -> fail ("Must not be null")
        | l when l.Length < minSize -> fail ("Must be more than " + minSize.ToString() + " character(s)")
        | l when l.Length > maxSize -> fail ("Must be less than " + maxSize.ToString() + " character(s)")
        | _ -> ok txt

    //let inline isCorrectDigitLength maxDigits maxDecimals (txt:string) =
    //    let maxNaturals = (maxDigits - maxDecimals) 
    //    let checkNatural (s: string) =
    //        if s.Length > maxNaturals then 
    //            fail ("Must have at most " + maxNaturals.ToString() + " digits left to the decimal point")
    //    match Decimal.TryParse txt with
    //    | true, d ->
    //        match d.ToString("0.00").Split with
    //        | [| naturals, decimals |]  -> 
                

    //    | null -> fail ("Must not be null")
    //    | l when l.Length < minSize -> fail ("Must be more than " + minSize.ToString() + " character(s)")
    //    | l when l.Length > maxSize -> fail ("Must be less than " + maxSize.ToString() + " character(s)")
    //    | _ -> ok txt

    let inline isCorrectByteLenght minSize maxSize (data:'A array) =
        match data with
        | null -> fail ("Must not be empty")
        | l when l.Length < minSize -> fail ("Must be more than " + minSize.ToString() + " byte(s)")
        | l when l.Length > maxSize -> fail ("Must be less than " + maxSize.ToString() + " byte(s)")
        | _ -> ok data

    let inline isCorrectPattern (pattern:Regex) (txt:string) =
        if pattern = null then 
            ok txt
        else
            match txt with
            | null -> ok txt
            | l when not(pattern.IsMatch(l)) -> fail ("Does not match pattern of " + (pattern.ToString()) )
            | _ -> ok txt

    let inline isWithinRange minVal maxVal value =
        match value with
        | v when v > maxVal -> fail ("Must not be more than " + maxVal.ToString())
        | v when v < minVal -> fail ("Must not be less than " + minVal.ToString())
        | _ -> ok value
    let inline ToDecimal (txt:string) =
        match System.Decimal.TryParse(txt) with
        | true, v -> pass v
        | false, _ -> fail ("Must be a valid number")
    let inline ToInt (txt:string) =
        match System.Int64.TryParse(txt) with
        | true, v -> pass v
        | false, _ -> fail ("Must be a valid integer")
    let inline ToDateTime (txt:string) =
        match System.DateTime.TryParse(txt) with
        | true, v -> pass v
        | false, _ -> fail ("Must be a valid date")
    let inline MustBeDecimal (txt:string) =
        match System.Decimal.TryParse(txt) with
        | true, v -> pass txt
        | false, _ -> fail ("Must be a valid number")
    let inline MustBeInt (txt:string) =
        match System.Int32.TryParse(txt) with
        | true, v -> pass txt
        | false, _ -> fail ("Must be a valid integer")
    let inline HasCorrectTotalDecimalDigits maxNumDigits (txt:string) =
        match txt.Split('.') with
        | [| _ |] -> pass txt //no period
        | [|_;decimalDigits |] when decimalDigits.Length <= maxNumDigits -> 
            pass txt
        | _ -> fail ("Must have at most " + maxNumDigits.ToString() + " decimal digits")
        
    let inline RemoveChar (charToRemove:string) (txt:string) =
        txt.Replace(charToRemove, "")

    let inline LPad (len:int) (charToPad:char) (txt:string) =
        txt.PadLeft(len, charToPad)
    

    //let RequiredDecimal length decimals (txt:string) = 
    //        txt
    //        |> Create ({
    //                    Required = true
    //                    Length = length
    //                    Def = FixedLenDef.ZeroSuppressed
    //                    Decimals = decimals
    //                    ValidOptions = Array.empty
    //                })
    //        >>= ToDecimal
