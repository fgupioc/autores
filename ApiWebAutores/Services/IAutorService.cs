using ApiWebAutores.Entidades;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWebAutores.Services
{

    public interface IAutorService
    {
        public Task<ActionResult<Autor>> get(int id);
    }
}
