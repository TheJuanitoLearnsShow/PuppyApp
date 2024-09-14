CREATE PROCEDURE puppy.spSetValuesForTypes
AS
-- TODO: is there a way not use magic string for the udf type names?
	delete [puppy].ValueLengthRangesString;
	insert into [puppy].ValueLengthRangesString(
	UdfName, 
	[MinLength],
	[MaxLength],
	[CustomValidationMessage])
	select 'PersonName', 2, 50, null
	
	
	delete [puppy].ValueRangesInt;
	insert into [puppy].ValueRangesInt(
	UdfName, 
	[MinValue],
	[MaxValue],
	[CustomValidationMessage])
	select 'GradeLevel', 6, 12, null

	
	delete [puppy].ValuesAllowed;
	insert into [puppy].ValuesAllowed(
	UdfName, 
	[Label],
	AllowedValue)
	select 'CountyCode', 'Polk', 'PLK'
	union all
	select 'CountyCode', 'Hardee', 'HRD'