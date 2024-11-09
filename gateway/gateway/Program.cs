
namespace gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ���������� IHttpClientFactory
            builder.Services.AddHttpClient();
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            /*builder.Services.AddReverseProxy()
       .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
*/
            var app = builder.Build();
            /*// ���������� �������������� �� ��� �������������
            app.UseRouting();*/
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            /*app.MapReverseProxy();  // ������ ��� ���� ���������, ������������ � appsettings.json
*/
            app.Run();
        }
    }
}
