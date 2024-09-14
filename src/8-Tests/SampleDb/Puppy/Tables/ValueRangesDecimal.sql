CREATE TABLE [puppy].[ValueRangesDecimal]
(
	UdfName varchar(70) NOT NULL PRIMARY KEY, 
	[MinValue] numeric(16,4),
	[MaxValue] numeric(16,4),
	[CustomValidationMessage] varchar(70),
)
