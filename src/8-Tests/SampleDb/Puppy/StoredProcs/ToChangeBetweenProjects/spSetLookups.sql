-- This stored procedure would chnage depending on the app, it is not generic
CREATE PROCEDURE [puppy].[spSetLookups]
AS
delete [puppy].[LookupValueConstraints]
insert into [puppy].[LookupValueConstraints](UdfName,
                                             [ObjectForSearch],
                                             [SearchParameterName],
                                             [IdColumnName],
                                             [LabelColumnName],
                                             [IsStoredProc])
select 'GradeLevel', 'Grade Levels', 'Grade Name', 
       'Grade Level', 'Grade Name', 0
