using OutOfOffice.Core.Exceptions.NotFound.Base;

namespace OutOfOffice.Core.Exceptions.NotFound;

public class LeaveRequestNotFoundException(string message) : BaseNotFoundException(message);