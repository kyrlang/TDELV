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
    public class HomeController : Controller
    {

        // GET: Home
        public ActionResult Index()
        {
            mvcUsuario usuario = new mvcUsuario();
            return View(usuario);
        }

        
        [ActionName("Login")]
        public ActionResult Login(mvcUsuario usuario)
        {
            if (Session["UsuarioLogado"] != null)
                usuario = (mvcUsuario)Session["UsuarioLogado"];

            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("Usuarios/Login", usuario).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var json = response.Content.ReadAsStringAsync();
                usuario = JsonConvert.DeserializeObject<mvcUsuario>(json.Result);
                Session["UsuarioLogado"] = usuario;
                return View("BoasVindas", usuario);
            }

            ViewBag.Mensagem = "Usuário informado não existe";
            return View("Index", usuario);
        }

        public ActionResult LembrarSenha()
        {
            return View();
        }
    }
}