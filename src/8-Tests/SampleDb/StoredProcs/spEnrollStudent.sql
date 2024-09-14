CREATE PROCEDURE [dbo].[spEnrollStudent]
	@FirstName PersonName
           ,@LastName PersonName
           ,@Age int
           ,@EnrolledOn datetime
           ,@GradeLevel [GradeLevel]
           ,@County CountyCode
AS


INSERT INTO [dbo].[Students]
           ([FirstName]
           ,[LastName]
           ,[Age])
     VALUES
           (@FirstName
           ,@LastName
           ,@Age)

Select SCOPE_IDENTITY() StudentId;

GO

EXEC sys.sp_addextendedproperty @name=N'Lookup Table for GradeLevel', @value=N'Grade Levels' , 
@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'spEnrollStudent'
GO