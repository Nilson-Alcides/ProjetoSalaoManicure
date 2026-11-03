
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;

namespace SalaoManicure.Libraries.Filtro
{
    public class ClienteAutorizacaoAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var usuario = context.HttpContext.User;

            // 1. Verifica se o JWT foi validado e autenticou o usuário.
            if (usuario.Identity?.IsAuthenticated != true)
            {
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Auth",
                    null);

                return;
            }

            // 2. Recupera o ID do usuário a partir das claims do JWT.
            string? usuarioIdTexto =
                usuario.FindFirst("UsuarioId")?.Value
                ?? usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? usuario.FindFirst("sub")?.Value;

            if (!int.TryParse(usuarioIdTexto, out int usuarioId))
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Content = "Acesso negado: identificação do usuário inválida."
                };

                return;
            }

            // 3. Consulta o cadastro do cliente.
            var clienteRepository = context.HttpContext.RequestServices
                .GetRequiredService<IClienteRepository>();

            Cliente? cliente =
                clienteRepository.BuscarPorUsuarioId(usuarioId);

            if (cliente == null)
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Content = "Acesso negado: cadastro de cliente não encontrado."
                };

                return;
            }

            // 4. Disponibiliza o cliente autenticado para o controller.
            context.HttpContext.Items["ClienteLogado"] = cliente;
        }
    }
}
