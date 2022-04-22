﻿CREATE PROCEDURE [dbo].[spEnrollStudent]
	@FirstName varchar(50)
           ,@LastName varchar(50)
           ,@Age int
           ,@EnrolledOn datetime
           ,@GradeLevel int
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