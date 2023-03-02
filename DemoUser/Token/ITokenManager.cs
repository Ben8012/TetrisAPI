using DemoUser.Models.DTO.DTOUser;

namespace DemoUser.Token
{
    public interface ITokenManager
    {
        string GenerateJWTUser(UserDB client);
    }
}
