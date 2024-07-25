using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommonCore.Result
{
    public class ApiResultFilterAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            /* if (context.Filters.Any(filterMetadata => filterMetadata.GetType() == typeof(ApiResultFilterForbidAttribute)))
             {
                 return;
             }*/
            switch (context.Result)
            {
                case ObjectResult result:
                    {
                        // this include OkObjectResult, NotFoundObjectResult, BadRequestObjectResult, CreatedResult (lose Location)
                        var objectResult = result;
                        if (objectResult.Value == null)
                        {
                            context.Result = new ObjectResult(ApiResult.Empty);
                        }
                        else if (objectResult.Value is ValidationProblemDetails validationProblemDetails)
                        {
                            var errors = string.Empty;
                            foreach (var error in validationProblemDetails.Errors)
                            {
                                errors += string.IsNullOrEmpty(errors) ? error.Value.First()
                                    : $",{error.Value.First()}";
                            }
                            context.Result = new ObjectResult(ApiResult.Failed(errors));
                        }
                        else if (!(objectResult.Value is IApiResult))
                        {
                            if (objectResult.DeclaredType != null)
                            {
                                var apiResult = Activator.CreateInstance(
                                    typeof(ApiResult<>).MakeGenericType(objectResult.DeclaredType), objectResult.Value, objectResult.StatusCode);
                                context.Result = new ObjectResult(apiResult);
                            }
                            else
                            {
                                context.Result = objectResult;
                            }
                        }

                        break;
                    }
                case EmptyResult _:
                    // return void or Task
                    context.Result = new ObjectResult(ApiResult.Empty);
                    break;

                case ContentResult result:
                    context.Result = new ObjectResult(ApiResult.Succeed(result.Content));
                    break;

                case StatusCodeResult result:
                    // this include OKResult, NoContentResult, UnauthorizedResult, NotFoundResult, BadRequestResult
                    context.Result = new ObjectResult(ApiResult.From(result.StatusCode));
                    break;
            }
        }
    }
}