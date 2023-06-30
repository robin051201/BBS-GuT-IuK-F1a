namespace PizzaWelt.Extension
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
              {
                  appError.Run(async context =>
                  {
                      context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                      context.Response.ContentType = "application/json";
                      IExceptionHandlerFeature? contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                      if (contextFeature != null)
                      {
                          await context.Response.WriteAsync(new ErrorDetails()
                          {
                              StatusCode = context.Response.StatusCode,
                              Message = "Internal Server Error."
                          }.ToString());
                      }
                  });
              });
        }
    }

    internal class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
