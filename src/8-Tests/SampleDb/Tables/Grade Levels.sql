CREATE TABLE [dbo].[Grade Levels]
(
	[Grade Level] INT NOT NULL PRIMARY KEY,
	[Grade Name] varchar(100)
)
go


EXEC sys.sp_addextendedproperty @name=N'Lookup ID', @value=N'Grade Level' , 
@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Grade Levels'
GO
EXEC sys.sp_addextendedproperty @name=N'Lookup Label', @value=N'Grade Name' , 
@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Grade Levels'
GO