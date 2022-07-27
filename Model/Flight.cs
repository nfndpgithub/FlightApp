namespace vezba.Model
{
    public class Flight
    {
        int id;
        string from;
        string to;
        DateTime date;
        int stops;
        int capacity;
        string seats;
        string status;
        string username;

        public int Id { get => id; set => id = value; }
        public string From { get => from; set => from = value; }
        public string To { get => to; set => to = value; }
        public DateTime Date { get => date; set => date = value; }
        public int Stops { get => stops; set => stops = value; }
        public int Capacity { get => capacity; set => capacity = value; }
        public string Status { get => status; set => status = value; }
        public string Seats { get => seats; set => seats = value; }
        public string Username { get => username; set => username = value; }
    }
}
