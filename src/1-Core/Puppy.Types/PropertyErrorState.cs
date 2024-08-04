namespace Puppy.Types;

public record PropertyErrorState(string PropertyName, IEnumerable<PropertyError> Errors);