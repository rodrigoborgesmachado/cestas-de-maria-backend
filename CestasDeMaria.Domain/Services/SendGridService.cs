using CestasDeMaria.Domain.Interfaces.Services;
using CestasDeMaria.Domain.ModelClasses;
using Microsoft.Extensions.Options;
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
    }
}
