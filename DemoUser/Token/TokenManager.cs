using DemoUser.Models.DTO.DTOUser;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DemoUser.Token
{
    public class TokenManager : ITokenManager
    {

        public static string secret = "OfOuYD2nzPUfL/WLz8JYDJIlhmVA8IbuO2o1vWzY8UOTG/gaVOaNNBar7hdX59USWfK7AzElt2cU+3JSNCrGRWOe/Vj169O1yRbMskpf1xAoDDSneLhmfYMQQRD+1WT66REh55hpdKsJuoFivlsIQtwN9Aq39H7ATI791QNI7RY=";
        //public static string myIssuer = "https://localhost:7022/"; // qui genere le token
        //public static string myAudience = "http://localhost:4200/"; // qui utilise le token


        public string GenerateJWTUser(UserDB client)
        {
            if (client.Email is null)
                throw new ArgumentNullException("L'email n'existe pas !");

            //Création des crédentials
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            //Création de l'objet contenant les informations à stocker dans le token
            Claim[] myClaims = new[]
            {
                new Claim(ClaimTypes.Email, client.Email),
                new Claim(ClaimTypes.Sid, client.Id.ToString())

            };

            //Génération du token => Nuget : System.IdentityModel.Tokens.Jwt
            JwtSecurityToken tokenClient = new JwtSecurityToken(
                claims: myClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                //issuer: myIssuer,
                //audience: myAudience
                );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenClient);
        }


    }
}
