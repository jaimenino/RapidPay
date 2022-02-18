using RapidPay.Application.Model;
using RapidPay.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

//Code based on https://jasonwatmore.com/post/2021/12/20/net-6-basic-authentication-tutorial-with-example-api#user-service-cs

namespace RapidPay.Application.Services
{
    public static class TokenAttributes
    {
        public const string Id = "id";
        public const string SaltCode = "salt";
        public const string ExpirationTime = "expiration";
    }
    public interface IAuthenticationService
    {
        AuthenticatedUser Authenticate(string username, string password);
        bool ValidateTokenProperties(string encryptedToken);
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly RapidPayEntities _dbContext = new();
        /// <summary>
        /// Performs the authentication and returns the token
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <returns>Authentication token</returns>
        public AuthenticatedUser Authenticate(string username, string password)
        {
            AuthenticatedUser? user = null;
            var hashPassword = Encryptor.HashPassword(password);
            var userDb = _dbContext.Users.Where(x => x.Username == username && x.Password == hashPassword)?.FirstOrDefault();
            if (userDb != null)
            {
                int expirationTime = 24 * 60;
                DateTime creationDate = DateTime.Now;

                string plainToken = string.Format("{0}={1};{2}={3};{4}={5}", 
                    TokenAttributes.Id, userDb.Id,
                    TokenAttributes.SaltCode, Guid.NewGuid(), 
                    TokenAttributes.ExpirationTime, creationDate.AddMinutes(expirationTime));
                string encryptedToken = Encryptor.Encrypt(plainToken);
                user = new AuthenticatedUser()
                {
                    Username = userDb.Username,
                    Token = encryptedToken
                };

                //Revoke all previous tokens
                InactivateUserTokens_ByUserId(userDb.Id);

                //Register new token
                var newUserToken = new UserToken()
                {
                    CreatedDate = creationDate,
                    ExpirationTime = expirationTime,
                    IsActive = true,
                    Token = encryptedToken,
                    UserId = userDb.Id
                };

                CreateUserToken(newUserToken);
            }
            return user;
        }
        /// <summary>
        /// Validates the token properties and return if its information is authentic
        /// </summary>
        /// <param name="encryptedToken">Token to be validated</param>
        /// <returns>True if token information is valid, otherwise, false</returns>
        public bool ValidateTokenProperties(string encryptedToken)
        {
            bool isValid = false;
            try
            {
                //Validates the token properties
                var token = Encryptor.Decrypt(encryptedToken);
                var tokenProperties = token.Split(';')?.ToList()?.ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1]);

                var id = int.Parse(tokenProperties[TokenAttributes.Id]);
                var expirationDate = DateTime.Parse(tokenProperties[TokenAttributes.ExpirationTime]);
                
                if (expirationDate >= DateTime.Now)
                {
                    //Get user from db to validate other data
                    var UserTokenDb = _dbContext.UserTokens.Where(x => x.UserId == id && x.Token == encryptedToken && x.IsActive)?.FirstOrDefault();
                    if (UserTokenDb != null)
                    {
                        isValid = true;
                    }
                }
            }
            catch
            {
            }
            return isValid;
        }
        /// <summary>
        /// Inactivates all the active user tokens
        /// </summary>
        /// <param name="userId">Unique user id</param>
        private void InactivateUserTokens_ByUserId(int userId)
        {
            var userTokens = _dbContext.UserTokens.Where(x => x.UserId == userId && x.IsActive)?.ToList();
            userTokens?.ForEach(x => x.IsActive = false);

            userTokens?.ForEach(x => _dbContext.Entry(x).State = EntityState.Modified);

            _dbContext.SaveChanges();
        }
        /// <summary>
        /// Inserts a new token information in the database
        /// </summary>
        /// <param name="userToken">Object with all the token information</param>
        public void CreateUserToken(UserToken userToken)
        {
            _dbContext.UserTokens.Add(userToken);
            _dbContext.SaveChanges();
        }
    }
}
