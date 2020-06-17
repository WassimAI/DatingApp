
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //validate request
            //We did not use modelstate since we are using the APIController atrribute, otherwise we would have to

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Username)) return BadRequest("Username aready exists");

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username,
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);//This is the status code to createdAtRoute
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userforLoginDto)
        {
            //Check if we have this user
            var userFromRepo = await _repo.Login(userforLoginDto.Username.ToLower(), userforLoginDto.Password);

            if (userFromRepo == null) return Unauthorized();

            //Token Building Token
            var claims = new[]
            {
                //One claim is ID
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                //The other claim is username
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            //Get our key stored in app settings and encode it
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            //Creating signing in creds from the key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //We create a token descriptor containing the claims, expiry date and creds created above
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject= new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            //Prep a tokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();

            //Give it the descriptor (with all data) and creating the token!
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token= tokenHandler.WriteToken(token)
            });
        }
    }
}