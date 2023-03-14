using DemoUser.Mappers.UserMapper;
using DemoUser.Models.DTO.DTOUser;
using DemoUser.Models.Forms.FormsUser;
using DemoUser.Models.Froms.FromsUser;
using DemoUser.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Windows.Input;
using Tools;

namespace DemoUser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize("Auth")]
    public class UserController : ControllerBase
    {
        private readonly Connection _connection;
        private readonly ITokenManager _tokenManager;
        public UserController(Connection connection, ITokenManager tokenManager)
        {
            _connection = connection;
            _tokenManager = tokenManager;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            Command command = new Command("SELECT Id, Name, Email, Point, IsActive FROM [User]", false);

            try
            {
                return Ok(_connection.ExecuteReader(command, dr => dr.ToUser()));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int? id)
        {
            
            try
            {           
                UserDB? user = GetUserById(id);
                if (user == null) return NotFound(new {message = "cet utilisateur n'est pas actif"});

                UserWithToken userWithToken = new UserWithToken()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Token = _tokenManager.GenerateJWTUser(user),
                };
                return Ok(userWithToken);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Insert([FromBody] FormInsert form)
        {
            //form.Firstname = "tom";
            //form.Lastname = "tom";
            //form.Email = "tom@mail.com";
            //form.Passwd = "Test1234=";
        
            form.Password =  BCrypt.Net.BCrypt.HashPassword(form.Password);

            Command command = new Command("INSERT INTO [User] ([Name],[Email],[Password],[Point],[IsActive]) OUTPUT inserted.id VALUES(@Name, @Email, @Password,@Point, @IsActive)", false);
            command.AddParameter("Name", form.Name);
            command.AddParameter("Email", form.Email);
            command.AddParameter("Password", form.Password);
            command.AddParameter("Point", form.Point);
            command.AddParameter("IsActive", form.IsActive);
            try
            {
                int? id = (int?)_connection.ExecuteScalar(command);
                if (id is null)
                {
                    return BadRequest(new { message = "erreur lors de l'insertion de l'utilisateur" });
                }
                UserDB? user = GetUserById(id);

                UserWithToken userWithToken = new UserWithToken()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Token = _tokenManager.GenerateJWTUser(user)

                };
                return Ok(userWithToken);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, FormUpdate form)
        {
            Command command = new Command("UPDATE [User] SET [Name] = @Name , [Email] = @Email WHERE Id = @Id", false);
            command.AddParameter("Id", id);
            command.AddParameter("Name",form.Name);
            command.AddParameter("Email",form.Email);

            try
            {
                _connection.ExecuteNonQuery(command);
                UserDB user= GetUserById(id);

                UserWithToken userWithToken = new UserWithToken()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Token = _tokenManager.GenerateJWTUser(user)

                };
                return Ok(userWithToken);
               
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            //Command command = new Command("DELETE FROM [User] WHERE Id = @Id ",false);
            Command command = new Command("Update [User] Set IsActive = 0 WHERE Id = @Id ",false);

            command.AddParameter("Id", id);
            try
            {
                return Ok(_connection.ExecuteNonQuery(command));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginForm form)
        {

            //form.Email = "tom@mail.com";
            //form.Passwd = "Test1234=";

            Command command = new Command("SELECT [User].Password FROM [User] WHERE Email = @Email ", false);
            command.AddParameter("Email", form.Email);
            try
            {
                string? passwordHash = (string?)_connection.ExecuteScalar(command);
                if(passwordHash is null)
                {
                    return NotFound(new { message =" email invalide" });
                }

                bool verified = BCrypt.Net.BCrypt.Verify(form.Password, passwordHash);
                if (!verified) return NotFound(new { message = "Le mot de passe est invalide" });

                Command command1 = new Command("SELECT [User].Id, [User].Name,[User].Email,[User].Point, [User].IsActive FROM [User] WHERE [User].Email = @Email AND [User].IsActive = 1", false);
                command1.AddParameter("Email", form.Email);

                try
                {
                    UserDB? user = _connection.ExecuteReader(command1, dr => dr.ToUser()).SingleOrDefault();

                    if (user is null)
                    {
                        return NotFound(new { message = "Votre compte n'est plus actif " });
                    }

                    UserWithToken userWithToken = new UserWithToken()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Point = user.Point,
                        Token = _tokenManager.GenerateJWTUser(user)            
                       
                    };
                    return Ok(userWithToken);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }


            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpPost("Token")]
        public IActionResult GetUserByToken([FromBody] UserToken form)
        {
            if (!ModelState.IsValid) return BadRequest(new { Message = "ModelState insert est invalide" });

            try
            {
                //var handler = new JwtSecurityTokenHandler();
                //var tokenData = handler.ReadJwtToken(form.TokenString);
                JwtSecurityToken token = new JwtSecurityToken(jwtEncodedString: form.TokenString);
                
                int id = int.Parse(token.Claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid").Value);

                UserDB? user = GetUserById(id);

                UserWithToken userWithToken = new UserWithToken()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Token = _tokenManager.GenerateJWTUser(user)

                };
                return Ok(userWithToken);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "l'insertion a échoué, contactez l'admin", ErrorMessage = ex.Message });
            }
        }


        private UserDB? GetUserById(int? id)
        {
            Command command = new Command("SELECT [User].Id, [User].Name, [User].Email, [User].Point,[user].IsActive FROM [User] WHERE [User].Id = @Id AND [User].IsActive = 1", false);
            command.AddParameter("Id", id);

            return _connection.ExecuteReader(command, dr => dr.ToUser()).SingleOrDefault();
        }

       
    }
}
