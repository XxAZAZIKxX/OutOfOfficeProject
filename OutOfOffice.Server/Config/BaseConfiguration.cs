namespace OutOfOffice.Server.Config;

public class BaseConfiguration
{
    private const string MissingValueExceptionTextFormat = """
                                                           "{0}" value is missing!
                                                           """;

    protected Exception GetSimpleMissingException(string valueName)
    {
        return new Exception(string.Format(MissingValueExceptionTextFormat, valueName));
    }
}