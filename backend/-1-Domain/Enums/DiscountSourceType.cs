namespace ERP.Domain.Enums
{
    /// <summary>
    /// Fonte de desconto para empréstimos e adiantamentos
    /// Códigos curtos (max 10 chars) para armazenamento no banco
    /// </summary>
    public static class DiscountSourceType
    {
        public const string All = "TODOS";                // Todos
        public const string Salary = "SALARIO";           // Salário
        public const string ThirteenthSalary = "13SAL";   // Décimo Terceiro
        public const string Vacation = "FERIAS";          // Férias
        public const string Annual = "ANUAL";             // Anual (não usado na UI)
        public const string Bonus = "BONUS";              // Bônus (não usado na UI)
        public const string Commission = "COMISSAO";      // Comissão (não usado na UI)

        /// <summary>
        /// Retorna a descrição amigável do código
        /// </summary>
        public static string GetDescription(string code)
        {
            return code switch
            {
                All => "Todos",
                Salary => "Salário",
                ThirteenthSalary => "Décimo Terceiro",
                Vacation => "Férias",
                Annual => "Anual",
                Bonus => "Bônus",
                Commission => "Comissão",
                _ => code
            };
        }

        /// <summary>
        /// Retorna todos os códigos válidos
        /// </summary>
        public static Dictionary<string, string> GetAll()
        {
            return new Dictionary<string, string>
            {
                { All, "Todos" },
                { Salary, "Salário" },
                { ThirteenthSalary, "Décimo Terceiro" },
                { Vacation, "Férias" },
                { Annual, "Anual" },
                { Bonus, "Bônus" },
                { Commission, "Comissão" }
            };
        }

        /// <summary>
        /// Converte descrição antiga para código novo
        /// </summary>
        public static string FromLegacyDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return All; // Default

            var normalized = description.Trim().ToLower();
            
            if (normalized.Contains("todos") || normalized.Contains("tudo"))
                return All;
            if (normalized.Contains("salário") || normalized.Contains("salario"))
                return Salary;
            if (normalized.Contains("décimo") || normalized.Contains("decimo") || normalized.Contains("13"))
                return ThirteenthSalary;
            if (normalized.Contains("férias") || normalized.Contains("ferias"))
                return Vacation;
            if (normalized.Contains("anual"))
                return Annual;
            if (normalized.Contains("bônus") || normalized.Contains("bonus"))
                return Bonus;
            if (normalized.Contains("comissão") || normalized.Contains("comissao"))
                return Commission;

            // Se não encontrar, retorna o próprio valor (truncado se necessário)
            return description.Length > 10 ? description.Substring(0, 10) : description;
        }
    }
}
