using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWebAutores.DTOs
{
    public class LibroCreateDTO
    {
        [Required]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }
        public List<int> AutoresIds { get; set; }
    }
}
