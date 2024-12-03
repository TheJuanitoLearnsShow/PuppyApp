using Puppy.Types;
using Xunit.Abstractions;

namespace PuppyApp.Tests;

public class PuppyTypesTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public PuppyTypesTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TestComplexObj()
    {

        var model = new ComplexPropertyDescriptor([
            new IntPropertyDescriptor("Age", 3, true ),
            new IntPropertyDescriptor("Grade", 2, false ),
            new StringPropertyDescriptor("MiddleName", 3, true ),
            new DateTimeOffsetPropertyDescriptor("EnrollmentDate", true ),
            
        ], "StudentEnrollment");

        var dataInput = new DataEntryInput();
        dataInput.ChildrenEntries = new () {
            {"Age", new DataEntryInput() { PrimitiveValue = "10"}},
            {"Grade", new DataEntryInput() { PrimitiveValue = "29999"}},
            {"MiddleName", new DataEntryInput() { PrimitiveValue = "Abadacus"}},
            {"EnrollmentDate", new DataEntryInput() { PrimitiveValue = "2024-09-01"}},
        };
        var state = new DictDataEntryValuesState();
        var results = model.Parse(dataInput, state);
    
        Assert.False(results.IsValid);

        Assert.NotNull(results.ChildrenEntries);
        var ageResults = results.ChildrenEntries["Grade"].Errors;
        Assert.NotEmpty(ageResults);
        Assert.Contains(ageResults, e => e.Description.StartsWith("Must not be greater"));

        Assert.Equal(10, state.GetInt("Age"));
        
        var middleNameResults = results.ChildrenEntries["MiddleName"].Errors;
        Assert.NotEmpty(middleNameResults);
        Assert.Contains(middleNameResults, e => e.Description.StartsWith("Length") && e.Code == ValidationErrorCode.ExceedsLength);
    }
    [Fact]
    public void TestComplexObj_Validation()
    {

        var model = new ComplexPropertyDescriptor([
            new IntPropertyDescriptor("Age", 3, true ),
            new IntPropertyDescriptor("Grade", 2, false ),
            new StringPropertyDescriptor("MiddleName", 3, true ),
            new DateTimeOffsetPropertyDescriptor("EnrollmentDate", true ),
            
        ], "StudentEnrollment");

        var dataInput = new DataEntryInput();
        dataInput.ChildrenEntries = new () {
            {"Age", new DataEntryInput() { PrimitiveValue = "10"}},
            {"Grade", new DataEntryInput() { PrimitiveValue = "29999"}},
            {"MiddleName", new DataEntryInput() { PrimitiveValue = "Abadacus"}},
            {"EnrollmentDate", new DataEntryInput() { PrimitiveValue = "2024-09-01"}},
        };
        var state = new DictDataEntryValuesState();
        var results = model.Parse(dataInput, state);
    
        Assert.False(results.IsValid);

        Assert.NotNull(results.ChildrenEntries);
        var ageResults = results.ChildrenEntries["Grade"].Errors;
        Assert.NotEmpty(ageResults);
        Assert.Contains(ageResults, e => e.Description.StartsWith("Must not be greater"));

        Assert.Equal(10, state.GetInt("Age"));
        
        var middleNameResults = results.ChildrenEntries["MiddleName"].Errors;
        Assert.NotEmpty(middleNameResults);
        Assert.Contains(middleNameResults, e => e.Description.StartsWith("Length") && e.Code == ValidationErrorCode.ExceedsLength);
    }
}