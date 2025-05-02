using Oracle.ManagedDataAccess.Client;
using System.Net.Mail;
using System.Net;
using System.Data;
using System.Globalization;

namespace CreditFollowupService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                string connStr = @"User Id=C##CRMUSER;
                                   Password=Password1!;
                                   Data Source=(DESCRIPTION=
                                     (ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1522))
                                     (CONNECT_DATA=(SERVICE_NAME=ORCLPDB1)))";

                using var connection = new OracleConnection(connStr);
                connection.Open();
                _logger.LogInformation("Oracle bağlantısı BAŞARILI.");

                using var command = new OracleCommand("GET_OVERDUE_CUSTOMERS", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("CURSOR_OUT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                using var reader = command.ExecuteReader();

                string emailBody = "";
                if (!reader.HasRows)
                {
                    _logger.LogWarning("Sorgu çalıştı ama hiçbir kayıt dönmedi.");
                }
                else
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        DateTime lastDate = reader.GetDateTime(1);
                        decimal debt = reader.GetDecimal(2);
                        int daysLate = reader.GetInt32(3);

                        _logger.LogInformation($"📌 {name} - {daysLate} gün gecikmiş - {debt} TL");
                        emailBody += $"- {name}: {daysLate} gün gecikmiş, borç: {debt.ToString("C", new CultureInfo("tr-TR"))}\n";
                    }
                }

                if (!string.IsNullOrEmpty(emailBody))
                {
                    _logger.LogInformation($"📧 E-posta içeriği:\n{emailBody}");
                    SendEmail(emailBody);
                    _logger.LogInformation("📬 Takibe düşen krediler için mail gönderildi.");
                }
                else
                {
                    _logger.LogInformation("🔍 Takibe düşen kredi bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Hata: {ex.Message}\n{ex.StackTrace}");
            }

        }

        private void SendEmail(string body)
        {
            var mail = new MailMessage("alcaydamla2@gmail.com", "alcaydamla2@gmail.com");
            mail.Subject = "📢 Takibe Düşen Krediler";
            mail.Body = $"Aşağıdaki müşteriler 90+ gün gecikmiş durumda:\n\n{body}";

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("alcaydamla2@gmail.com", "naalaogrrentlgrn"),
                EnableSsl = true
            };

            smtp.Send(mail);
        }
    }
}
