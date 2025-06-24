using System.Collections.Generic;
using System.Linq;

namespace Prueba.Models
{
    public class TrabajadorViewModel
    {
        public int Id { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Nombres { get; set; }
        public string Sexo { get; set; }
        public int? IdDepartamento { get; set; }
        public string NombreDepartamento { get; set; }
        public int? IdProvincia { get; set; }
        public string NombreProvincia { get; set; }
        public int? IdDistrito { get; set; }
        public string NombreDistrito { get; set; }

        // Método para obtener la ubicación formateada correctamente

    }
}