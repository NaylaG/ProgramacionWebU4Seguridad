using System;
using System.Collections.Generic;

namespace Actividad1.ControlDeCuentasDeUsuario.Models
{
    public partial class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ulong Activo { get; set; }
        public string Codigo { get; set; }
    }
}
