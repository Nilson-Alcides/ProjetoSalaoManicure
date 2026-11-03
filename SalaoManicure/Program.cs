using SalaoManicure.Repositories;
using SalaoManicure.Repository;
using SalaoManicure.Repository.Contract;
using SalaoManicure.Services;
using SalaoManicure.Services.Contract;


var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Repositories
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IProfissionalRepository, ProfissionalRepository>();
builder.Services.AddScoped<IServicoRepository, ServicoRepository>();
builder.Services.AddScoped<IDisponibilidadeRepository, DisponibilidadeRepository>();
builder.Services.AddScoped<IBloqueioRepository, BloqueioRepository>();
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();



// Services
builder.Services.AddScoped<JwtService>();

// Mercado Pago
builder.Services.AddHttpClient<IMercadoPagoService, MercadoPagoService>(
    client =>
    {
        client.BaseAddress = new Uri("https://api.mercadopago.com/");
        client.Timeout = TimeSpan.FromSeconds(30);
    });

// Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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

app.UseSession();

// Mantenha os middlewares de autenticação/autorização
// conforme a configuração de autenticação já existente no projeto.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();