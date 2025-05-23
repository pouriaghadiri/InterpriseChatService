using Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.ValueObjects
{
    public class HashedPassword : ValueObject
    {
        public string Hash { get; private set; }
        public string Salt { get; private set; }


        private const int iterations = 10000;
        private const int keySize = 32; // 256-bit
         
        private HashedPassword(string plain)
        { 
            Salt = GenerateSalt();
            Hash = GenerateHashPassword(plain, Salt);
        }
        public static ResultDTO<HashedPassword> Create(string plain)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(plain))
                errors.Add("Password is required.");
            if (plain?.Length < 8)
                errors.Add("Password must be at least 8 charachters.");

            if(errors.Count > 0)
                return ResultDTO<HashedPassword>.Failure("Invalid Password", errors, "Please fix the password input.");

            return ResultDTO<HashedPassword>.Success("Valid Password", new HashedPassword(plain), "Password created successfully");
        }
        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }
        private string GenerateHashPassword(string plain, string salt)
        {
            using var deriveByte = new Rfc2898DeriveBytes(plain, Convert.FromBase64String(salt),iterations, HashAlgorithmName.SHA256);
            return Convert.ToBase64String(deriveByte.GetBytes(keySize));
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Hash;
            yield return Salt;
        }
        public bool Verify(string plain)
        {
            string comparedHash = GenerateHashPassword(plain, Salt);
            return Hash == comparedHash;
        }
        public static HashedPassword CreateFromPlain(string plain)
        {
            return new HashedPassword(plain);
        }
    }
}
