CREATE PROCEDURE [puppy].[spGetStoredProcedureMetadata] @spName varchar(100)
AS

select  p.name                                                                                            ParameterName,
        possibleValues.[Label],
        possibleValues.AllowedValue
from sys.parameters p
         inner join sys.types t
                   on p.user_type_id = t.user_type_id
         
         inner join puppy.ValuesAllowed possibleValues
                   on t.is_user_defined = 1
                       and t.name = possibleValues.UdfName
where object_id = object_id(@spName)
order by p.parameter_id, possibleValues.[Label];

select p.name                                                                                            ParameterName,
       type_name(p.system_type_id)                                                                       BaseSqlTypeName,
       case when t.is_user_defined = 1 and t.is_nullable = 1 then cast(1 as bit) else cast(0 as bit) end UdtIsNullable,
       case
           when type_name(p.system_type_id) = 'uniqueidentifier'
               then p.precision
           else OdbcPrec(p.system_type_id, p.max_length, p.precision) end                                Prec,
       coalesce(OdbcScale(p.system_type_id, p.scale), -1)                                                Scale,
       p.max_length                                                                                      MaxLen,

       l.MinLength                  AS                                                                   MinLenString,
       l.MaxLength                  AS                                                                   MaxLenString,
       l.CustomValidationMessage    AS                                                                   CustomValidationMessageLenRangeString,


       rdt.MinValue                 AS                                                                   MinValueDateTime,
       rdt.MinValueDaysOffset       AS                                                                   MinValueDaysOffsetDateTime,
       rdt.MaxValue                 AS                                                                   MaxValueDateTime,
       rdt.MaxValueDaysOffset       AS                                                                   MaxValueDaysOffsetDateTime,
       rdt.CustomValidationMessage  AS                                                                   CustomValidationMessageRangeDateTime,

       rdto.MinValue                AS                                                                   MinValueDateTimeOffset,
       rdto.MinValueDaysOffset      AS                                                                   MinValueDaysOffsetDateTimeOffset,
       rdto.MaxValue                AS                                                                   MaxValueDateTimeOffset,
       rdto.MaxValueDaysOffset      AS                                                                   MaxValueDaysOffsetDateTimeOffset,
       rdto.CustomValidationMessage AS                                                                   CustomValidationMessageRangeDateTimeOffset,

       rdec.MinValue                AS                                                                   MinValueDecimal,
       rdec.MaxValue                AS                                                                   MaxValueDecimal,
       rdec.CustomValidationMessage AS                                                                   CustomValidationMessageRangeDecimal,

       rint.MinValue                                                                                     MinValueInt,
       rint.MaxValue                                                                                     MaxValueInt,
       rint.CustomValidationMessage                                                                      CustomValidationMessageRangeInt,

       rlng.MinValue                AS                                                                   MinValueLong,
       rlng.MaxValue                AS                                                                   MaxValueLong,
       rlng.CustomValidationMessage AS                                                                   CustomValidationMessageRangeLong,

       rmny.MinValue                AS                                                                   MinValueMoney,
       rmny.MaxValue                AS                                                                   MaxValueMoney,
       rmny.CustomValidationMessage AS                                                                   CustomValidationMessageRangeMoney

from sys.parameters p
         left join sys.types t
                   on p.user_type_id = t.user_type_id
         left join puppy.ValueLengthRangesString l
                   on t.is_user_defined = 1
                       and t.name = l.UdfName
         left join puppy.ValueRangesDateTime rdt
                   on t.is_user_defined = 1
                       and t.name = rdt.UdfName
         left join puppy.ValueRangesDateTimeOffset rdto
                   on t.is_user_defined = 1
                       and t.name = rdto.UdfName
         left join puppy.ValueRangesDecimal rdec
                   on t.is_user_defined = 1
                       and t.name = rdec.UdfName
         left join puppy.ValueRangesInt rint
                   on t.is_user_defined = 1
                       and t.name = rint.UdfName
         left join puppy.ValueRangesLong rlng
                   on t.is_user_defined = 1
                       and t.name = rlng.UdfName
         left join puppy.ValueRangesMoney rmny
                   on t.is_user_defined = 1
                       and t.name = rmny.UdfName
where object_id = object_id(@spName)
order by p.parameter_id;
