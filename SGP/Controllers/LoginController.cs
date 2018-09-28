using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using SGP.Models;
namespace SGP.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        SGPAPIEntities db = new SGPAPIEntities();
        public ActionResult Index(string User, string Pass, string Post)
        {
            Session["User"] = "";
            string view = "";
            int a;
            if (String.IsNullOrEmpty(User) || String.IsNullOrEmpty(Pass) || String.IsNullOrEmpty(Post))
              {
                  User = "";
                  Pass = "";
                  Post = "";
              }
            else
             {
                var paraUser = new SqlParameter("@User", User);
                var paraPass = new SqlParameter("@Pass", System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Pass, "SHA1"));
                var paraPost = new SqlParameter("@Post", Post);
                List<ResponeLogin> list = new List<ResponeLogin>();
                var result = db.Database.SqlQuery<ResponeLogin>("SGP_WEB_Login @User,@Pass,@Post", paraUser, paraPass,paraPost).First();
                a = result.Success;
                if (result.Success == 1)
                {
                    Session["User"] = User;
                    view = "~/Views/Home/Index.cshtml";
                }
                else
                {
                    view = "";
                }
            }
            return View(view);
           
        }
	}
}