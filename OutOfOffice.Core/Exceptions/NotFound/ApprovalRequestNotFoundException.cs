using OutOfOffice.Core.Exceptions.NotFound.Base;

namespace OutOfOffice.Core.Exceptions.NotFound;

public class ApprovalRequestNotFoundException(string message) : BaseNotFoundException(message);