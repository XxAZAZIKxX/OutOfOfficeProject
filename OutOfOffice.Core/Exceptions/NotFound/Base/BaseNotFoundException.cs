namespace OutOfOffice.Core.Exceptions.NotFound.Base;

public abstract class BaseNotFoundException(string message) : Exception(message);