using Actividad1.ControlDeCuentasDeUsuario.Models;
using Actividad1.ControlDeCuentasDeUsuario.Models.ViewModels;
using Actividad1.ControlDeCuentasDeUsuario.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Actividad1.ControlDeCuentasDeUsuario.Controllers
{
    public class HomeController : Controller
    {
        bdusuariosContext context;
        public IWebHostEnvironment Environment { get; set; }
        public HomeController(bdusuariosContext ctx, IWebHostEnvironment env)
        {
            context = ctx;
            Environment = env;
        }
        [Authorize(Roles = "Usuario")]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Registrar(Usuario nuevo)
        {
            try
            {
                UsuarioRepository repos = new UsuarioRepository(context);
                
                nuevo.Password= HashingHelper.GetHash(nuevo.Password);
                nuevo.Codigo = GeneraCodigoHelper.GetCodigo();
                repos.Insert(nuevo);

                //para confirmar correo
                MailMessage message = new MailMessage();
                message.From = new MailAddress("sistemascomputacionales7g@gmail.com", "Cuenta automatizada de Biblioteca Virtual");
                message.Bcc.Add(nuevo.Email);
                message.Subject = "Activar Cuenta";
                string text = System.IO.File.ReadAllText(Environment.WebRootPath + "/Verificar.html");
                //message.Body = text.Replace("{##correo##}", nuevo.Email);
                message.Body = text.Replace("{##codigo##}", nuevo.Codigo);
                message.IsBodyHtml = true;

                
                SmtpClient client = new SmtpClient("smtp.gmail.com",587);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("sistemascomputacionales7g@gmail.com", "sistemas7g");
                client.Send(message);                

                return RedirectToAction("ActivarCuenta");
            }
            catch (Exception error)
            {
                Repository<Usuario> repos = new Repository<Usuario>(context);
                ModelState.AddModelError("", error.Message);
                return View(nuevo);
            }
        }
        
        [AllowAnonymous]
        public IActionResult ActivarCuenta()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ActivarCuenta(string email, string codigo)
        {
           UsuarioRepository repos = new UsuarioRepository(context);
            
            var usuario = repos.GetByEmail(email);           
            if(usuario.Codigo == codigo )
            {
                usuario.Activo = 1;
                repos.Update(usuario);
            }
           
            return RedirectToAction("IniciarSesion");
        }


        [AllowAnonymous]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IniciarSesion(Usuario usuario, bool mantenerSesion)
        {
            try
            {
                UsuarioRepository repos = new UsuarioRepository(context);
               
                var user = repos.GetByEmail(usuario.Email);
                
                if (user.Email == usuario.Email && user.Password == HashingHelper.GetHash(usuario.Password) && user.Activo == 1)
                {

                    List<Claim> informacion = new List<Claim>();
                    informacion.Add(new Claim(ClaimTypes.Email, user.Email));
                    informacion.Add(new Claim(ClaimTypes.Role, "Usuario"));//tiene que ir el rol para poder autenticar
                    informacion.Add(new Claim("Id", user.Id.ToString()));
                    informacion.Add(new Claim("Correo", user.Email));
                    //if (mantenerSesion == true)
                    //{ informacion.Add(new Claim(ClaimTypes.IsPersistent, "true")); }
                    //else
                    //{ informacion.Add(new Claim(ClaimTypes.Expiration, "20")); }
                    informacion.Add(new Claim("Nombre",user.Username));


                    //guardar los claimns para saber los datos

                    var claimidentity = new ClaimsIdentity(informacion, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimprincipal = new ClaimsPrincipal(claimidentity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimprincipal, new AuthenticationProperties { IsPersistent = mantenerSesion });
                    return RedirectToAction("Index");

                }else if(user.Activo==0)
                {
                    return RedirectToAction("ActivarCuenta");
                }
                else
                {
                    ModelState.AddModelError("", "El usuario o la contraseña son incorrectas");
                    return View();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("IniciarSesion");
        }

        [AllowAnonymous]
        public IActionResult Denegado()
        {
            return View();
        }


        [AllowAnonymous]
        public IActionResult RecuperarContraseña()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult RecuperarContraseña(Usuario u)
        {
            try
            {
                UsuarioRepository repos = new UsuarioRepository(context);
                var user = repos.GetByEmail(u.Email);


                MailMessage message = new MailMessage();
                message.From = new MailAddress("sistemascomputacionales7g@gmail.com", "Cuenta automatizada de sistemas");
                message.Bcc.Add(user.Email);
                message.Subject = "Activar Cuenta";
                string text = System.IO.File.ReadAllText(Environment.WebRootPath + "/CorreoRecuperarContraseña.html");
                message.Body = text.Replace("{##codigo##}", user.Codigo);
                //message.Body = text.Replace("{##correo##}", user.Email);
            
                message.IsBodyHtml = true;


                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("sistemascomputacionales7g@gmail.com", "sistemas7g");
                client.Send(message);
                

                    return RedirectToAction("NuevaContraseña");
            }
            catch (Exception error)
            {
                Repository<Usuario> repos = new Repository<Usuario>(context);
                ModelState.AddModelError("", error.Message);
                return View(User);
            }
        }

        [AllowAnonymous]
        public IActionResult NuevaContraseña()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult NuevaContraseña(UsuarioViewModel vm)
        {
            try
            {
                UsuarioRepository repos = new UsuarioRepository(context);

                var usuario = repos.GetByEmail(vm.Usuario.Email);

                
                if (usuario.Codigo != vm.Usuario.Codigo)
                {
                    //codigo incorrecto
                }

                if (vm.NuevaContraseña != vm.ConfirmarContraseña)
                {
                    //the passwords are differnt
                }
                if (usuario.Password == HashingHelper.GetHash(vm.NuevaContraseña))
                {
                    //the password can't be te same
                }

                usuario.Password = HashingHelper.GetHash(vm.NuevaContraseña);
                repos.Update(usuario);


                return RedirectToAction("IniciarSesion");

            }
            catch (Exception error)
            {
                Repository<Usuario> repos = new Repository<Usuario>(context);
                ModelState.AddModelError("", error.Message);
                return View(vm);
            }
        }

        [Authorize(Roles = "Usuario")]
        public IActionResult CambiarContraseña()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Usuario")]
        public IActionResult CambiarContraseña( UsuarioViewModel vm)
        {
            try
            {
                UsuarioRepository repos = new UsuarioRepository(context);

                var usuario = repos.GetById(vm.Usuario.Id);

                if (usuario.Password != HashingHelper.GetHash(vm.Usuario.Password))
                {
                    //se dice que la contrasena esta incorrecta
                }


                if (vm.NuevaContraseña != vm.ConfirmarContraseña)
                {
                    //the passwords are differnt
                }
                //if (usuario.Password == HashingHelper.GetHash(vm.NuevaContraseña))
                //{
                //    //the password can't be te same
                //}

                usuario.Password = HashingHelper.GetHash(vm.NuevaContraseña);
                repos.Update(usuario);


                return RedirectToAction("Index");

            }
            catch (Exception error)
            {
                ModelState.AddModelError("", error.Message);
                return View(vm);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Usuario")]
        public IActionResult EliminarCuenta(string email)
        {
            UsuarioRepository repos = new UsuarioRepository(context);
            var user = repos.GetByEmail(email);
            if(user!=null)
            {
                repos.Delete(user);
            }
            return RedirectToAction("IniciarSesion");
        }


    }
}
