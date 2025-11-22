namespace labka3
{
    public class Apartment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string City { get; set; }
        public string Street { get; set; }
        public int Rooms { get; set; }
        public double Area { get; set; }
        public decimal Price { get; set; }
        public int Floor { get; set; }
        public bool IsRenovated { get; set; }
    }
}