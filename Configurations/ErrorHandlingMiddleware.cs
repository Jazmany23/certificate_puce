namespace certificated_unemi.Configurations
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            string message;

            if (exception is ArgumentException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                message = exception.Message;
            }
            else
            {
                statusCode = StatusCodes.Status500InternalServerError;
                message = "Error interno del servidor: " + exception.Message;
            }

            context.Response.StatusCode = statusCode;

            var result = new
            {
                code = statusCode,
                message
            };

            return context.Response.WriteAsJsonAsync(result);
        }
    }
}
