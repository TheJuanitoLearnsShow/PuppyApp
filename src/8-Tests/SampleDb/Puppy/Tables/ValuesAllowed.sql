CREATE TABLE [puppy].[ValuesAllowed]
(
	UdfName varchar(70) NOT NULL , 
	[AllowedValue] varchar(70) NOT NULL, 
	[Label] varchar(70) NOT NULL, 
    PRIMARY KEY ([UdfName], [AllowedValue]),
)
