namespace Puppy.SqlMapper

module SqlTypeTranslator = 
    open System.Collections.Generic
    open System.Data
    open PuppyData.Types
    open System.Data.SqlClient
    open FsCommon
    open FsCommon.Validations
    open System
    open PuppyData.Types.NetNatureHelper

    type SqlTypeTranslator = SpParamInfo -> PuppySqlTypeInfo

    let private typeBuilders : IDictionary< string, SqlDbType * NetNature> = 
        dict [
            // exact numerics
            "bigint", (SqlDbType.BigInt, buildInt)
            //"bit", (SqlDbType.Bit, "System.Boolean") 
            "decimal", (SqlDbType.Decimal, buildDecimal) 
            "int", (SqlDbType.Int, buildInt)
            "money", (SqlDbType.Money, buildDecimal) 
            "numeric", (SqlDbType.Decimal, buildDecimal) 
            "smallint", (SqlDbType.SmallInt, buildInt)
            "smallmoney", (SqlDbType.SmallMoney, buildDecimal) 
            "tinyint", (SqlDbType.TinyInt, buildInt)

            // approximate numerics
            //"float", (SqlDbType.Float, "System.Double") // This is correct. SQL Server 'float' type maps to double
            //"real", (SqlDbType.Real, "System.Single")

            // date and time
            "date", (SqlDbType.Date, buildDate)
            "datetime", (SqlDbType.DateTime, buildDate)
            "datetime2", (SqlDbType.DateTime2, buildDate)
            //"datetimeoffset", (SqlDbType.DateTimeOffset, "System.DateTimeOffset")
            "smalldatetime", (SqlDbType.SmallDateTime,  buildDate)
            //"time", (SqlDbType.Time, "System.TimeSpan")

            // character strings
            "char", (SqlDbType.Char, buildText)
            "text", (SqlDbType.Text, buildText)
            "varchar", (SqlDbType.VarChar, buildText)

            // unicode character strings
            "nchar", (SqlDbType.NChar, buildText)
            "ntext", (SqlDbType.NText, buildText)
            "nvarchar", (SqlDbType.NVarChar, buildText)
            "sysname", (SqlDbType.NVarChar, buildText)

            // binary
            //"binary", (SqlDbType.Binary, "System.Byte[]")
            //"image", (SqlDbType.Image, "System.Byte[]")
            //"varbinary", (SqlDbType.VarBinary, "System.Byte[]")


            //"sql_variant", (SqlDbType.Variant, "System.Object")

            //"timestamp", (SqlDbType.Timestamp, "System.Byte[]")  // note: rowversion is a synonym but SQL Server stores the data type as 'timestamp'
            //"uniqueidentifier", (SqlDbType.UniqueIdentifier, "System.Guid")
            "xml", (SqlDbType.Xml, buildText)

            //Custom types??
            "shortname", (SqlDbType.VarChar, buildText)
            
        ]
        
    let ToNature (sqlTypeName:string) =
        typeBuilders.[sqlTypeName.ToLower()] |> snd |> (fun n -> n.NetTypeName)

    // TODO: add this (the .net nature dict) to shared logic?
    let GetNature (netTypeName:string) =
        typeBuildersNetTypes.[netTypeName] 