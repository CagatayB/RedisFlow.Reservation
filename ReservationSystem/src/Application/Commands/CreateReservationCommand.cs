namespace ReservationSystem.src.Application.Commands
{
    public class CreateReservationCommand
    {
        public Guid SlotId { get; set; }
        public Guid UserId { get; set; }
    }
}
