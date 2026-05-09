using Crossdyne.Toolkit.Results;
using Microsoft.AspNetCore.Http;
using Shared.Kernel.Errors;

namespace Shared.Web.Extensions
{
    public static class MinimalApiErrorMappingExtensions
    {
         public static IResult MapToMinimalApiResult(this IReadOnlyList<Error> errors)
        {
            var firstError = errors.FirstOrDefault();
            
            if (firstError is null)
                return Results.StatusCode(StatusCodes.Status500InternalServerError);

            return firstError.Code.Name switch
            {
                nameof(ErrorCode.NotFound) => Results.NotFound(errors),
                
                nameof(ErrorCode.Save) or 
                nameof(ErrorCode.Server) or
                nameof(ErrorCode.Create) => Results.Json(errors, statusCode: StatusCodes.Status500InternalServerError),

                nameof(ErrorCode.Conflict) or 
                nameof(AppErrors.Duplicate) => Results.Conflict(errors),
                
                nameof(AppErrors.SessionExpired) or 
                nameof(ErrorCode.Unauthorized) => Results.Json(errors, statusCode: StatusCodes.Status401Unauthorized),
                
                _ or 
                nameof(AppErrors.Validation) or
                nameof(AppErrors.TimeEnded) or 
                nameof(AppErrors.IncorrectValue) => Results.BadRequest(errors)
            };
        }
    }
}