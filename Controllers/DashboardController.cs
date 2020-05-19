using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using TaskManager.Models;
using TaskManager.ViewModels;

namespace TaskManager.Controllers
{
    public class DashboardController : Controller
    {
        private bool IsOwner()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            bool isOwner = false;

            string sqlCmd = String.Format("SELECT * FROM projects WHERE id = {0} AND userFK = {1}", Session["CurrentProject"].ToString(), Session["ID"].ToString());
            SqlCommand ownerCheck = new SqlCommand(sqlCmd, connection);
            if (ownerCheck.ExecuteReader().HasRows)
                isOwner = true;
            else
                isOwner = false;

            connection.Close();
            return isOwner;
        }

        private bool IsParticipant(int projectID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            bool isParticipant = false;

            string sqlCmd = String.Format("SELECT * FROM project_user_join WHERE userFK = {0} and projectFK = {1}", Session["ID"].ToString(), projectID.ToString());
            SqlCommand participantCheck = new SqlCommand(sqlCmd, connection);
            if (participantCheck.ExecuteReader().HasRows)
                isParticipant = true;
            else
                isParticipant = false;

            connection.Close();
            return isParticipant;
        }
        public ActionResult Projects()
        {
            if (Session["LoggedIn"] == null)
                return RedirectToAction("Login", "Account");

            string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);

            string cmdString = String.Format("SELECT * FROM projects p WHERE EXISTS (SELECT * FROM project_user_join puj WHERE puj.projectFK = p.id AND puj.userFK = {0}) OR p.userFK = {0}", Session["ID"].ToString());

            SqlCommand cmd = new SqlCommand(cmdString, connection);

            List<Project> userProjects = new List<Project>();

            connection.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            while(sdr.Read())
                userProjects.Add(new Project(sdr.GetInt32(0), sdr.GetInt32(1), sdr.GetString(2), sdr.GetDateTime(3)));
            connection.Close();

            UserContext userContext = new UserContext();
            ViewBag.UserContext = userContext;

