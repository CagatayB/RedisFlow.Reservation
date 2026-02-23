using ReservationSystem.src.Domain.Enums;

namespace ReservationSystem.src.Domain.Entities
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public Guid SlotId { get; set; }
        public Guid UserId { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
