using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWebAutores.Entidades
{
    public class AutorLibro
    {
        public int AutorId { get; set; }
        public int LibroId { get; set; }
        public int Orden { get; set; }
        public Autor Autor { get; set; }
        public Libro Libro { get; set; }
    }
}
