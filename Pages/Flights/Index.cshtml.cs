using CSCL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using vezba.Model;

namespace vezba.Pages.Flights
{
    public class IndexModel : PageModel
    {
        public List<Flight> listFlights = new List<Flight>();
        public void OnGet()
        {
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
                    String sql = "select * from flights";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Flight flight = new Flight();

                                flight.Id =  reader.GetInt32(0);
                                flight.From= reader.GetString(1);
                                flight.To = reader.GetString(2);
                                flight.Date = Convert.ToDateTime(reader.GetDateTime(3));
                                flight.Stops = reader.GetInt32(4);
                                flight.Capacity = reader.GetInt32(5);
                                listFlights.Add(flight);
                                

                                /*user.id = "" + reader.GetInt32(0);
                                user.name = reader.GetString(1);
                                user.password = reader.GetString(2);
                                user.role = reader.GetString(3);
                                listUsers.Add(user);*/

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("greska neka");
            }
        }
    }
}
