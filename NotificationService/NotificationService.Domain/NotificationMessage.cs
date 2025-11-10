namespace NotificationService.Domain
{
    public class NotificationMessage
    {
        public string Channel { get; set; } = "email";
        public string To { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
