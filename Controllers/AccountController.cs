using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using TaskManager.Models;
using TaskManager.ViewModels;

namespace TaskManager.Controllers
{
    public class AccountController : Controller
    {
        private static string CreateMD5(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegistrationViewModel rvm)
        {
            if (ModelState.IsValid)
            {
                User usr = new User(rvm.Email, rvm.Name, rvm.Surname, rvm.Password);
                string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand("addUser", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter emailParam = new SqlParameter();
                emailParam.ParameterName = "@Email";
                emailParam.Value = usr.Email;
                cmd.Parameters.Add(emailParam);

                SqlParameter nameParam = new SqlParameter();
                nameParam.ParameterName = "@Name";
                nameParam.Value = usr.Name;
                cmd.Parameters.Add(nameParam);

                SqlParameter surnameParam = new SqlParameter();
                surnameParam.ParameterName = "@Surname";
                surnameParam.Value = usr.Surname;
                cmd.Parameters.Add(surnameParam);

                SqlParameter passwordParam = new SqlParameter();
                passwordParam.ParameterName = "@Password";
                passwordParam.Value = CreateMD5(usr.Password);
                cmd.Parameters.Add(passwordParam);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();

                return RedirectToAction("Login", "Account");
            }

            return View(rvm);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel lvm)
        {
            if (ModelState.IsValid)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand("getUser", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter emailParam = new SqlParameter();
                emailParam.ParameterName = "@Email";
                emailParam.Value = lvm.Email;
                cmd.Parameters.Add(emailParam);

                SqlParameter nameParam = new SqlParameter();
                nameParam.ParameterName = "@Password";
                nameParam.Value = CreateMD5(lvm.Password);
                cmd.Parameters.Add(nameParam);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    User usr = new User(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                    usr.ID = reader.GetInt32(0);
                    Session["ID"] = usr.ID;
                    Session["Email"] = usr.Email;
                    Session["Name"] = usr.Name;
                    Session["Surname"] = usr.Surname;
                    Session["LoggedIn"] = true;
                    reader.Close();
                    connection.Close();
                    return RedirectToAction("Projects", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError("IncorrectData", "Email or password is incorrect");
                    connection.Close();
                    return View(lvm);
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Remove("ID");
            Session.Remove("Email");
            Session.Remove("Name");
            Session.Remove("Surname");
            Session.Remove("LoggedIn");
            return RedirectToAction("Login", "Account");
        }
    }
}