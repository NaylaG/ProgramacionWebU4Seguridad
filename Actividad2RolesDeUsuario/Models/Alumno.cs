using System;
using System.Collections.Generic;

namespace Actividad2RolesDeUsuario.Models
{
    public partial class Alumno
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NumControl { get; set; }
        public string ClaveMaestro { get; set; }
    }
}
