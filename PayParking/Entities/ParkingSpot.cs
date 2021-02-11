namespace PayParking.Entities
{
    public class ParkingSpot
    {
        public int ParkingSpotId { get; set; }
        public string ParkingSpotNumber { get; set; }
        public int? CarId { get; set; }
        public Car Car { get; set; }
    }
}
