using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Actividad2RolesDeUsuario.Models;

namespace Actividad1.ControlDeCuentasDeUsuario.Repositories
{
	public class Repository<T> where T : class
	{
        public bdescuelaContext Context { get; set; }

        public Repository(bdescuelaContext context)
        {
            Context = context;
        }
        public virtual IEnumerable<T> GetAll()
        {
            return Context.Set<T>();
        }

        public virtual T GetById(object id)
        {
            return Context.Find<T>(id);
        }
       
        public virtual void Insert(T entidad)
        {
            if (Validate(entidad))
            {
                Context.Add(entidad);
                Save();
            }
        }

        public virtual void Update(T entidad)
        {
            if (Validate(entidad))
            {
                Context.Update(entidad);
                Save();
            }
        }

        public virtual void Delete(T entidad)
        {

            Context.Remove(entidad);
            Save();

        }

        public virtual void Save()
        {
            Context.SaveChanges();
        }

        public virtual bool Validate(T entidad)
        {
            return true;
        }

    }
}

