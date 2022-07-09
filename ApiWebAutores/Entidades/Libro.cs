using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWebAutores.Entidades
{
    public class Libro
    {
        public int Id { get; set; } 
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }
        //public int AutorId { get; set; }
        //public Autor Autor { get; set; }
        public List<Comentario> Comentarios { get; set; }
        public List<AutorLibro> Autores { get; set; }
    }
}
