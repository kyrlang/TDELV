using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginWithApi.Models.MVC
{
    public class mvcUsuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "O E-mail não é válido")]
        [EmailAddress]
        public string Email { get; set; }
        public string Senha { get; set; }
        public Nullable<System.DateTime> CadastroData { get; set; }
    }
}