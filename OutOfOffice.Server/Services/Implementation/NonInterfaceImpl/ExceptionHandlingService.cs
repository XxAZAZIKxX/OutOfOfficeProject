using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Core.Exceptions.NotFound.Base;

namespace OutOfOffice.Server.Services.Implementation.NonInterfaceImpl;

public sealed class ExceptionHandlingService
{
    public ActionResult<T> HandleException<T>(Exception exception) => new(HandleException(exception));
    public Task<ActionResult<T>> HandleExceptionAsync<T>(Exception exception) => Task.FromResult(HandleException<T>(exception));

    public ActionResult HandleException(Exception exception)
    {
        return exception switch
        {
            BaseNotFoundException notFound => new NotFoundObjectResult(notFound),
            _ => throw exception
        };
    }
    public Task<ActionResult> HandleExceptionAsync(Exception exception) => Task.FromResult(HandleException(exception));
}