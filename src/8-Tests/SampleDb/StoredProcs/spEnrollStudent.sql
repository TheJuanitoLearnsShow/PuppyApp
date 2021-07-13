CREATE PROCEDURE [dbo].[spEnrollStudent]
	@FirstName varchar(50)
           ,@LastName varchar(50)
           ,@Age int
AS


INSERT INTO [dbo].[Students]
           ([FirstName]
           ,[LastName]
           ,[Age])
     VALUES
           (@FirstName
           ,@LastName
           ,@Age)
GO

