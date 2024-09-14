msbuild "SampleDb.sqlproj" 
SqlPackage /Action:Publish /SourceFile:".\bin\debug\SampleDb.dacpac" /TargetConnectionString:"Data Source=.\sqlExpress;Database=SampleDb;Integrated Security=True;" /p:DropObjectsNotInSource=True /p:ScriptDatabaseCompatibility=True /p:BlockOnPossibleDataLoss=False
