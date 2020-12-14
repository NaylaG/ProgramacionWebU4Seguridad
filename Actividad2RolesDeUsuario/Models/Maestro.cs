using System;
using System.Collections.Generic;

namespace Actividad2RolesDeUsuario.Models
{
    public partial class Maestro
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
        public int Clave { get; set; }
        public ulong Activo { get; set; }
        public string Email { get; set; }
    }
}
