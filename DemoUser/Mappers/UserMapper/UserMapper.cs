

using DemoUser.Models.DTO.DTOUser;
using System.Data.Common;

namespace DemoUser.Mappers.UserMapper
{
    public static class UserMapper
    {
        internal static UserDB ToUser(this DbDataReader reader)
        {
            return new UserDB()
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Email = (string)reader["Email"],
                Point = (int)reader["Point"],
                IsActive = (bool)reader["IsActive"]
            };

        }

       
    }
}
