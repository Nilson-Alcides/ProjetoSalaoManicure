using System.Security.Cryptography;
using System.Text;

namespace SalaoManicure.Libraries.Seguranca
{
    public static class WebhookMercadoPagoValidator
    {
        public static bool Validar(
            string? assinatura,
            string? requestId,
            string? dataId,
            string? segredo)
        {
            if (string.IsNullOrWhiteSpace(assinatura) ||
                string.IsNullOrWhiteSpace(requestId) ||
                string.IsNullOrWhiteSpace(dataId) ||
                string.IsNullOrWhiteSpace(segredo))
            {
                return false;
            }

            string? timestamp = null;
            string? hashRecebido = null;

            var partes = assinatura.Split(',');

            foreach (var parte in partes)
            {
                var item = parte.Trim().Split('=', 2);

                if (item.Length != 2)
                    continue;

                if (item[0] == "ts")
                    timestamp = item[1];

                if (item[0] == "v1")
                    hashRecebido = item[1];
            }

            if (string.IsNullOrWhiteSpace(timestamp) ||
                string.IsNullOrWhiteSpace(hashRecebido))
            {
                return false;
            }

            string manifesto =
                $"id:{dataId};request-id:{requestId};ts:{timestamp};";

            using var hmac = new HMACSHA256(
                Encoding.UTF8.GetBytes(segredo));

            byte[] hashCalculado = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(manifesto));

            string hashHex = Convert.ToHexString(
                hashCalculado).ToLowerInvariant();

            try
            {
                byte[] recebido = Convert.FromHexString(hashRecebido);
                byte[] calculado = Convert.FromHexString(hashHex);

                return CryptographicOperations.FixedTimeEquals(
                    recebido,
                    calculado);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}