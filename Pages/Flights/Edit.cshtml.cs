using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using vezba.Model;
using System.Data.SqlClient;
using CSCL;

namespace vezba.Pages.Flights
{
    public class EditModel : PageModel
    {
        public Flight flight = new Flight();
        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
            String id = Request.Query["id"];
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
                    String sql = "select * from flights where id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                flight.Id = reader.GetInt32(0);
                                flight.From = reader.GetString(1);
                                flight.To = reader.GetString(2);
                                flight.Date = Convert.ToDateTime(reader.GetDateTime(3));
                                flight.Stops = reader.GetInt32(4);
                                flight.Capacity = reader.GetInt32(5);


                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
        }

        public void OnPost()
        {
            flight.Id = Convert.ToInt32(Request.Form["id"]);
            flight.From = Request.Form["from"];
            flight.To = Request.Form["to"];
            if (Request.Form["date"] != "" && Request.Form["stops"] != "" && Request.Form["capacity"] != "" && flight.From.Length != 0 && flight.To.Length != 0)
            {
                flight.Date = Convert.ToDateTime(Request.Form["date"]);
                flight.Stops = Convert.ToInt32(Request.Form["stops"]);
                flight.Capacity = Convert.ToInt32(Request.Form["capacity"]);

            }
            else
            {
                errorMessage = "Fields are empty";
                return;

            }





            try
            {
                ConnectionStringManager cs = new ConnectionStringManager();
                var connectionString = cs.GetConnectionString();
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "update  flights set od=@from, do=@to, date=@date, stops=@stops, capacity=@capacity where id=@id;";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@from", flight.From);
                        command.Parameters.AddWithValue("@to", flight.To);
                        command.Parameters.AddWithValue("@date", flight.Date);
                        command.Parameters.AddWithValue("@stops", flight.Stops);
                        command.Parameters.AddWithValue("@capacity", flight.Capacity);
                        command.Parameters.AddWithValue("@id", flight.Id);
                        command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;

            }
            successMessage = "New flight is added";
            Response.Redirect("/Flights/Index");

        }

    }
}
