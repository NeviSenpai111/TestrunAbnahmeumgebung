namespace TestrunAbnahmeumgebung.Models
{
    public class Ping
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; } = "Pong";
    }
}
