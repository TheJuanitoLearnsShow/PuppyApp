CREATE TABLE [puppy].ValueLengthRangesString
(
	UdfName varchar(70) NOT NULL PRIMARY KEY, 
	[MinLength] int ,
	[MaxLength] int ,
	[CustomValidationMessage] varchar(70),
)
