
using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Libraries.Seguranca;
using SalaoManicure.Repository.Contract;
using SalaoManicure.Services.Contract;

namespace SalaoManicure.Controllers
{
    [ApiController]
    [Route("webhook/mercadopago")]
    public class MercadoPagoWebhookController : ControllerBase
    {
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IMercadoPagoService _mercadoPagoService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MercadoPagoWebhookController> _logger;

        public MercadoPagoWebhookController(
            IPagamentoRepository pagamentoRepository,
            IMercadoPagoService mercadoPagoService,
            IConfiguration configuration,
            ILogger<MercadoPagoWebhookController> logger)
        {
            _pagamentoRepository = pagamentoRepository;
            _mercadoPagoService = mercadoPagoService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ReceberNotificacao()
        {
            try
            {
                // Aceitar somente notificações de Order.

                var tipo = Request.Query["type"].ToString();
                var dataId = Request.Query["data.id"].ToString();

                if (!string.Equals(
                    tipo, "order",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return Ok();
                }

                if (string.IsNullOrWhiteSpace(dataId))
                    return BadRequest();

                // Validar assinatura
                var assinatura =
                    Request.Headers["x-signature"].ToString();

                var requestId =
                    Request.Headers["x-request-id"].ToString();

                var segredo =
                    _configuration["MercadoPago:WebhookSecret"];

                bool assinaturaValida =
                    WebhookMercadoPagoValidator.Validar(
                        assinatura,
                        requestId,
                        dataId,
                        segredo);

                if (!assinaturaValida)
                {
                    _logger.LogWarning(
                        "Webhook Mercado Pago recebido com assinatura inválida.");

                    return Unauthorized();
                }

                // Consultar a Order diretamente no Mercado Pago
                var order =
                    await _mercadoPagoService.ConsultarOrderAsync(dataId);

                // Conferir se a Order consultada corresponde à notificação.
                if (!string.Equals(
                    order.OrderId,
                    dataId,
                    StringComparison.Ordinal))
                {
                    return BadRequest();
                }

                // Localizar pagamento no banco
                var pagamento =
                    _pagamentoRepository.BuscarPorOrderId(order.OrderId);

                if (pagamento == null)
                {
                    _logger.LogWarning(
                        "Pagamento não encontrado para a Order {OrderId}.",
                        order.OrderId);

                    return NotFound();
                }

                // Conferir vínculo entre Order e agendamento
                if (order.ExternalReference !=
                    pagamento.AgendamentoId.ToString())
                {
                    _logger.LogWarning(
                        "ExternalReference divergente para a Order {OrderId}.",
                        order.OrderId);

                    return BadRequest();
                }

                // Confirmar somente pagamento efetivamente processado.
                if (order.Status == "processed" && order.StatusDetail == "accredited")
                {
                    if (string.IsNullOrWhiteSpace(order.PaymentId))
                    {
                        _logger.LogWarning(
                            "Order {OrderId} processada sem PaymentId.",
                            order.OrderId);

                        return StatusCode(503);
                    }

                    if (pagamento.Status != "Aprovado")
                    {
                        _pagamentoRepository.ConfirmarPagamento(
                            pagamento.Id,
                            order.PaymentId);
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao processar Webhook do Mercado Pago.");

                return StatusCode(500);
            }
        }
    }
}
