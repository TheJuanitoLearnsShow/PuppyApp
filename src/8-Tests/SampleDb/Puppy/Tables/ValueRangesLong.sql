CREATE TABLE [puppy].[ValueRangesLong]
(
	UdfName varchar(70) NOT NULL PRIMARY KEY, 
	[MinValue] bigint,
	[MaxValue] bigint,
	[CustomValidationMessage] varchar(70)
);