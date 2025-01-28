using System.Text.RegularExpressions;

namespace CestasDeMaria.Application.Helpers
{
    public static class IncludesMethods
    {
        /// <summary>
        /// Return allow includes
        /// </summary>
        /// <param name="include"></param>
        /// <param name="allowInclude"></param>
        /// <returns>string[]</returns>
        public static string[] GetIncludes(string include, string[] allowInclude)
        {
            List<string> result = new List<string>();

            if (!string.IsNullOrEmpty(include))
            {
                var tmp = include.Split(',');

                foreach (var toInclude in tmp)
                {
                    if (allowInclude.Contains(toInclude))
                    {
                        result.Add(toInclude);
                    }
                }
            }

            return result.ToArray<string>();
        }

        /// <summary>
        /// Return clean string
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        public static string CleanString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                str = "";
            }

            string pattern = @"[^0-9a-zA-Z çÇãõÃÕáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ]";
            return Regex.Replace(str, pattern, string.Empty).Trim().Replace("  ", " ");
        }

        /// <summary>
        /// Method that verify if the CPF is ok
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static bool VerifyCPF(string document)
        {
            if (string.IsNullOrEmpty(document))
            {
                return false;
            }

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            string cpf = document.Trim();

            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;

            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }
    }
}
