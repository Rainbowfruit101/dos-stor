using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using DbContexts;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using ViewModels;

namespace Services
{
    public class UserService
    {
        private readonly DocumentStorageContext _dbContext;
        private readonly string _passwordHashSalt;

        public UserService(DocumentStorageContext dbContext, string passwordHashSalt)
        {
            _dbContext = dbContext;
            _passwordHashSalt = passwordHashSalt;
        }
        
        public ClaimsIdentity GetIdentity(UserAuth auth)
        {
            var passwordHash = HashPassword(auth.Password);
            var user = _dbContext.Users.Include(u => u.Role).FirstOrDefault(u => u.Username == auth.Username && u.Password == passwordHash);
            if (user == null) 
                return null;
            
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Id.ToString())
            };
            return new ClaimsIdentity(claims, "Token", 
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType
            );
        }
        
        public string HashPassword(string password)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(_passwordHashSalt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8)
            );
        }
    }
}