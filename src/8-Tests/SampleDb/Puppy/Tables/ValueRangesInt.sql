CREATE TABLE [puppy].[ValueRangesInt]
(
	UdfName varchar(70) NOT NULL PRIMARY KEY, 
	[MinValue] int,
	[MaxValue] int,
	[CustomValidationMessage] varchar(70)
);


