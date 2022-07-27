using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using vezba.Model;
using CSCL;


namespace vezba.Pages.Users
{
    public class IndexModel : PageModel
    { public List<User> listUsers = new List<User>();
        public void OnGet()
        {
            try
            {
                if (HttpContext.Session.GetString("username") == "" || HttpContext.Session.GetString("username") == null)
                {
                    Response.Redirect("/Login/Login");
                }
                ConnectionStringManager cs=new ConnectionStringManager();
                var connectionString = cs.GetConnectionString();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "select * from users";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                                while (reader.Read())
                            {
                                User user = new User();
                                user.id = "" + reader.GetInt32(0);
                                user.name = reader.GetString(1);
                                user.password=reader.GetString(2);  
                                user.role=reader .GetString(3);
                                listUsers.Add(user);    

                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("greska neka");
            }
           

        }
    }
    /*public class User
    {
        public String id;
        public string name;
        public string password;
        public string role;
    }*/

}