            return View(userProjects);
        }

        [HttpGet]
        public ActionResult AddProject()
        {
            if (Session["LoggedIn"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public ActionResult AddProject(ProjectViewModel pvm)
        {
            if (ModelState.IsValid && Session["LoggedIn"] != null)
            {
                Project proj = new Project(pvm.Name, DateTime.Now);
                string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
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

        [HttpGet]
        public ActionResult AddPersonToProject(int? projectID)
        {  
            if (Session["LoggedIn"] == null)
                return RedirectToAction("Login", "Account");

            if (projectID == null)
                return RedirectToAction("Projects", "Dashboard");

            Session["CurrentProject"] = projectID;

            if (!IsOwner())
                return RedirectToAction("Projects", "Dashboard");

            string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string cmdString = String.Format("SELECT * FROM users u WHERE NOT EXISTS (SELECT * FROM project_user_join puj WHERE {0} = puj.projectFK AND u.id = puj.userFK) AND u.id != (SELECT userFK FROM projects WHERE id = {0})", projectID);

            SqlCommand cmd = new SqlCommand(cmdString, connection);
            SqlDataReader sdr = cmd.ExecuteReader();
            List<string> users = new List<string>();
            while (sdr.Read())
                users.Add(sdr.GetString(2) + ' ' + sdr.GetString(3) + " #" + sdr.GetInt32(0));
            connection.Close();
            ViewBag.Users = users;
            
            return View();
        }

        [HttpPost]
        public ActionResult AddPersonToProject(ProjectUserJoinViewModel pujvm)
        {
            if (Session["LoggedIn"] == null)
                return RedirectToAction("Login", "Account");

            if (pujvm.UserString == null)
                return RedirectToAction("AddPersonToProject", "Dashboard", new { projectID = Int32.Parse(Session["CurrentProject"].ToString()) });

            if (ModelState.IsValid)
            {
                string id = pujvm.UserString.Substring(pujvm.UserString.IndexOf('#') + 1);

                string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);

                string cmdString = String.Format("INSERT INTO project_user_join VALUES ({0}, {1})", Session["CurrentProject"].ToString(), id);

                SqlCommand cmd = new SqlCommand(cmdString, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            else
                return View();

            return RedirectToAction("Projects", "Dashboard");
        }

        [HttpGet]
        public ActionResult Tasks(int? projectID)
        {
            if (Session["LoggedIn"] == null)
                return RedirectToAction("Login", "Account");

            if (projectID == null)
                return RedirectToAction("Projects", "Dashboard");

            Session["CurrentProject"] = projectID;

            if (!IsOwner() && !IsParticipant((int)projectID))
                return RedirectToAction("Projects", "Dashboard");

            UserContext userContext = new UserContext();
            ViewBag.UserContext = userContext;
            ProjectContext projectContext = new ProjectContext();
            ViewBag.ProjectContext = projectContext;

            Session["CurrentProjectName"] = projectContext.Projects.Single(proj => proj.ID == projectID).Name;

            string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand("getTaskForProject", connection);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            SqlParameter projectFk = new SqlParameter();
            projectFk.ParameterName = "@Project";
            projectFk.Value = Session["CurrentProject"];
            cmd.Parameters.Add(projectFk);

            List<Task> projectTasks = new List<Task>();
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
                projectTasks.Add(new Task(sdr.GetInt32(0), sdr.GetInt32(1), sdr.GetInt32(2), sdr.GetString(3), sdr.GetString(4), sdr.GetString(5), sdr.GetString(6), sdr.GetInt32(7), sdr.GetInt32(8), sdr.GetString(9), sdr.GetString(10)));
            connection.Close();


            return View(projectTasks);
        }

        [HttpGet]
        public ActionResult AddTask()
        {
            if (Session["LoggedIn"] == null)
                return RedirectToAction("Login", "Account");

            string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string sqlCmd = String.Format("SELECT * FROM users u WHERE EXISTS (SELECT * FROM project_user_join puj WHERE u.id = puj.userFK AND projectFK = {0})", Session["CurrentProject"]);
            SqlCommand participantsCmd = new SqlCommand(sqlCmd, connection);

            SqlDataReader sdr = participantsCmd.ExecuteReader();
            List<User> users = new List<User>();
            while(sdr.Read())
                users.Add(new Models.User(sdr.GetInt32(0), sdr.GetString(3), sdr.GetString(3)));

            connection.Close();

            connection.Open();

            bool isOwner;

            sqlCmd = String.Format("SELECT * FROM projects WHERE id = {0} AND userFK = {1}", Session["CurrentProject"].ToString(), Session["ID"].ToString());
            SqlCommand ownerCheck = new SqlCommand(sqlCmd, connection);
            if (ownerCheck.ExecuteReader().HasRows)
                isOwner = true;
            else
                isOwner = false;

            ViewBag.isOwner = isOwner;

            connection.Close();

            List<string> userStrings = new List<string>();
            foreach (User u in users)
                userStrings.Add(u.Name + " " + u.Surname + " #" + u.ID.ToString());

            List<string> technologies = new List<string>();
            sqlCmd = String.Format("SELECT name FROM technologies WHERE projectFK = {0}", Session["CurrentProject"].ToString());
            SqlCommand getTechnologies = new SqlCommand(sqlCmd, connection);
            connection.Open();
            SqlDataReader technologyReader = getTechnologies.ExecuteReader();
            while (technologyReader.Read())
                technologies.Add(technologyReader.GetString(0));
            connection.Close();

            List<string> tasks = new List<string>();
            sqlCmd = String.Format("SELECT name FROM errands WHERE projectFK = {0}", Session["CurrentProject"].ToString());
            SqlCommand getErrands = new SqlCommand(sqlCmd, connection);
            connection.Open();
            SqlDataReader errandsReader = getErrands.ExecuteReader();
            while (errandsReader.Read())
                tasks.Add(errandsReader.GetString(0));
            connection.Close();

            List<string> types = new List<string>();
            sqlCmd = String.Format("SELECT name FROM types WHERE projectFK = {0}", Session["CurrentProject"].ToString());
            SqlCommand getTypes = new SqlCommand(sqlCmd, connection);
            connection.Open();
            SqlDataReader typesReader = getTypes.ExecuteReader();
            while (typesReader.Read())
                types.Add(typesReader.GetString(0));
            connection.Close();

            List<string> statuses = new List<string> { "New", "In progress", "Done", "Canceled" };

            ViewBag.Users = userStrings;
            ViewBag.Technologies = technologies;
            ViewBag.Tasks = tasks;
            ViewBag.Types = types;
            ViewBag.Statuses = statuses;

            return View();
        }

        [HttpPost]
        public ActionResult AddTask(TaskViewModel tvm)
        {
            if (ModelState.IsValid && Session["LoggedIn"] != null)
            {
                Task task = new Task(tvm.Name, tvm.Technology, tvm.Errand, tvm.Type, tvm.Hours, tvm.Status, tvm.Description);
                string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand("addTask", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter projectFK = new SqlParameter();
                projectFK.ParameterName = "@Project";
                projectFK.Value = Session["CurrentProject"];
                cmd.Parameters.Add(projectFK);

                SqlParameter userFk = new SqlParameter();
                userFk.ParameterName = "@User";
                userFk.Value = Int32.Parse(tvm.User.Split(' ').Last().Trim('#'));
                cmd.Parameters.Add(userFk);

                SqlParameter name = new SqlParameter();
                name.ParameterName = "@Name";
                name.Value = task.Name;
                cmd.Parameters.Add(name);

                SqlParameter technology = new SqlParameter();
                technology.ParameterName = "@Technology";
                technology.Value = task.Technology;
                cmd.Parameters.Add(technology);

                SqlParameter errand = new SqlParameter();
                errand.ParameterName = "@Errand";
                errand.Value = task.Errand;
                cmd.Parameters.Add(errand);

                SqlParameter type = new SqlParameter();
                type.ParameterName = "@Type";
                type.Value = task.Type;
                cmd.Parameters.Add(type);

                SqlParameter hours = new SqlParameter();
                hours.ParameterName = "@Hours";
                hours.Value = task.Hours;
                cmd.Parameters.Add(hours);

                SqlParameter doneHours = new SqlParameter();
                doneHours.ParameterName = "@DoneHours";
                doneHours.Value = 0;
                cmd.Parameters.Add(doneHours);

                SqlParameter status = new SqlParameter();
                status.ParameterName = "@Status";
                status.Value = tvm.Status;
                cmd.Parameters.Add(status);

                SqlParameter description = new SqlParameter();
                description.ParameterName = "@Description";
                description.Value = task.Description;
                cmd.Parameters.Add(description);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            else
                return RedirectToAction("Login", "Account");


            return RedirectToAction("Tasks", "Dashboard", new { projectID = Session["CurrentProject"]} );
        }

        [HttpGet]
        public ActionResult EditTask(int? taskID)
        {
            if (Session["LoggedIn"] == null)
                return RedirectToAction("Login", "Account");

            if (taskID == null)
                return RedirectToAction("Projects", "Dashboard");

            bool isOwner = IsOwner(), isParticipant = IsParticipant((int)Session["CurrentProject"]);

            if (!isOwner && !isParticipant)
                return RedirectToAction("Projects", "Dashboard");

            ViewBag.isOwner = isOwner;

            string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);

            string sqlCmd = String.Format("SELECT * FROM tasks WHERE id = {0}", taskID.ToString());
            SqlCommand getTasks = new SqlCommand(sqlCmd, connection);
            connection.Open();
            SqlDataReader tasksReader = getTasks.ExecuteReader();
            Task task = new Task();
            while (tasksReader.Read())
                task = new Task(tasksReader.GetInt32(0), tasksReader.GetInt32(1), tasksReader.GetInt32(2), tasksReader.GetString(3), tasksReader.GetString(4), tasksReader.GetString(5), tasksReader.GetString(6), tasksReader.GetInt32(7), tasksReader.GetInt32(8), tasksReader.GetString(9), tasksReader.GetString(10));
            connection.Close();

            connection.Open();

            sqlCmd = String.Format("SELECT * FROM users u WHERE EXISTS (SELECT * FROM project_user_join puj WHERE u.id = puj.userFK AND projectFK = {0})", Session["CurrentProject"]);
            SqlCommand participantsCmd = new SqlCommand(sqlCmd, connection);

            SqlDataReader sdr = participantsCmd.ExecuteReader();
            List<User> users = new List<User>();
            while (sdr.Read())
                users.Add(new Models.User(sdr.GetInt32(0), sdr.GetString(2), sdr.GetString(3)));

            connection.Close();

            List<string> userStrings = new List<string>();
            foreach (User u in users)
                userStrings.Add(u.Name + " " + u.Surname + " #" + u.ID.ToString());

            List<string> technologies = new List<string>();
            sqlCmd = String.Format("SELECT name FROM technologies WHERE projectFK = {0}", Session["CurrentProject"].ToString());
            SqlCommand getTechnologies = new SqlCommand(sqlCmd, connection);
            connection.Open();
            SqlDataReader technologyReader = getTechnologies.ExecuteReader();
            while (technologyReader.Read())
                technologies.Add(technologyReader.GetString(0));
            connection.Close();

            List<string> tasks = new List<string>();
            sqlCmd = String.Format("SELECT name FROM errands WHERE projectFK = {0}", Session["CurrentProject"].ToString());
            SqlCommand getErrands = new SqlCommand(sqlCmd, connection);
            connection.Open();
            SqlDataReader errandsReader = getErrands.ExecuteReader();
            while (errandsReader.Read())
                tasks.Add(errandsReader.GetString(0));
            connection.Close();

            List<string> types = new List<string>();
            sqlCmd = String.Format("SELECT name FROM types WHERE projectFK = {0}", Session["CurrentProject"].ToString());
            SqlCommand getTypes = new SqlCommand(sqlCmd, connection);
            connection.Open();
            SqlDataReader typesReader = getTypes.ExecuteReader();
            while (typesReader.Read())
                types.Add(typesReader.GetString(0));
            connection.Close();

            List<string> statuses = new List<string> { "New", "In progress", "Done", "Canceled" };

            TaskViewModel tvm = new TaskViewModel();
            sqlCmd = String.Format("SELECT * FROM users WHERE id = {0}", task.UserFK.ToString());
            SqlCommand getUser = new SqlCommand(sqlCmd, connection);
            connection.Open();
            SqlDataReader usersReader = getUser.ExecuteReader();
            while (usersReader.Read())
                tvm.User = usersReader.GetString(2) + " " + usersReader.GetString(3) + " #" + usersReader.GetInt32(0).ToString();
            connection.Close();

            tvm.Name = task.Name;
            tvm.Technology = task.Technology;
            tvm.Errand = task.Errand;
            tvm.Type = task.Type;
            tvm.Hours = task.Hours;
            tvm.DoneHours = task.DoneHours;
            tvm.Status = task.Status;
            tvm.Description = task.Description;

            ViewBag.Users = userStrings;
            ViewBag.Technologies = technologies;
            ViewBag.Tasks = tasks;
            ViewBag.Types = types;
            ViewBag.Statuses = statuses;
            ViewBag.Task = task;

            return View(tvm);
        }

        [HttpPost]
        public ActionResult EditTask(TaskViewModel tvm)
        {
            if (ModelState.IsValid && Session["LoggedIn"] != null)
            {
                Task task = new Task(tvm.Name, tvm.Technology, tvm.Errand, tvm.Type, tvm.Hours, tvm.Status, tvm.Description);
                string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand("editTask", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter id = new SqlParameter();
                id.ParameterName = "@ID";
                id.Value = Session["CurrentTask"];
                cmd.Parameters.Add(id);

                SqlParameter userFk = new SqlParameter();
                userFk.ParameterName = "@User";
                userFk.Value = Int32.Parse(tvm.User.Split(' ').Last().Trim('#'));
                cmd.Parameters.Add(userFk);

                SqlParameter name = new SqlParameter();
                name.ParameterName = "@Name";
                name.Value = task.Name;
                cmd.Parameters.Add(name);

                SqlParameter technology = new SqlParameter();
                technology.ParameterName = "@Technology";
                technology.Value = task.Technology;
                cmd.Parameters.Add(technology);

                SqlParameter errand = new SqlParameter();
                errand.ParameterName = "@Errand";
                errand.Value = task.Errand;
                cmd.Parameters.Add(errand);

                SqlParameter type = new SqlParameter();
                type.ParameterName = "@Type";
                type.Value = task.Type;
                cmd.Parameters.Add(type);

                SqlParameter hours = new SqlParameter();
                hours.ParameterName = "@Hours";
                hours.Value = task.Hours;
                cmd.Parameters.Add(hours);

                SqlParameter doneHours = new SqlParameter();
                doneHours.ParameterName = "@DoneHours";
                doneHours.Value = tvm.DoneHours;
                cmd.Parameters.Add(doneHours);

                SqlParameter status = new SqlParameter();
                status.ParameterName = "@Status";
                status.Value = tvm.Status;
                cmd.Parameters.Add(status);

                SqlParameter description = new SqlParameter();
                description.ParameterName = "@Description";
                description.Value = task.Description;
                cmd.Parameters.Add(description);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            else
                return RedirectToAction("Login", "Account");


            return RedirectToAction("Tasks", "Dashboard", new { projectID = Session["CurrentProject"] });
        }

        [HttpGet]
        public ActionResult AddTechnology()
        {
            if (Session["LoggedIn"] == null)
                return RedirectToAction("Login", "Account");

            string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            bool isOwner;

            string sqlCmd = String.Format("SELECT * FROM projects WHERE id = {0} AND userFK = {1}", Session["CurrentProject"].ToString(), Session["ID"].ToString());
            SqlCommand ownerCheck = new SqlCommand(sqlCmd, connection);
            if (ownerCheck.ExecuteReader().HasRows)
                isOwner = true;
            else
                isOwner = false;

            connection.Close();

            if (!isOwner)
                return RedirectToAction("AddTasks", "Dashboard");

            return View();
        }

        [HttpPost]
        public ActionResult AddTechnology(TechnologyViewModel tvm)
        {
            if (Session["LoggedIn"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {

                string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                bool isOwner;

                string sqlCmd = String.Format("SELECT * FROM projects WHERE id = {0} AND userFK = {1}", Session["CurrentProject"].ToString(), Session["ID"].ToString());
                SqlCommand ownerCheck = new SqlCommand(sqlCmd, connection);
                if (ownerCheck.ExecuteReader().HasRows)
                    isOwner = true;
                else
                    isOwner = false;

                connection.Close();

                if (!isOwner)
                    return RedirectToAction("AddTask", "Dashboard");


                SqlCommand insertTechnology = new SqlCommand("addTechnology", connection);
                insertTechnology.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter name = new SqlParameter();
                name.ParameterName = "@Name";
                name.Value = tvm.Name;
                insertTechnology.Parameters.Add(name);

                SqlParameter projectFK = new SqlParameter();
                projectFK.ParameterName = "@ProjectFK";
                projectFK.Value = Session["CurrentProject"];
                insertTechnology.Parameters.Add(projectFK);

                SqlParameter price = new SqlParameter();
                price.ParameterName = "@Price";
                price.Value = tvm.Price;
                insertTechnology.Parameters.Add(price);

                connection.Open();
                insertTechnology.ExecuteNonQuery();
                connection.Close();
            }
            else
                return View();
            return RedirectToAction("AddTask", "Dashboard");
        }

        [HttpGet]
        public ActionResult AddType()
        {
            if (Session["LoggedIn"] == null)
                return RedirectToAction("Login", "Account");

            string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            bool isOwner;

            string sqlCmd = String.Format("SELECT * FROM projects WHERE id = {0} AND userFK = {1}", Session["CurrentProject"].ToString(), Session["ID"].ToString());
            SqlCommand ownerCheck = new SqlCommand(sqlCmd, connection);
            if (ownerCheck.ExecuteReader().HasRows)
                isOwner = true;
            else
                isOwner = false;

            connection.Close();

            if (!isOwner)
                return RedirectToAction("AddTask", "Dashboard");

            return View();
        }

        [HttpPost]
        public ActionResult AddType(TypeViewModel tvm)
        {
            if (Session["LoggedIn"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {

                string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                bool isOwner;

                string sqlCmd = String.Format("SELECT * FROM projects WHERE id = {0} AND userFK = {1}", Session["CurrentProject"].ToString(), Session["ID"].ToString());
                SqlCommand ownerCheck = new SqlCommand(sqlCmd, connection);
                if (ownerCheck.ExecuteReader().HasRows)
                    isOwner = true;
                else
                    isOwner = false;

                connection.Close();

                if (!isOwner)
                    return RedirectToAction("AddTask", "Dashboard");


                SqlCommand insertTechnology = new SqlCommand("addType", connection);
                insertTechnology.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter name = new SqlParameter();
                name.ParameterName = "@Name";
                name.Value = tvm.Name;
                insertTechnology.Parameters.Add(name);

                SqlParameter projectFK = new SqlParameter();
                projectFK.ParameterName = "@ProjectFK";
                projectFK.Value = Session["CurrentProject"];
                insertTechnology.Parameters.Add(projectFK);

                connection.Open();
                insertTechnology.ExecuteNonQuery();
                connection.Close();
            }
            else
                return View();
            return RedirectToAction("AddTask", "Dashboard");
        }

        [HttpGet]
        public ActionResult AddErrand()
        {
            if (Session["LoggedIn"] == null)
                return RedirectToAction("Login", "Account");

            string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            bool isOwner;

            string sqlCmd = String.Format("SELECT * FROM projects WHERE id = {0} AND userFK = {1}", Session["CurrentProject"].ToString(), Session["ID"].ToString());
            SqlCommand ownerCheck = new SqlCommand(sqlCmd, connection);
            if (ownerCheck.ExecuteReader().HasRows)
                isOwner = true;
            else
                isOwner = false;

            connection.Close();

            if (!isOwner)
                return RedirectToAction("AddTask", "Dashboard");

            return View();
        }

        [HttpPost]
        public ActionResult AddErrand(ErrandViewModel evm)
        {
            if (Session["LoggedIn"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {

                string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                bool isOwner;

                string sqlCmd = String.Format("SELECT * FROM projects WHERE id = {0} AND userFK = {1}", Session["CurrentProject"].ToString(), Session["ID"].ToString());
                SqlCommand ownerCheck = new SqlCommand(sqlCmd, connection);
                if (ownerCheck.ExecuteReader().HasRows)
                    isOwner = true;
                else
                    isOwner = false;

                connection.Close();

                if (!isOwner)
                    return RedirectToAction("AddTask", "Dashboard");


                SqlCommand insertTechnology = new SqlCommand("addErrand", connection);
                insertTechnology.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter name = new SqlParameter();
                name.ParameterName = "@Name";
                name.Value = evm.Name;
                insertTechnology.Parameters.Add(name);

                SqlParameter projectFK = new SqlParameter();
                projectFK.ParameterName = "@ProjectFK";
                projectFK.Value = Session["CurrentProject"];
                insertTechnology.Parameters.Add(projectFK);

                connection.Open();
                insertTechnology.ExecuteNonQuery();
                connection.Close();
            }
            else
                return View();
            return RedirectToAction("AddTask", "Dashboard");
        }

    }
}