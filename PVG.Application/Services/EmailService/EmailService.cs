using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using PVG.Domain.Models;
using PVG.Infrastucture.Repositories.ConfigurationRepository;

namespace PVG.Application.Services.EmailService
{
    public class EmailService: IEmailService
    {
        private readonly IConfigurationRepository _configurationRepository;
        public EmailService(IConfigurationRepository configurationRepository)
        {
            _configurationRepository = configurationRepository;
        }

        public async Task<RS_EmailModel> SendEmailRequest(string _subject, string _htmlBody, IEnumerable<IFormFile>? attachments = null)
        {
            try
            {
                var configs = _configurationRepository.FindAll().ToList();
                string fromName = "",
                    fromEmail = "",
                    password = "",
                    toEmail = "",
                    smtpHost = "";
                int port = 0;
                if (configs != null && configs.Count > 0)
                {
                    fromName = configs.Find(x => x.Key == "EmailFromName")?.Value;
                    fromEmail = configs.Find(x => x.Key == "EmailSend")?.Value;
                    password = configs.Find(x => x.Key == "EmailSendPassword")?.Value;
                    toEmail = configs.Find(x => x.Key == "EmailReceive")?.Value;
                    smtpHost = configs.Find(x => x.Key == "EmailSmtpHost")?.Value;
                    port = int.Parse(configs.Find(x => x.Key == "EmailPort")?.Value);
                }

                //smtpHost = "smtp.gmail.com";
                //port = 587;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = _subject;

                //
                var builder = new BodyBuilder
                {
                    HtmlBody = _htmlBody
                };

                if (attachments != null)
                {
                    foreach (var file in attachments)
                    {
                        if (file.Length > 0)
                        {
                            using var ms = new MemoryStream();
                            await file.CopyToAsync(ms);
                            ms.Position = 0;
                            builder.Attachments.Add(file.FileName, ms.ToArray(), ContentType.Parse(file.ContentType));
                        }
                    }
                }

                message.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                // Kết nối
                await smtp.ConnectAsync(smtpHost, port, SecureSocketOptions.StartTls).ConfigureAwait(false);
                // Nếu server yêu cầu auth
                if (!string.IsNullOrWhiteSpace(fromEmail))
                {
                    await smtp.AuthenticateAsync(fromEmail, password).ConfigureAwait(false);
                }
                await smtp.SendAsync(message).ConfigureAwait(false);
                await smtp.DisconnectAsync(true).ConfigureAwait(false);
                return new()
                {
                    IsSuccessed = true,
                    ErrMsg = ""
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSuccessed = false,
                    ErrMsg = ex.Message
                };
            }
        }

    }
}
