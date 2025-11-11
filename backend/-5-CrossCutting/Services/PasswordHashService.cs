using System.Security.Cryptography;

namespace ERP.CrossCutting.Services
{
    public interface IPasswordHashService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }

    public class PasswordHashService : IPasswordHashService
    {
        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 32; // 256 bits
        private const int Iterations = 100000; // Número de iterações PBKDF2
        private const char Delimiter = ';';

        public string HashPassword(string password)
        {
            // Gera um salt aleatório
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            // Gera o hash usando PBKDF2
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] key = pbkdf2.GetBytes(KeySize);

                // Combina iterações, salt e hash em uma única string
                string hashString = string.Join(
                    Delimiter,
                    Convert.ToBase64String(salt),
                    Convert.ToBase64String(key),
                    Iterations
                );

                return hashString;
            }
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                // Extrai os componentes do hash armazenado
                string[] parts = hash.Split(Delimiter);
                
                if (parts.Length != 3)
                {
                    // Pode ser um hash BCrypt antigo, retornar false para forçar reset
                    return false;
                }

                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] storedKey = Convert.FromBase64String(parts[1]);
                int iterations = int.Parse(parts[2]);

                // Gera o hash da senha fornecida usando os mesmos parâmetros
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                {
                    byte[] computedKey = pbkdf2.GetBytes(KeySize);

                    // Compara os hashes usando comparação segura
                    return CryptographicOperations.FixedTimeEquals(computedKey, storedKey);
                }
            }
            catch
            {
                // Em caso de erro (formato inválido, etc), retorna false
                return false;
            }
        }
    }
}
