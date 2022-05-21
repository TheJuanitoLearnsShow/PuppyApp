using PuppySqlWrapper;
using Xunit.Abstractions;

namespace PuppyApp.Tests;

public class SqlParamTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private string connStr = @"Data Source=.\sqlexpress;Database=SampleDb;Integrated Security=True;Pooling=False;MultipleActiveResultSets=True;Connect Timeout=60;";

    public SqlParamTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task TestReadParamsForSp()
    {

        var spName = "dbo.spEnrollStudent";
        var service = new StoredProcParamsService();
        var paramsTypes = await service.GetParametersTypes(connStr, spName);

        foreach (var paramType in paramsTypes)
        {
            _testOutputHelper.WriteLine(paramType.ToString());
        }
        
        Assert.True(paramsTypes.Count > 0);
        Assert.True(paramsTypes.Count(p => p.ClrTypeName == "String") == 2);
    }
}