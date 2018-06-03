using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using LoginWithApi.Classes;
using LoginWithApi.Models.MVC;
using Newtonsoft.Json;

namespace LoginWithApi.Controllers.MVC
{
    public class UsuariosController : Controller
    {

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UsuarioLogado"] != null)
                base.OnActionExecuting(filterContext);
            else
                filterContext.Result = RedirectToAction("Index", "Home");
        }

        // GET: Usuarios
        public ActionResult Index()
        {
            return View(ListarUsuarios());
        }

        public ActionResult BoasVindas(mvcUsuario usuario)
        {
            return View(usuario);
        }

        public ActionResult FormUsuario(mvcUsuario usuario)
        {
            return View(usuario);
        }

        public ActionResult AddOrUpdate(mvcUsuario usuario, string submit)
        {

            if (submit == "Voltar")
                return RedirectToAction("Index");

            HttpResponseMessage response;

            if (Session["email"] != null && Session["email"].ToString() != usuario.Email )
            {
                response = GlobalVariables.WebApiClient.PostAsJsonAsync("usuarios/email", usuario).Result;
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    ViewBag.Mensagem = "O e-mail inforamado já existe";
                    return View("FormUsuario", usuario);
                }
            }

            if (usuario.Id != 0)
                response = GlobalVariables.WebApiClient.PutAsJsonAsync("usuarios/" + usuario.Id, usuario).Result;
            else
                response = GlobalVariables.WebApiClient.PostAsJsonAsync("usuarios", usuario).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return RedirectToAction("Index");
            else
                return View("Error");
        }
        
        public ActionResult Selecionar(int id)
        {
            mvcUsuario usuario = new mvcUsuario();
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Usuarios/" + id).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var json = response.Content.ReadAsStringAsync();
                usuario = JsonConvert.DeserializeObject<mvcUsuario>(json.Result);
                usuario.Id = id;
                Session["email"] = usuario.Email;
            }
            return View("FormUsuario", usuario);
        }

        public ActionResult Excluir(int id)
        {
            HttpResponseMessage response = GlobalVariables.WebApiClient.DeleteAsync("Usuarios/" + id).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return RedirectToAction("Index");
            else
                return View("Error");
        }

        private IEnumerable<mvcUsuario> ListarUsuarios()
        {
            IEnumerable<mvcUsuario> usu = null;
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Usuarios").Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                usu = response.Content.ReadAsAsync<IEnumerable<mvcUsuario>>().Result;

            return usu;
        }
    }
}