using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using vezba.Model;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using CSCL;

namespace vezba.Pages.Profile
{
    public class IndexModel : PageModel
    {
        public List<Flight> listFlights = new List<Flight>();
        public string errorMessage = "";
        public string username;
        public string role;
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
                     username = HttpContext.Session.GetString("username");
                    role= HttpContext.Session.GetString("role");
                    SqlCommand cmd = new SqlCommand("select * from reservations where username='" + username + "'", connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Flight flight = new Flight();
                            flight.From = dr.GetValue(2).ToString();
                            flight.To = dr.GetValue(3).ToString();
                            flight.Date = Convert.ToDateTime(dr.GetValue(4));
                            var seats=dr.GetValue(6).ToString();
                            var status=dr.GetValue(7).ToString();
                            flight.Status = status;
                            flight.Seats = seats;
                            flight.Id = Convert.ToInt32( dr.GetValue(0));

                            listFlights.Add(flight);
                           
                        }


                        dr.Close();
                    }
                    else
                    {
                        errorMessage = "You don't have any flights booked.";
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
