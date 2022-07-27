using CSCL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using vezba.Model;

namespace vezba.Pages.Users
{
    public class EditModel : PageModel
    {
        public User user = new User();
        public string errorMessage = "";
        public string successMessage = "";
        

        public void OnGet()
        {
             String id=Request.Query["id"];
            
            try
            {
                if (HttpContext.Session.GetString("username") == "" || HttpContext.Session.GetString("username") == null)
                {
                    Response.Redirect("/Login/Login");
                }
                ConnectionStringManager cs = new ConnectionStringManager();
                var connectionString = cs.GetConnectionString();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "select * from users where id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //User user = new User();
                                user.id =""+ reader.GetInt32(0);
                                user.name = reader.GetString(1);
                                user.password = reader.GetString(2);
                                user.role = reader.GetString(3);
                               

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage=ex.Message;
                return;
            }
        }
        public void OnPost()
        {
            user.id = Request.Form["id"];
            user.name = Request.Form["name"];
            user.password = Request.Form["password"];
            user.role = Request.Form["role"];
            if (user.name.Length == 0 || user.password.Length == 0 || user.role.Length == 0)
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
                    
                    String sql = "update users set username=@name, password=@password, role=@role where id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", user.name);
                        command.Parameters.AddWithValue("@password", user.password);
                        command.Parameters.AddWithValue("@role", user.role);
                        command.Parameters.AddWithValue("@id", user.id);
                        command.ExecuteNonQuery();

                    }
                }
            }catch(Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            Response.Redirect("/Users/Index");

        }
    }
}
