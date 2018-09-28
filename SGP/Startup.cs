using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using SGP.Models;

[assembly: OwinStartupAttribute(typeof(SGP.Startup))]
namespace SGP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
        }
        private void createRolesandUsers()
        {
            //ApplicationDbContext context = new ApplicationDbContext();

            //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            //var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User     
            //if (!roleManager.RoleExists("Admin"))
            //{

            //    // first we create Admin rool    
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Admin";
            //    roleManager.Create(role);

            //    //Here we create a Admin super user who will maintain the website                   

            //    var user = new ApplicationUser();
            //    user.UserName = "ketoan";
            //    string userPWD = "123456";
            //    var chkUser = UserManager.Create(user, userPWD);

            //    //Add default User to Role Admin    
            //    if (chkUser.Succeeded)
            //    {
            //        var result1 = UserManager.AddToRole(user.Id, "Admin");

            //    }
            //}
            ////tao tai khoan nguoi dung
            //var user1 = new ApplicationUser();
            //user1.UserName = "ketoan";
            //string userPWD1 = "123456";
            //var chkUser1 = UserManager.Create(user1, userPWD1);
            // creating Creating Manager role     
            //if (!roleManager.RoleExists("Manager"))
            //{
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Manager";
            //    roleManager.Create(role);

            //}

            // creating Creating Employee role     
            //if (!roleManager.RoleExists("Employee"))
            //{
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Employee";
            //    roleManager.Create(role);
            //}
            // creating Creating Employee role     
            //if (!roleManager.RoleExists("Reporter"))
            //{
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Reporter";
            //    roleManager.Create(role);
            //}
            //if (!roleManager.RoleExists("Accounting")) // tai khoan ke toan
            //{
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Accounting";
            //    roleManager.Create(role);
            //}
        }
    }
}
