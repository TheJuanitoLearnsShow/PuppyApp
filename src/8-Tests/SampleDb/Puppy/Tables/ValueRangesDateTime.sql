CREATE TABLE [puppy].[ValueRangesDateTime]
(
	UdfName varchar(70) NOT NULL PRIMARY KEY, 
	[MinValue] DateTime ,
	[MinValueDaysOffset] int ,
	[MaxValue] DateTime ,
	[MaxValueDaysOffset] int ,
	[CustomValidationMessage] varchar(70),
)
