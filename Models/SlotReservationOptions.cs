namespace PortHub.Api.Models
{
    public class SlotReservationOptions
    {
        public int TimeoutMinutes { get; set; } = 15;

        public bool AutoCleanupEnabled { get; set; } = true;

        public int CleanupIntervalMinutes { get; set; } = 5;
    }
}