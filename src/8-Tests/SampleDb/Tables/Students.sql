CREATE TABLE [dbo].[Students]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
	[FirstName] PersonName,
	[LastName] PersonName,
	[Age] int NOT NULL
)
