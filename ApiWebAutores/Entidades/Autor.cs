using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWebAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} no debe ser mayor {1} caracteres")] 
        public string Nombre { get; set; }

        public List<AutorLibro> libros { get; set; }
        /*
        [Range(18, 50)]
        [NotMapped]
        public int Edad { get; set; }
        */
        //public List<Libro> Libros { get; set; }
    }
}
