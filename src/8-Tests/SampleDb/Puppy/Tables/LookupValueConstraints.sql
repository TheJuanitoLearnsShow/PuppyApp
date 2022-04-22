CREATE TABLE [puppy].[LookupValueConstraints]
(
	UdfName varchar(70) NOT NULL, 
	[ObjectForSearch] varchar(70),
	[SearchParameterName] varchar(70) NOT NULL, 
	[IdColumnName] varchar(70) NOT NULL, 
	[LabelColumnName] varchar(70) NOT NULL, 
	[IsStoredProc] bit not null,
    CONSTRAINT [PK_LookupValueConstraints] PRIMARY KEY ([UdfName]) 
)
