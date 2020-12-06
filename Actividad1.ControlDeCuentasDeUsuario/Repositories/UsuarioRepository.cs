using Actividad1.ControlDeCuentasDeUsuario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actividad1.ControlDeCuentasDeUsuario.Repositories
{
    public class UsuarioRepository: Repository<Usuario>
    {
        public UsuarioRepository(bdusuariosContext ctx):base(ctx)
        { }
        public Usuario GetByEmail(string email)
        {
            return Context.Usuario.FirstOrDefault(x => x.Email.ToUpper() == email.ToUpper());
        }


        public override bool Validate(Usuario entidad)
        {
            if(Context.Usuario.Any(x=>x.Email==entidad.Email && x.Id != entidad.Id))
            {
                throw new Exception("Este correo ya se encuentra registrado");
            }

            return true;
        }
    }
}
