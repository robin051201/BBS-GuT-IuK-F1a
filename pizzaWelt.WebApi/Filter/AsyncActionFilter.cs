namespace PizzaWelt.Filter
{
    public class AsyncActionFilter : IAsyncActionFilter
    {
        private readonly ServiceHelper _serviceHelper;

        public AsyncActionFilter(ServiceHelper serviceHelper)
        {
            _serviceHelper = serviceHelper;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string? token = await context.HttpContext.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token");

            if (!await _serviceHelper.IsAccessTokenValid(token))
            {
                context.Result = new UnauthorizedObjectResult(new AuthResponseDto { ErrorMessage = "Token is not valid!" });
            }

            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }

            _ = await next();
        }
    }
}
