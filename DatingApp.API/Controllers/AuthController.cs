
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        public AuthController(IConfiguration config, DataContext context, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //validate request
            //We did not use modelstate since we are using the APIController atrribute, otherwise we would have to


            //user is desctination and userForRegisterDto is source
            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);

            if (result.Succeeded)
            {
                //we return the route + user we created
                return CreatedAtRoute("GetUser", new { Controller = "Users", id = userToCreate.Id }, userToReturn); //This is the status code to createdAtRoute
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userforLoginDto)
        {
            //Check if we have this user
            // var userFromRepo = await _repo.Login(userforLoginDto.Username.ToLower(), userforLoginDto.Password);
            var user = await _userManager.FindByNameAsync(userforLoginDto.Username);

            var results = await _signInManager.CheckPasswordSignInAsync(user, userforLoginDto.Password, false);

            if(results.Succeeded)
            {
                var appUser = _mapper.Map<UserForListDto>(user);

                return Ok(new
                {
                    token = GenerateJwtToken(user).Result,
                    user = appUser
                });
            }

            return Unauthorized();

        }

        private async Task<string> GenerateJwtToken(User user)
        {
            //Token Building Token
            var claims = new List<Claim>
            {
                    //One claim is ID
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    //The other claim is username
                    new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //Get our key stored in app settings and encode it
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            //Creating signing in creds from the key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //We create a token descriptor containing the claims, expiry date and creds created above
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            //Prep a tokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();

            //Give it the descriptor (with all data) and creating the token!
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}