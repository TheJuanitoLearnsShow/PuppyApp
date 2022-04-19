namespace PuppyData.Types

[<CLIMutable>]
type ErrorRendition = {
    ErrorProperty: string
    ErrorMessage: string seq
}

[<CLIMutable>]
type SpResult = {
    Results: DataRows array
    MetaData: ResultColumnInfo array
    Errors: ErrorRendition seq
}
