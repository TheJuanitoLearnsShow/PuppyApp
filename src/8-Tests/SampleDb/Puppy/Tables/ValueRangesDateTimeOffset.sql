CREATE TABLE [puppy].[ValueRangesDateTimeOffset]
(
	UdfName varchar(70) NOT NULL PRIMARY KEY, 
	[MinValue] DateTimeOffset ,
	[MinValueDaysOffset] int ,
	[MaxValue] DateTimeOffset ,
	[MaxValueDaysOffset] int ,
	[CustomValidationMessage] varchar(70),
)
