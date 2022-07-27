using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;
using vezba.Model;
using System.Configuration;
using CSCL;

namespace vezba.Pages.Users
{
    public class CreateModel : PageModel
    {
        public User user = new User();

        public string errorMessage = "";
        public string successMessage ="";


        public void OnGet()
        {
            if (HttpContext.Session.GetString("username") == "" || HttpContext.Session.GetString("username") == null)
            {
                Response.Redirect("/Login/Login");
            }
        }
        public void OnPost()
        {
            user.name=Request.Form["name"];
            user.password=Request.Form["password"]; 
            user.role = Request.Form["role"];

            if(user.name.Length==0||user.password.Length==0|| user.role.Length == 0)
            {
                errorMessage = "All fields are required";
                return;
            }
            try
            {
                ConnectionStringManager cs = new ConnectionStringManager();
                var connectionString = cs.GetConnectionString();
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("select * from users where username='" + user.name + "'", connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        errorMessage = "User already exist";
                        return;
                    }
                    dr.Close();
                        String sql = "Insert into users (username, password, role) values(@name, @password, @role);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", user.name);
                        command.Parameters.AddWithValue("@password", user.password);
                        command.Parameters.AddWithValue("@role", user.role);
                        command.ExecuteNonQuery();
                        
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage=ex.Message;
                return;

            }

            user.name = "";
            user.password = "";
            user.role = "";

            successMessage = "New user is added";
            Response.Redirect("/Users/Index");
        }
    }
}
