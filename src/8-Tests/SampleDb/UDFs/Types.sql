CREATE TYPE [dbo].[GradeLevel]
	FROM int NOT NULL;
go;

CREATE TYPE [dbo].PersonName
	FROM varchar(50) NOT NULL;
go;

CREATE TYPE [dbo].CountyCode
	FROM varchar(3) NOT NULL;
go;
