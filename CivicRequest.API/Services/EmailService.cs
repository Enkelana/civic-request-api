using MailKit.Net.Smtp;
using MimeKit;

namespace CivicRequest.API.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string toName, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _config["Email:SenderName"],
                _config["Email:SenderEmail"]
            ));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _config["Email:SmtpHost"],
                int.Parse(_config["Email:SmtpPort"]!),
                false
            );
            await client.AuthenticateAsync(
                _config["Email:SenderEmail"],
                _config["Email:Password"]
            );
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public string KerkesaEReTemplate(string citizenName, string title, string category) => $@"
        <div style='font-family: Arial; max-width: 600px; margin: 0 auto;'>
            <div style='background: #1F4E79; padding: 20px; text-align: center;'>
                <h1 style='color: white; margin: 0;'>🏛️ CivicRequest</h1>
            </div>
            <div style='padding: 30px; background: #f9f9f9;'>
                <h2 style='color: #1F4E79;'>Kërkesa juaj u regjistrua!</h2>
                <p>I/e nderuar <strong>{citizenName}</strong>,</p>
                <p>Kërkesa juaj është regjistruar me sukses në sistemin CivicRequest.</p>
                <div style='background: white; padding: 15px; border-left: 4px solid #2E75B6; margin: 20px 0;'>
                    <p><strong>Titulli:</strong> {title}</p>
                    <p><strong>Kategoria:</strong> {category}</p>
                    <p><strong>Statusi:</strong> Në Pritje</p>
                </div>
                <p>Do të njoftoheni kur statusi i kërkesës të ndryshojë.</p>
            </div>
            <div style='background: #1F4E79; padding: 15px; text-align: center;'>
                <p style='color: #BDD7EE; margin: 0; font-size: 12px;'>CivicRequest © 2026 - Agjencia e Inovacionit dhe Ekselencës</p>
            </div>
        </div>";

        public string StatusNdryshimTemplate(string citizenName, string title, string status, string notes) => $@"
        <div style='font-family: Arial; max-width: 600px; margin: 0 auto;'>
            <div style='background: #1F4E79; padding: 20px; text-align: center;'>
                <h1 style='color: white; margin: 0;'>🏛️ CivicRequest</h1>
            </div>
            <div style='padding: 30px; background: #f9f9f9;'>
                <h2 style='color: #1F4E79;'>Statusi i kërkesës suaj ndryshoi!</h2>
                <p>I/e nderuar <strong>{citizenName}</strong>,</p>
                <div style='background: white; padding: 15px; border-left: 4px solid #70AD47; margin: 20px 0;'>
                    <p><strong>Titulli:</strong> {title}</p>
                    <p><strong>Statusi i ri:</strong> {status}</p>
                    <p><strong>Shënime:</strong> {notes}</p>
                </div>
            </div>
            <div style='background: #1F4E79; padding: 15px; text-align: center;'>
                <p style='color: #BDD7EE; margin: 0; font-size: 12px;'>CivicRequest © 2026 - Agjencia e Inovacionit dhe Ekselencës</p>
            </div>
        </div>";
    }
}