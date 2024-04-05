using Microsoft.EntityFrameworkCore;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;

namespace PassIn.Application.UseCases.Attendees.GetAllByEventId;

public class GetAllByAttendessByEventIdUseCase
{
    private readonly PassInDbContext _dbContext;

    public GetAllByAttendessByEventIdUseCase()
    {
        _dbContext = new PassInDbContext();
    }
    public ResponseAllAttendeesJson Execute(Guid eventId)
    {
        var entity = _dbContext.Events.Include(ev => ev.Attendees)
            .ThenInclude(at => at.CheckIn)
            .FirstOrDefault(ev => ev.Id == eventId);

        if (entity is null)
            throw new NotFoundException("An event with his id dont exist.");

        return new ResponseAllAttendeesJson
        {
            Attendees = entity.Attendees.Select(at => new ResponseAttendeeJson
            {
                Id = at.Id,
                Name = at.Name,
                Email = at.Email,
                CreatedAt = at.Created_At,
                CheckedInAt = at.CheckIn?.Created_at,
            }).ToList(),
        };
    }
}
