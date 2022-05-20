CREATE PROCEDURE puppy.[spSetSpMetadata]
AS
	delete [puppy].StoredProceduresMetaData
	insert into [puppy].StoredProceduresMetaData(
	        [SpName]
           ,[Heading1]
           ,[ActionType1]
           ,[ActionURITemplate1]
           ,[ActionDataTemplate1]
           ,[Heading2]
           ,[ActionType2]
           ,[ActionURITemplate2]
           ,[ActionDataTemplate2]
           ,[Heading3]
           ,[ActionType3]
           ,[ActionURITemplate3]
           ,[ActionDataTemplate3]
           ,[Heading4]
           ,[ActionType4]
           ,[ActionURITemplate4]
           ,[ActionDataTemplate4])
     VALUES
           ('dbo.spEnrollStudent'
           ,'Students Enrollment Results'
           ,'UriCmdPerRow'
           ,'puppy://dbo.spStudentDetails'
           ,'ALL_COLUMNS'
           ,''
           ,''
           ,''
           ,''
           ,''
           ,''
           ,''
           ,''
           ,''
           ,''
           ,''
           ,'')
