using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using vezba.Model;
using System.Data.SqlClient;
using CSCL;

namespace vezba.Pages.Flights
{
    public class CreateModel : PageModel
    {
        Flight flight = new Flight();
        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
            if (HttpContext.Session.GetString("username") == "" || HttpContext.Session.GetString("username") == null)
            {
                Response.Redirect("/Login/Login");
            }
        }
        public void OnPost()
        {
            flight.From=Request.Form["from"];
            flight.To = Request.Form["to"];
            if (Request.Form["date"]!=""&& Request.Form["stops"]!=""&& Request.Form["capacity"]!="" && Convert.ToInt32(Request.Form["stops"])>=0 && Convert.ToInt32(Request.Form["capacity"])>=0 && flight.From.Length!=0 && flight.To.Length != 0)
            {
                flight.Date = Convert.ToDateTime(Request.Form["date"]);
                flight.Stops = Convert.ToInt32(Request.Form["stops"]);
                flight.Capacity = Convert.ToInt32(Request.Form["capacity"]);
                if(DateTime.Now.CompareTo(flight.Date) > 0)
                {
                    errorMessage = "Invalid date;";
                    return;
                }
            }
            else
            {errorMessage = "All fields are required.";
                                return;

            }
            

            
                
            
            try
            {
                ConnectionStringManager cs = new ConnectionStringManager();
                var connectionString = cs.GetConnectionString();
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "Insert into flights (od, do, date, stops, capacity) values(@from, @to, @date, @stops, @capacity);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@from", flight.From);
                        command.Parameters.AddWithValue("@to", flight.To);
                        command.Parameters.AddWithValue("@date", flight.Date);
                        command.Parameters.AddWithValue("@stops", flight.Stops);
                        command.Parameters.AddWithValue("@capacity", flight.Capacity);
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
