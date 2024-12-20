using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using SharedLibrary.Dtos;

namespace AuthServer.API.Extensions
{
    public static class CustomExceptionHandle
    {
        public static void CustomException(this IApplicationBuilder app)
        {

            app.UseExceptionHandler(configure =>
            {
                configure.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    ErrorDto errorDto = null;
                    if (errorFeature != null)
                    {
                        var ex = errorFeature.Error;
                        errorDto = new ErrorDto(ex.Message, true);
                    }

                    var response = Response<NoDataDto>.Fail(errorDto, 500);

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                });
            });

        }
    }
}
