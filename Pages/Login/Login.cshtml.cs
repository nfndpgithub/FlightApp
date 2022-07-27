using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using CSCL;

namespace vezba.Pages.Login
{
    public class LoginModel : PageModel
    {
        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
        }
        public void OnPost()
        {
            string username = Request.Form["username"];
            string password = Request.Form["password"];
            if (username.Length == 0 || password.Length == 0)
            {
                errorMessage = "All fields are required";
                return;
            }
            try
            {
                ConnectionStringManager cs = new ConnectionStringManager();
                var connectionString = cs.GetConnectionString();

                
                SqlConnection connection = new SqlConnection(connectionString);

                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand cmd = new SqlCommand("select * from users where username='"+username+"'and password='"+password+"'",connection);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        
                        var role = dr.GetValue(3).ToString();
                        HttpContext.Session.SetString("username", dr.GetValue(1).ToString());
                        HttpContext.Session.SetString("role", role);
                        
                    }
                    if (HttpContext.Session.GetString("role") == "posetilac")
                    {
                        Response.Redirect("/Reservation/Index");
                    }
                    if(HttpContext.Session.GetString("role") == "admin" || HttpContext.Session.GetString("role") == "agent")
                    {
                        Response.Redirect("/Flights/Index");
                    }
                    
                }
                else
                {
                    errorMessage = "invalid credentials";
                    return;

                }


            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;

            }

        }
    }
}
