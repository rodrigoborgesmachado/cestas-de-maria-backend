using CestasDeMaria.Presentation.Api.App_Start;
using CestasDeMaria.Presentation.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

new Start(builder).Builder();

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger/index.html");
    }
    else
    {
        await next();
    }
});

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.EnablePersistAuthorization();
    options.DefaultModelExpandDepth(-1);
    options.DefaultModelsExpandDepth(-1);
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    options.EnableFilter();
});

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.UseAuthentication();

app.UseAuthorization();

app.Run();
