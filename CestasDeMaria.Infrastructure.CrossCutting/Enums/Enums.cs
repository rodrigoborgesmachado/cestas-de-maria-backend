using System.ComponentModel;
using System.Reflection;

namespace CestasDeMaria.Infrastructure.CrossCutting.Enums
{
    public static class Enums
    {
        public enum Profile
        {
            Administrative,
            Client
        }

        public enum StatusMail
        {
            Sent = 0,
            Error = 1
        }

        public enum EmailType
        {
            RecoveryPassword,
            Wellcome,
        }

        public enum DeliveryStatus
        {
            SOLICITAR = 1,
            SOLICITADO = 2,
            ENTREGUE = 3,
            FALTOU = 4
        }

        public enum FamilyStatus
        {
            CORTADO = 1,
            EMESPERA = 2,
            EMATENDIMENTO = 3,
            ELEGIVEL = 4
        }

        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            if (field == null)
                return value.ToString();

            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }

        public static int GetValue(this Enum value)
        {
            return Convert.ToInt32(value);
        }
    }
}
