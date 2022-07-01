namespace PayCenter.MiddleWares
{
    public class MiddlewarePipeline
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseTokenAuthMiddleware();
        }
    }
}
