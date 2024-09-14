using Puppy.Types;
using PuppySqlWrapper;
using Xunit.Abstractions;

namespace PuppyApp.Tests;

public class SqlParamTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private string connStr = @"Data Source=(localdb)\MSSQLLocalDB;Database=SampleDb;Integrated Security=True;";

    public SqlParamTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task TestReadParamsForSpIntoPropDescriptors()
    {

        var spName = "dbo.spEnrollStudent";
        var service = new StoredProcParamsService();
        var paramsTypes = (await service.GetParametersAsPropDescriptors(connStr, spName)).ChildProperties.ToList();
        
        foreach (var paramType in paramsTypes)
        {
            _testOutputHelper.WriteLine(paramType.ToString());
        }

        var firstName = paramsTypes.FirstOrDefault(p => p.Name == "FirstName") as StringPropertyDescriptor;
        Assert.NotNull(firstName);
        Assert.Equal(2, firstName.MinLength);
        
        var gradeLevel = paramsTypes.FirstOrDefault(p => p.Name == "GradeLevel") as IntPropertyDescriptor;
        Assert.NotNull(gradeLevel);
        Assert.Equal(6, gradeLevel.MinValue);
        Assert.Equal(12, gradeLevel.MaxValue);
        
        var gpa = paramsTypes.FirstOrDefault(p => p.Name == "GPA") as DecimalPropertyDescriptor;
        Assert.NotNull(gpa);
        Assert.Equal(-9.99M, gpa.MinValue);
        Assert.Equal(9.99M, gpa.MaxValue);
        
        Assert.True(paramsTypes.Count > 0);
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