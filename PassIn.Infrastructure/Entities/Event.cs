namespace PassIn.Infrastructure.Entities;

public class Event
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = String.Empty;
    public string Details { get; set; } = String.Empty;

    public string Slug { get; set; } = String.Empty;

    public int Maximum_Attendees { get; set; }
}
