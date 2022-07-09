using ApiWebAutores.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiWebAutores.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentaController: ControllerBase
    {
        private readonly ApplicationDbContex contex;
        private readonly ILogger<CuentaController> logger;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;

        public CuentaController(ApplicationDbContex contex, ILogger<CuentaController> logger, IMapper mapper, 
            UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager)
        {
            this.contex = contex;
            this.logger = logger;
            this.mapper = mapper;
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<RespuestaAutenticacion>> Post(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser { UserName = credencialesUsuario.Username, Email = credencialesUsuario.Username };
            var res = await userManager.CreateAsync(usuario, credencialesUsuario.Password);

            if (res.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            } else
            {
                return BadRequest(res.Errors);
            }


        }

        [HttpPost]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {
            var res = await signInManager.PasswordSignInAsync(credencialesUsuario.Username, credencialesUsuario.Password, isPersistent: false,
                lockoutOnFailure: false);

            if (res.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            } else
            {
                return BadRequest("Loggin incorrecto...");
            }
        }

        private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", credencialesUsuario.Username),
                new Claim("nombre", "franz")
            };

            var usuario = await userManager.FindByNameAsync(credencialesUsuario.Username);
            var clamesDB = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(clamesDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llaveJwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMonths(1);

            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds);

            return new RespuestaAutenticacion() {
                Token = new JwtSecurityTokenHandler().WriteToken(token), 
                Expiracion = expiration
            };
        }

        [HttpGet("renovar-token")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacion>> RenovarToken()
        {
          
            var emailClame = HttpContext.User.Claims.Where(c => c.Type == "email").FirstOrDefault();
            var email = emailClame.Value;
            var usuario = await userManager.FindByNameAsync(email);

            var credenciales = new CredencialesUsuario() { 
                Username = email
            };

            return await ConstruirToken(credenciales);
        }

        [HttpPost("HacerAdmin")]
        public async Task<ActionResult> HacerAdmin(CredencialesUsuario credencialesUsuario)
        {
            var usuario = await userManager.FindByNameAsync(credencialesUsuario.Username);
            await userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }

        [HttpPost("RemoverAdmin")]
        public async Task<ActionResult> RemoverAdmin(CredencialesUsuario credencialesUsuario)
        {
            var usuario = await userManager.FindByNameAsync(credencialesUsuario.Username);
            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }


    }
}
