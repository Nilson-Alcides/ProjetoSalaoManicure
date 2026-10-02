using SalaoManicure.Repositories;
using SalaoManicure.Repository;
using SalaoManicure.Repository.Contract;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Repositories
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IProfissionalRepository, ProfissionalRepository>();
builder.Services.AddScoped<IServicoRepository, ServicoRepository>();
builder.Services.AddScoped<IDisponibilidadeRepository,DisponibilidadeRepository>();
builder.Services.AddScoped<IBloqueioRepository,BloqueioRepository>();
builder.Services.AddScoped<IAgendamentoRepository,AgendamentoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Controllers MVC e API
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

