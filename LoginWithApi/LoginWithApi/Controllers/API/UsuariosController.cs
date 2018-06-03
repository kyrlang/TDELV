using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using LoginWithApi.Models;
using Newtonsoft.Json.Linq;

namespace LoginWithApi.Controllers
{
    public class UsuariosController : ApiController
    {
        private DBModels db = new DBModels();

        [HttpGet()]
        [Route("api/usuarios")]
        public IQueryable<Usuario> ListarUsuarios()
        {
            return db.Usuarios.OrderBy(u => u.Nome);
        }

        [HttpGet()]
        [Route("api/usuarios/{id}")]
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult ListarUsuario(int id)
        {
            try
            {
                Usuario usuario = db.Usuarios.Find(id);

                if (usuario == null)
                    return NotFound();

                return Ok<Usuario>(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut()]
        [Route("api/usuarios/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Atualizar(int id, Usuario usuario)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != usuario.Id)
                return BadRequest("O Usuário e o ID informado não são o mesmo.");

            db.Entry(usuario).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!UsuarioExists(id))
                    return NotFound();
                else
                    return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return StatusCode(HttpStatusCode.OK);
        }

        [HttpPost()]
        [Route("api/usuarios")]
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult Inserir(Usuario usuario)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                usuario.CadastroData = DateTime.Now;
                db.Usuarios.Add(usuario);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(usuario);
            // CreatedAtRoute("DefaultApi", new { id = usuario.Id }, usuario);
        }

        [HttpDelete()]
        [Route("api/usuarios/{id}")]
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult Excluir(int id)
        {
            try
            {
                Usuario usuario = db.Usuarios.Find(id);

                if (usuario == null)
                    return NotFound();

                db.Usuarios.Remove(usuario);
                db.SaveChanges();

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        //[ActionName("Login")]
        [Route("api/usuarios/login")]
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult Login([FromBody] JObject json)
        {
            try
            {
                Usuario usuario = usuario = json.ToObject<Usuario>();
                usuario = db.Usuarios
                            .Where(u => u.Email == usuario.Email && u.Senha == usuario.Senha)
                            .FirstOrDefault();

                if (usuario == null)
                    return NotFound();

                return Ok<Usuario>(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        //[ActionName("ValidaEmail")]
        [Route("api/usuarios/email")]
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult ValidaEmail([FromBody] JObject json)
        {
            try
            {
                Usuario usuario = usuario = json.ToObject<Usuario>();
                usuario = db.Usuarios
                            .Where(u => u.Email == usuario.Email)
                            .FirstOrDefault();

                if (usuario != null)
                    return Conflict();

                return Ok<Usuario>(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UsuarioExists(int id)
        {
            return db.Usuarios.Count(e => e.Id == id) > 0;
        }
    }
}