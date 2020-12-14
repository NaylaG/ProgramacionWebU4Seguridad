using Actividad1.ControlDeCuentasDeUsuario.Repositories;
using Actividad2RolesDeUsuario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actividad2RolesDeUsuario.Repositories
{
    public class MaestroRepository: Repository<Maestro>
    {
        public MaestroRepository(bdescuelaContext ctx) : base(ctx)
        {

        }
        public virtual Maestro GetMaestroByUsername(string username)
        {
            return Context.Maestro.FirstOrDefault(x => x.Username == username);
        }

        public virtual Maestro GetMaestroByClave(int clave)
        {
            return Context.Maestro.FirstOrDefault(x => x.Clave == clave);
        }


    }
}
