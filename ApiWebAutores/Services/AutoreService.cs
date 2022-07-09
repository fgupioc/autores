using ApiWebAutores.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWebAutores.Services
{
    public class AutorService : IAutorService
    {

        private readonly ApplicationDbContex contex;

        public AutorService(ApplicationDbContex contex)
        {
            this.contex = contex;
        }
        public async Task<ActionResult<Autor>> get(int id)
        {
            if (id == 0)
            {
                throw new Exception("El id del autor no es valido");
            }

            Autor autor = await contex.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
            {
                throw new Exception("El id del autor no es valido"); 
            }

            return autor;
        }

        
    }
}
