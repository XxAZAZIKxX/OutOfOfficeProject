using OutOfOffice.Core.Exceptions.NotFound.Base;

namespace OutOfOffice.Core.Exceptions.NotFound;

public class EmployeeNotFoundException(string message) : BaseNotFoundException(message);