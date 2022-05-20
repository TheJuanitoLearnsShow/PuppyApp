CREATE TABLE [puppy].[StoredProceduresMetaData]
(
	[SpName] varchar(150) PRIMARY KEY,

	[Heading1] varchar(150) not null,
	[ActionType1] varchar(50) not null,
	[ActionURITemplate1] varchar(255) not null,
	[ActionDataTemplate1] varchar(512) not null,
	
	[Heading2] varchar(150) not null,
	[ActionType2] varchar(50) not null,
	[ActionURITemplate2] varchar(255) not null,
	[ActionDataTemplate2] varchar(512) not null,

	
	[Heading3] varchar(150) not null,
	[ActionType3] varchar(50) not null,
	[ActionURITemplate3] varchar(255) not null,
	[ActionDataTemplate3] varchar(512) not null,

	
	[Heading4] varchar(150) not null,
	[ActionType4] varchar(50) not null,
	[ActionURITemplate4] varchar(255) not null,
	[ActionDataTemplate4] varchar(512) not null, 
    CONSTRAINT [CK_StoredProceduresMetaData_ActionType1] CHECK (ActionType1 in ('UriCmdPerRow', 'NavigateToUri', '')),
)
