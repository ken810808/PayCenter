namespace PayCenter.MiddleWares
{
    public class TokenAuthMiddlewarePipeline
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseTokenAuthMiddleware();
        }
    }
}
