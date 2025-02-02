using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Appointment.Api.Infrastructure.HttpResponses
{
    public static class FromResultsToHttpCode
    {
        public static IActionResult ToHttpResponse(this Result result) => result switch
        {
            { IsSuccess: false } e => new BadRequestObjectResult(e),
            { IsSuccess: true } => new EmptyResult()
            //_ => throw new System.NotImplementedException(),
        };
        public static IActionResult ToHttpResponse<T>(this Result<T, ResultError> result) => result switch
        {
            { IsSuccess: false } e when e.Error is DoesNotExistError => new NotFoundObjectResult(e.Error.Message),
            { IsSuccess: false } e when e.Error is BadInputError => new BadRequestObjectResult(new
            {
                ErrorCode = HttpStatusCode.BadRequest,
                e.Error.Message
            }),
            { IsSuccess: false } e => new BadRequestObjectResult(new
            {
                ErrorCode = HttpStatusCode.BadRequest,
                Message = e.Error
            }),
            { IsSuccess: true } r when r.Value is null => new EmptyResult(),
            { IsSuccess: true } r => new OkObjectResult(r.Value)
            //_ => throw new System.NotImplementedException(),
        };

        public static IActionResult ToHttpResponse<T>(this Result<T, UnauthorizedError> result) => result switch
        {
            { IsSuccess: false } e => new UnauthorizedObjectResult(new
            {
                ErrorCode = HttpStatusCode.BadRequest,
                Message = e.Error
            }),
            { IsSuccess: true } r when r.Value is null => new EmptyResult(),
            { IsSuccess: true } r => new OkObjectResult(r.Value)
            //_ => throw new System.NotImplementedException(),
        };
        public static IActionResult ToHttpResponse<T>(this Result<T> result) => result switch
        {
            //TODO: Solve it 
            { IsSuccess: false } e => new BadRequestObjectResult(new
            {
                ErrorCode = HttpStatusCode.BadRequest,
                Message = e.Error
            }),
            { IsSuccess: true } r => new OkObjectResult(r.Value)
            //_ => new EmptyResult(),
        };
    }
}
