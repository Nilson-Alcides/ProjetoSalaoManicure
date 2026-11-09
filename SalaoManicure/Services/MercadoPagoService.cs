using Microsoft.Extensions.Configuration;
using SalaoManicure.Services.Contract;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SalaoManicure.Services
{
    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public MercadoPagoService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<SalaoManicure.Services.Contract.ResultadoPagamento>
     CriarOrderAsync(
             int agendamentoId,
            string nomeServico,
            decimal valor,
            string emailCliente,
            string nomeCliente,
            string urlSucesso,
            string urlFalha,
            string urlPendente)
        {
            var accessToken =
                _configuration["MercadoPago:AccessToken"];

            if (string.IsNullOrWhiteSpace(accessToken))
                throw new InvalidOperationException(
                    "Access Token do Mercado Pago não configurado.");

            if (agendamentoId <= 0)
                throw new ArgumentException("Agendamento inválido.");

            if (valor <= 0)
                throw new ArgumentException(
                    "O valor do pagamento deve ser maior que zero.");

            if (string.IsNullOrWhiteSpace(emailCliente))
                throw new ArgumentException(
                    "O e-mail do cliente é obrigatório.");

            var valorFormatado = valor.ToString(
                "F2",
                CultureInfo.InvariantCulture);

            //var pedido = new
            //{
            //    type = "online",
            //    processing_mode = "manual",
            //    capture_mode = "automatic_async",
            //    total_amount = valorFormatado,
            //    external_reference = agendamentoId.ToString(),

            //    description = $"Sinal de 30% - {nomeServico}",

            //    payer = new
            //    {
            //        email = emailCliente
            //    },

            //    items = new[]
            //    {
            //        new
            //        {
            //            title = $"Sinal de 30% - {nomeServico}",
            //            quantity = 1,
            //            unit_price = valorFormatado
            //        }
            //    }
            //};
            var pedido = new
            {
                type = "online",
                processing_mode = "manual",
                capture_mode = "automatic_async",
                total_amount = valorFormatado,
                external_reference = agendamentoId.ToString(),

                description = $"Sinal de 30% - {nomeServico}",

                payer = new
                {
                    email = emailCliente
                },

                items = new[]
    {
        new
        {
            title = $"Sinal de 30% - {nomeServico}",
            quantity = 1,
            unit_price = valorFormatado
        }
    },

                config = new
                {
                    online = new
                    {
                        success_url = urlSucesso,
                        failure_url = urlFalha,
                        pending_url = urlPendente,
                        auto_return = "approved"
                    }
                }
            };

            using var requisicao = new HttpRequestMessage(
                HttpMethod.Post,
                "v1/orders");

            requisicao.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    accessToken);

            requisicao.Headers.Add(
                "X-Idempotency-Key",
                Guid.NewGuid().ToString());

            requisicao.Content = JsonContent.Create(pedido);

            using var resposta =
                await _httpClient.SendAsync(requisicao);

            var conteudo = await resposta.Content.ReadAsStringAsync();

            //Console.WriteLine("HTTP: " + (int)resposta.StatusCode);
            //Console.WriteLine("RESPOSTA MERCADO PAGO:");
            //Console.WriteLine(conteudo);

            if (!resposta.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Erro ao criar pagamento no Mercado Pago. " +
                    $"Status: {(int)resposta.StatusCode}. " +
                    $"Resposta: {conteudo}");
            }

            using var documento = JsonDocument.Parse(conteudo);

            var raiz = documento.RootElement;

            var orderId = raiz.TryGetProperty(
                "id", out var idElement)
                    ? idElement.GetString()
                    : null;

            var checkoutUrl = raiz.TryGetProperty(
                "checkout_url", out var checkoutElement)
                    ? checkoutElement.GetString()
                    : null;

            var status = raiz.TryGetProperty(
                "status", out var statusElement)
                    ? statusElement.GetString()
                    : null;

            if (string.IsNullOrWhiteSpace(orderId) ||
                string.IsNullOrWhiteSpace(checkoutUrl))
            {
                throw new InvalidOperationException(
                    "O Mercado Pago não retornou o ID da Order ou a URL de checkout.");
            }

            return new SalaoManicure.Services.Contract.ResultadoPagamento
            {
                OrderId = orderId,
                CheckoutUrl = checkoutUrl,
                Status = status ?? "created"
            };
        }

        public async Task<ResultadoConsultaOrder> ConsultarOrderAsync(
    string orderId)
        {
            var accessToken =
                _configuration["MercadoPago:AccessToken"];

            if (string.IsNullOrWhiteSpace(accessToken))
                throw new InvalidOperationException(
                    "Access Token do Mercado Pago não configurado.");

            if (string.IsNullOrWhiteSpace(orderId))
                throw new ArgumentException("Order ID inválido.");

            using var requisicao = new HttpRequestMessage(
                HttpMethod.Get,
                $"v1/orders/{Uri.EscapeDataString(orderId)}");

            requisicao.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            using var resposta = await _httpClient.SendAsync(requisicao);

            var conteudo = await resposta.Content.ReadAsStringAsync();

            if (!resposta.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Erro ao consultar Order no Mercado Pago. " +
                    $"Status: {(int)resposta.StatusCode}. " +
                    $"Resposta: {conteudo}");
            }

            using var documento = JsonDocument.Parse(conteudo);
            var raiz = documento.RootElement;

            string status = raiz.TryGetProperty("status", out var statusElement)
                ? statusElement.GetString() ?? ""
                : "";

            string statusDetail = raiz.TryGetProperty(
                "status_detail", out var detailElement)
                    ? detailElement.GetString() ?? ""
                    : "";

            string externalReference = raiz.TryGetProperty(
                "external_reference", out var referenceElement)
                    ? referenceElement.GetString() ?? ""
                    : "";

            string? paymentId = null;

            if (raiz.TryGetProperty("transactions", out var transactions) &&
                transactions.TryGetProperty("payments", out var payments) &&
                payments.ValueKind == JsonValueKind.Array &&
                payments.GetArrayLength() > 0)
            {
                var primeiroPagamento = payments[0];

                if (primeiroPagamento.TryGetProperty("id", out var paymentElement))
                {
                    paymentId = paymentElement.GetString();
                }
            }

            return new ResultadoConsultaOrder
            {
                OrderId = orderId,
                Status = status,
                StatusDetail = statusDetail,
                ExternalReference = externalReference,
                PaymentId = paymentId
            };
        }

    }
}