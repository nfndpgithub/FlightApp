using CSCL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.Data.SqlClient;
using vezba.Hubs;
using vezba.Model;


namespace vezba.Pages.Reservation
{
    public class AgnetSideModel : PageModel
    {
        public List<Flight> listFlights = new List<Flight>();
        public string errorMessage = "";
        public string username;
        public string role;
        private readonly IHubContext<ComHub> _hubContext;

        public AgnetSideModel(IHubContext<ComHub> hubContext)
        {
            _hubContext = hubContext;
        }
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
                    //username = HttpContext.Session.GetString("username");
                    //role = HttpContext.Session.GetString("role");
                    SqlCommand cmd = new SqlCommand("select * from reservations ", connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {

                            Flight flight = new Flight();
                            flight.From = dr.GetValue(2).ToString();
                            flight.To = dr.GetValue(3).ToString();
                            flight.Date = Convert.ToDateTime(dr.GetValue(4));
                            flight.Capacity =Convert.ToInt32(dr.GetValue(8));
                            var seats = dr.GetValue(6).ToString();
                            var status = dr.GetValue(7).ToString();
                            var username = dr.GetValue(1).ToString();
                            flight.Seats = seats;
                            flight.Status = status;
                            flight.Username = username;
                            flight.Id = Convert.ToInt32(dr.GetValue(0));

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
            }
        }

        public async void OnPostAnswer()
        {
            string id = Request.Form["id"];
            string seats= Request.Form["seats"];
            string remainingSeats = "";
              bool answer=false;
            string flightId = "";

            try
            {
                ConnectionStringManager cs = new ConnectionStringManager();
                var connectionString = cs.GetConnectionString();


                SqlConnection connection = new SqlConnection(connectionString);

                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                if (Request.Form["accept"] == "accept")
                {
                    SqlCommand cmd = new SqlCommand("select * from reservations where id='" + id + "'", connection);
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            remainingSeats = dr.GetValue(8).ToString();
                            flightId = dr.GetValue(5).ToString(); 

                        }
                        
                    }
                    else
                    {
                        errorMessage = "invalid credentials";
                        return;

                    }
                    dr.Close();
                    var newRemaingingSeats = Convert.ToInt32(remainingSeats) -Convert.ToInt32( seats);
                    if (newRemaingingSeats < 0)
                    {
                        errorMessage = "There is no enough free seats";
                        return;
                    }
                    cmd = new SqlCommand("update reservations set status='Accepted', remainingSeats ='"+newRemaingingSeats+"' where id='"+id+"'", connection);
                    dr = cmd.ExecuteReader();
                    dr.Close();
                    cmd = new SqlCommand("update flights set capacity='"+ newRemaingingSeats +"'  where id='" + flightId + "'", connection);
                    dr = cmd.ExecuteReader();
                    answer = true;
                    await _hubContext.Clients.All.SendAsync("Answer", id, answer);


                }
                else
                {
                    SqlCommand cmd = new SqlCommand("update reservations set status='Declined' where id='" + id + "'", connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Close();
                    answer = false;
                    await _hubContext.Clients.All.SendAsync("Answer", id, answer);

                }



            }   
            catch(Exception ex)
            {

            }
            OnGet();
            

        }

		
	}

    
}

