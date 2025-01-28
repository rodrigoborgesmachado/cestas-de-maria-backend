using Newtonsoft.Json;

namespace CestasDeMaria.Presentation.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Call the next middleware in the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    Message = "An error occurred",
                    Exception = ex.Message,
                    ex.StackTrace,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                };

                // Serialize the error response to JSON
                var json = JsonConvert.SerializeObject(errorResponse);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;

                // Write the JSON response to the body
                await context.Response.WriteAsync(json);
            }
        }
    }
}
