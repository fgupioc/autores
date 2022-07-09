using ApiWebAutores.DTOs;
using ApiWebAutores.Entidades;
using ApiWebAutores.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWebAutores.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AutoresController: ControllerBase
    {
        private readonly ApplicationDbContex contex;
        private readonly IAutorService autorService;
        private readonly ILogger<AutoresController> logger;
        private readonly IMapper mapper;

        public AutoresController(
            ApplicationDbContex contex, 
            IAutorService autorService, 
            ILogger<AutoresController> logger,
            IMapper mapper
            )
        {
            this.contex = contex;
            this.autorService = autorService;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet] // api/Autores
        //[HttpGet("listado")] // api/Autores/listado  -- QueryString api/Autores/listado?nombre=franz&apellidos=gupioc
       // [HttpGet("/listado")] // listado
       //[Authorize]
        public async Task<ActionResult<List<Autor>>> Get([FromQuery] string nombre, [FromQuery] string apellidos)
        {
            //return await contex.Autores.Include(x => x.Libros).ToListAsync();
            return await contex.Autores.ToListAsync();
        }

       
        [HttpGet("listado")] // api/Autores/listado  -- QueryString api/Autores/listado?nombre=franz&apellidos=gupioc
        public async Task<ActionResult<List<AutorListDTO>>> Get()
        {

            var autores = await contex.Autores.ToListAsync();

            return mapper.Map<List<AutorListDTO>>(autores);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]AutorDTO autorDTO)
        {
            var existeAutorConElNombre = await contex.Autores.AnyAsync(x => x.Nombre == autorDTO.Nombre);
            if(existeAutorConElNombre)
            {
                return BadRequest($"Ya existe el nombre del autor {autorDTO.Nombre}");
            }
            /*
            Autor autor = new Autor()
            {
                Nombre = autorDTO.Nombre
            };
            */

            Autor autor = mapper.Map<Autor>(autorDTO);
            contex.Add(autor);
            await contex.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous] // evitar la autorizacion
        public async Task<ActionResult<Autor>> get([FromRoute] int id)
        {

           return await autorService.get(id);
            /*
            if (id == null)
            {
                return BadRequest("El id del autor no es valido");
            }

            Autor autor = await contex.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
            {
                return NotFound("El id del autor no es valido");
            }

            return autor;
            */

        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<AutorListDTO>>> get([FromRoute] string nombre)
        {


            /*
            Autor autor = await contex.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));
            if (autor == null)
            {
                return NotFound("El nombre del autor no existe.");
            }
            */

            List<Autor> autores = await contex.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync(); 

            return mapper.Map<List<AutorListDTO>>(autores); ; 
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromBody]Autor autor, [FromRoute]int id)
        {
            if (autor.Id != id)
            {
                return BadRequest("El id del autor no es valido");
            }

            var exist = await contex.Autores.AnyAsync(x => x.Id == id);

            if (!exist)
            {
                return NotFound("El id del autor no es valido");
            }
            contex.Update(autor);
            await contex.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        public async Task<ActionResult> Delete(int id)
        {

            var exist = await contex.Autores.AnyAsync(x => x.Id == id);

            if (!exist)
            {
                return NotFound("El id del autor no es valido");
            }

            contex.Remove(new Autor() { Id = id });
            await contex.SaveChangesAsync();
            return Ok();
        }

        /*

        [HttpGet("primer-autor")]
        public async Task<ActionResult<Autor>> primerActor()
        {
            return await contex.Autores.Include(x => x.Libros).FirstOrDefaultAsync();
        }
       

        [HttpDelete("{id:int}/{param?}")]
        public async Task<ActionResult> eliminarDemo(int id, string param)
        {

            var exist = await contex.Autores.AnyAsync(x => x.Id == id);

            if (!exist)
            {
                return NotFound("El id del autor no es valido");
            }

            contex.Remove(new Autor() { Id = id });
            await contex.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}/{param=persona}")]
        public async Task<ActionResult> eliminarDemo2(int id, string param)
        {

            var exist = await contex.Autores.AnyAsync(x => x.Id == id);

            if (!exist)
            {
                return NotFound("El id del autor no es valido");
            }

            contex.Remove(new Autor() { Id = id });
            await contex.SaveChangesAsync();
            return Ok();
        }
        */
    }
}
