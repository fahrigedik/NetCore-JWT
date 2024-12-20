using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Dtos;

namespace AuthServer.API.Extensions
{
    public static class AddCustomValidationResponse
    {
        public static void UseCustomValidationResponse(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values.Where(x => x.Errors.Count > 0).SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage);
                    var errorDto = new ErrorDto(errors.ToList(), true);
                    var response = Response<ErrorDto>.Fail(errorDto, 400);

                    return new BadRequestObjectResult(response);
                };
            });
        }
    }
}
