CREATE TABLE [puppy].[ValueRangesMoney]
(
	UdfName varchar(70) NOT NULL PRIMARY KEY, 
	[MinValue] money ,
	[MaxValue] money ,
	[CustomValidationMessage] varchar(70),
)
