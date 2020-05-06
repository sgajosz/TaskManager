using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Models;
using TaskManager.ViewModels;

namespace TaskManager.Controllers
{
    public class DashboardController : Controller
    {
        public ActionResult Projects()
        {
            if (Session["LoggedIn"] == null)
                return RedirectToAction("Login", "Account");

            ProjectContext projectContext = new ProjectContext();
            List<Project> projects = projectContext.Projects.ToList();
            List<Project> userProjects = new List<Project>();
            foreach (Project proj in projects)
                if(proj.UserFK == (int)Session["ID"])
                    userProjects.Add(proj);
            UserContext userContext = new UserContext();
            ViewBag.UserContext = userContext;

            return View(userProjects);
        }

        [HttpGet]
        public ActionResult AddProject()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProject(ProjectViewModel pvm)
        {
            if (ModelState.IsValid && Session["LoggedIn"] != null)
            {
                Project proj = new Project(pvm.Name, DateTime.Now);
                string connectionString = ConfigurationManager.ConnectionStrings["ProjectContext"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand("addProject", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter userFk = new SqlParameter();
                userFk.ParameterName = "@User";
                userFk.Value = Session["ID"];
                cmd.Parameters.Add(userFk);

                SqlParameter name = new SqlParameter();
                name.ParameterName = "@Name";
                name.Value = proj.Name;
                cmd.Parameters.Add(name);

                SqlParameter creation = new SqlParameter();
                creation.ParameterName = "@Creation";
                creation.Value = proj.Creation;
                cmd.Parameters.Add(creation);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            else
                return RedirectToAction("Login", "Account");


            return RedirectToAction("Projects", "Dashboard");
        }
    }
}