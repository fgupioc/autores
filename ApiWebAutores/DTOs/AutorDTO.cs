using System; 
using System.ComponentModel.DataAnnotations; 

namespace ApiWebAutores.DTOs
{
    public class AutorDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} no debe ser mayor {1} caracteres")]
        public string Nombre { get; set; }
    }
}
