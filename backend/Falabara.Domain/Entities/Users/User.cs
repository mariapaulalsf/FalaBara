using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using System.Linq;

namespace Falabara.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string Cpf { get; private set; }
        public UserType Type { get; private set; }
        public bool Active { get; private set; }
        public string? Department { get; private set; }
        public string? FoneNumber { get; private set; }

        // Este construtor vazio é para o Entity Framework não reclamar
        protected User() 
        {
            Name = null!;
            Email = null!;
            PasswordHash = null!;
            Cpf = null!;
        }

        public User(string name, string email, string password, string cpf, UserType type, string? department = null, string? foneNumber = null)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("O nome é obrigatório.");
            if (string.IsNullOrEmpty(email)) throw new Exception("O email é obrigatório.");
            if (string.IsNullOrEmpty(cpf)) throw new Exception("O CPF é obrigatório.");
            
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                throw new Exception("A senha deve ter pelo menos 8 caracteres.");

            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            Cpf = cpf;
            Type = type;
            Department = department;
            FoneNumber = foneNumber;
            Active = true;

            SetPassword(password);
        }
        public void Update(string name, string? department, string? foneNumber)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("O nome é obrigatório.");
            
            Name = name;
            Department = department;
            FoneNumber = foneNumber;
        }
        public void SetPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            PasswordHash = $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        public bool ValidatePassword(string password)
        {
            var parts = PasswordHash.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hashOriginal = parts[1];

            string hashNovo = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashOriginal == hashNovo;
        }
    }
}