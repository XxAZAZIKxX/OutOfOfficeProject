namespace OutOfOffice.Server.Core.Exceptions;

public class ConfigurationMissingException(string message) : KeyNotFoundException(message);