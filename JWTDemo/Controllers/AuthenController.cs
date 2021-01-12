using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using JWTDemo.Models;
using JWTDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JWTDemo.Controllers
{   
   
    [Route("[controller]")]
    [ApiController]

    public class AuthenController : ControllerBase
    {
        private IService _service;
        private IMapper _imapper;
        private readonly AppSettings _appSettings;

        public AuthenController(IService service, IMapper imapper, IOptions<AppSettings> appSettings)
        {
            _service = service;
            _imapper = imapper;
            _appSettings = appSettings.Value; 
        }
       
        [AllowAnonymous]
        [HttpGet("authenticate")]
        public IActionResult Authenticate ([FromBody]AuthenModel model )
        {
            //var user = _imapper.Map<User>(model);
            var user = _service.Authenticate(model.UserName, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,  user.Id.ToString()),
                  //  new Claim("dang",  "vu")

                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return user
            return Ok(new
            {
                Id = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisModel model)
        {
            var user = _imapper.Map<User>(model);

            try
            {
                // create user
                _service.Create(user, model.Password);
                return Ok();
            }
            catch 
            {
                // return error message if there was an exception
                return BadRequest(new ArgumentException("failed"));
            }
        }
       [AllowAnonymous] 
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var users = _service.GetAll();
            var model = _imapper.Map<IList<UserModel>>(users);
            return Ok(model);
        }

            
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _service.GetById(id);
            var model = _imapper.Map<UserModel>(user);
            return Ok(model);
        }
        
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateModel model)
        {
            // map model 
            var user = _imapper.Map<User>(model);
            user.Id = id;

            try
            {
                // update user 
                _service.Update(user, model.Password);
                return Ok();
            }
            catch 
            {
                // return error message if there was an exception
                return BadRequest("loi");
            }

        }

      
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return Ok();
        }
    }
}
