using ApiWebAutores.DTOs;
using ApiWebAutores.Entidades;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiWebAutores.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ComentarioController : ControllerBase
    {
        private readonly ApplicationDbContex contex;
        private readonly ILogger<AutoresController> logger;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentarioController(ApplicationDbContex contex, ILogger<AutoresController> logger,
            IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.contex = contex;
            this.logger = logger;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> get(int libroId)
        {
            List<Comentario> comentarios = await contex.Comentarios.Where(comment => comment.LibroId == libroId).ToListAsync();

            return mapper.Map<List<ComentarioDTO>>(comentarios); 
        }

        [HttpPost]
       
        public async Task<ActionResult> Post(int libroId, ComentarioCreateDTO comentarioDTO)
        {
            var emailClame = HttpContext.User.Claims.Where(c => c.Type == "email").FirstOrDefault();
            var email = emailClame.Value;
            var usuario = await userManager.FindByNameAsync(email);

            var existeLibro = await contex.Libros.AnyAsync(libro => libro.Id == libroId);

            if (!existeLibro)
            {
                return NoContent();
            }
            var comentario = mapper.Map<Comentario>(comentarioDTO);
            comentario.LibroId = libroId;
            comentario.UsuarioID = usuario.Id;
            contex.Add(comentario);
            await contex.SaveChangesAsync();
            return Ok();
        }
    }
}
