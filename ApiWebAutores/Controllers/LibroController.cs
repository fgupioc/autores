using ApiWebAutores.DTOs;
using ApiWebAutores.Entidades;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ApiWebAutores.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibroController: ControllerBase
    {
        private readonly ApplicationDbContex contex;
        private readonly ILogger<LibroController> logger;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public LibroController(ApplicationDbContex contex, ILogger<LibroController> logger,
            IMapper mapper, IConfiguration configuration)
        {
            this.contex = contex;
            this.logger = logger;
            this.mapper = mapper;
            this.configuration = configuration;
        }
         
        [HttpGet]
        public async Task<ActionResult<List<LibroDTO>>> get()
        {
            var libros =  await contex.Libros.ToListAsync();

            return mapper.Map<List<LibroDTO>>(libros);
        }

        [HttpGet("configuraciones")]
        public async Task<ActionResult<string>> configuraciones()
        {
            // configuration["apellido"]
            //return configuration["connectionStrings:defaultConnection"];
            return configuration["apellido"];
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LibroDTO>> get(int id)
        {
            Libro libro = await contex.Libros.Include(x => x.Comentarios)
                .Include(l => l.Autores)
                .ThenInclude(al => al.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (libro == null)
            {
                return BadRequest("no se encontro el libro");
            }
            return mapper.Map<LibroDTO>(libro);
        }
       
        [HttpPost] 
        public async Task<ActionResult> post (LibroCreateDTO libroDTO)
        {

            if (libroDTO.AutoresIds == null)
            {
                return BadRequest("No se puede registrar un libro sin autores");
            }

                var autoresIds = await contex.Autores.Where(x => libroDTO.AutoresIds.Contains(x.Id)).Select( x => x.Id).ToListAsync();

            if(libroDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No equiste uno de los autores");
            }

            Libro libro = mapper.Map<Libro>(libroDTO);

            var list = new List<AutorLibro>();
            if (libroDTO.AutoresIds.Count > 0)
            {
                int index = 0;
                foreach (var autorId in libroDTO.AutoresIds)
                {
                    list.Add(new AutorLibro() { AutorId = autorId, Orden = index });
                    index++;
                }
                libro.Autores = list;
            }

            contex.Libros.Add(libro);
            await contex.SaveChangesAsync();
            return Ok();
        }


    }
}
