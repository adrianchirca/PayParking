using System;

namespace PayParking.Entities
{
    public class Car
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime EntranceTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public TimeSpan? StationaryPeriod
        {
            get { return (ExitTime - EntranceTime).GetValueOrDefault(); }
        }
    }

}
