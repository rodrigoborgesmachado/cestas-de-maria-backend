using CestasDeMaria.Domain.Interfaces.Services;
using CestasDeMaria.Domain.ModelClasses;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;
using Logger = CestasDeMaria.Domain.Interfaces.Repository.ILoggerRepository;
using Main = CestasDeMaria.Domain.Entities.Mailmessage;

namespace CestasDeMaria.Domain.Services
{
    public class SendGridService : ISendGridService
    {
        private readonly Settings _settings;
        private readonly Logger _logger;

        public SendGridService(IOptions<Settings> options, Logger logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<bool> SendMail(Main entity)
        {
            bool retorno = true;
            entity.Retries++;

            var client = new SendGridClient(_settings.SendGridApiKey);
            var from = new EmailAddress(_settings.EmailCredential);
            var subject = entity.Subject;
            var to = new EmailAddress(entity.To);
            var htmlContent = entity.Body;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, htmlContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            retorno = response.IsSuccessStatusCode;

            if (!retorno)
            {
                await _logger.InsertAsync($"Erro ao enviar email: {response.StatusCode}", entity.Adminid);
            }

            return retorno;
        }

        public async Task<bool> SendMailV2(Main entity)
        {
            bool retorno = true;
            entity.Retries++;
            var from = _settings.EmailCredential;
            var subject = entity.Subject;
            var to = entity.To;
            var htmlContent = entity.Body;

            var client = new MailjetClient(_settings.MailjetApiKey, _settings.MailjetSecretKey);

            var request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.FromEmail, from)
            .Property(Send.Subject, subject)
            .Property(Send.TextPart, htmlContent)
            .Property(Send.HtmlPart, htmlContent)
            .Property(Send.Recipients, new JArray {
                new JObject {
                    { "Email", to }
                }
            });

            var response = await client.PostAsync(request);

            retorno = response.IsSuccessStatusCode;

            if (!retorno)
            {
                await _logger.InsertAsync($"Erro ao enviar email: {response.StatusCode}", entity.Adminid);
            }

            return retorno;
        }
    }
}
