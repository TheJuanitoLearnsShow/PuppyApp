
declare @spName as varchar(255)= 'spEnrollStudent'

select  
    name ParameterName,  
    type_name(user_type_id) TypeName,  
    type_name(system_type_id) BaseSqlTypeName,
    max_length MaxLen ,  
    case when type_name(system_type_id) = 'uniqueidentifier' 
                then precision  
                else OdbcPrec(system_type_id, max_length, precision) end Prec,  
    OdbcScale(system_type_id, scale) Scale, 
	case when OdbcScale(system_type_id, scale) is not null then cast(1 as bit)  else cast(0 as bit) end IsNumericType, 
    parameter_id ParamOrder,  
    convert(sysname, 
                    case when system_type_id in (35, 99, 167, 175, 231, 239)  
                    then ServerProperty('collation') end) Collation  

    from sys.parameters where object_id = object_id(@spName)
	for json auto