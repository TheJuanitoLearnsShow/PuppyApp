msbuild "SampleDb.sqlproj" 
dotnet tool install -g microsoft.sqlpackage
SqlPackage /Action:Publish /SourceFile:".\bin\Release\SampleDb.dacpac" /TargetConnectionString:"Data Source=(localdb)\MSSQLLocalDB;Database=SampleDb;Integrated Security=True;" /p:DropObjectsNotInSource=True /p:ScriptDatabaseCompatibility=True /p:BlockOnPossibleDataLoss=False
