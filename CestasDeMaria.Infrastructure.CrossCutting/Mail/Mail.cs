using System.Reflection;
using static CestasDeMaria.Infrastructure.CrossCutting.Enums.Enums;

namespace CestasDeMaria.Infrastructure.CrossCutting.Mail
{
    public static class Mail
    {
        private static string GetTemplate(string assembly)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(assembly);
            var sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }

        public static string PrepareTemplate(string source, string[] values = null)
        {
            string result = null;
            result = GetTemplate(source);

            for (int i = 0; i < values.Length; i++)
            {
                var toReplace = "{" + Convert.ToString(i) + "}";

                result = result.Replace($"{toReplace}", values[i]);
            }

            return result;
        }

        public static string PrepareTemplate(EmailType type, string[] values = null)
        {
            string template = string.Empty;

            switch (type)
            {
                case EmailType.RecoveryPassword:
                    template = "recovery_password_template";
                    break;
                case EmailType.Wellcome:
                    template = "wellcome_template";
                    break;
                default:
                    throw new Exception("Type not found");
            }

            string htmlTemplate = GetTemplate(string.Concat("CestasDeMaria.Infrastructure.CrossCutting.Template.", template, ".html"));

            int i = 0;
            foreach (var item in values)
            {
                htmlTemplate = htmlTemplate.Replace("{" + i + "}", item);
                i++;
            }

            return htmlTemplate;
        }
    }
}
