using CSCL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.Data.SqlClient;
using vezba.Hubs;
using vezba.Model;
namespace vezba.Pages.Reservation
{
    public class ReservationModel : PageModel
    {
        public Flight flight = new Flight();
        public List<Flight> listFlights = new List<Flight>();
        public List<Flight> subListFlights = new List<Flight>();
        public string errorMessage = "";
        public string successMessage="";
        private readonly IHubContext<ComHub> _hubContext;
        public ReservationModel(IHubContext<ComHub> hubContext)
        {
            _hubContext = hubContext;

        }
        public void OnGet()
        {
            try
            {
                if (HttpContext.Session.GetString("username") == "" || HttpContext.Session.GetString("username")==null)
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

                                flight.Id = reader.GetInt32(0);
                                flight.From = reader.GetString(1);
                                flight.To = reader.GetString(2);
                                flight.Date = Convert.ToDateTime(reader.GetDateTime(3));
                                flight.Stops = reader.GetInt32(4);
                                flight.Capacity = reader.GetInt32(5);

                                listFlights.Add(flight);




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

        public void OnPost()
        {
            string from = Request.Form["from"];
            string to = Request.Form["to"];
            String date = "";
            if (Request.Form["date"] != "" || from.Length == 0 || to.Length == 0)
            {
                date = Convert.ToDateTime(Request.Form["date"]).Date.ToString("d");
            }
            


            bool non_stop;
            if (Request.Form["stops"] == "on")
            {
                non_stop = true;
            }
            else { non_stop = false; }
            OnGet();
            foreach (Flight flight in listFlights)
            {
                if (date == "")
                {
                    if (flight.From == from && flight.To == to )
                    {

                        if (non_stop)
                        {
                            if (flight.Stops != 0)
                            {


                                continue;
                            }
                        }
                        subListFlights.Add(flight);

                    }

                }
                else
                {

                
                if (flight.From == from && flight.To == to && flight.Date.Date.ToString("d") == date)
                {

                    if (non_stop)
                    {
                        if (flight.Stops != 0)
                        {


                            continue;
                        }
                    }
                    subListFlights.Add(flight);

                }
            }
            }


            listFlights = subListFlights;
            if (listFlights.Count == 0)
            {
                errorMessage = "There is no flights.";
                return;
            }
            return;
        }

        public async void OnPostReserv()
        {
             string id = Request.Form["id"];
            string seats = Request.Form["seats"];
            if (seats.Length == 0)
            {
                errorMessage = "You have to insert  how many seats you want to book.";
                OnGet();
                return;
            }
            
            try
            {
                ConnectionStringManager cs = new ConnectionStringManager();
                var connectionString = cs.GetConnectionString();

                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("select * from flights where id='" + id + "'", connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            flight.Id = (int)dr.GetValue(0);
                            flight.From = dr.GetValue(1).ToString();
                            flight.To = dr.GetValue(2).ToString();
                            flight.Date = Convert.ToDateTime(dr.GetValue(3));
                            flight.Stops = (int)dr.GetValue(4);
                            flight.Capacity = (int)dr.GetValue(5);

                        }
                        if (flight.Capacity < Convert.ToInt32(seats))
                        {
                            errorMessage = "There is not enough seats.";
                            OnGet();
                            return;
                        }

                        dr.Close();
                        cmd = new SqlCommand("select * from reservations where username='" + HttpContext.Session.GetString("username") + "'and flight_id='" + flight.Id + "'", connection);
                        dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            errorMessage = "You already have reserved  this flight.";
                            OnGet();
                            return;
                        }
                        dr.Close();
                        OnGet();
                        String sql = "Insert into reservations (username, od, do, date, flight_id, numOfSeats, status, remainingSeats) values(@username, @from, @to, @date, @flightID, @seats, @status, @remainingSeats);";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@username", HttpContext.Session.GetString("username"));
                            flight.Username = HttpContext.Session.GetString("username");
                            command.Parameters.AddWithValue("@from", flight.From);

                            command.Parameters.AddWithValue("@to", flight.To);
                            command.Parameters.AddWithValue("@date", flight.Date);
                            flight.Date = Convert.ToDateTime(flight.Date);
                            command.Parameters.AddWithValue("@flightID", flight.Id);
                            command.Parameters.AddWithValue("@seats", Convert.ToInt32(seats));
                            flight.Seats = seats;
                            command.Parameters.AddWithValue("@status", "Pending");
                            flight.Status = "Pending";
                            command.Parameters.AddWithValue("@remainingSeats", flight.Capacity);
                            command.ExecuteNonQuery();

                        }

                    }
                    else
                    {
                        return;

                    }


                }
            }
            catch (Exception ex)
            {
                errorMessage=ex.Message;
                return;

            }
           //call the hub to alert agents
           
            successMessage = "You booked the flight";
            //OnGet();
            
            await _hubContext.Clients.All.SendAsync("SendRequest", flight);
            return;
        }
    }
}
