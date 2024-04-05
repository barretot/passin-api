﻿using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using System.Net.Mail;

namespace PassIn.Application.UseCases.Events.RegisterAttendee;

public class RegisterAttendeeOnEventUseCase
{
    private readonly PassInDbContext _dbContext;
    
    public RegisterAttendeeOnEventUseCase()
    {
        _dbContext = new PassInDbContext();
    }

    public ResponseRegisteredJson Execute(Guid eventId, RequestRegisterEventJson request)
    {
        Validate(eventId, request);

        var entity = new Infrastructure.Entities.Attendee
        {
            Email = request.Email,
            Name = request.Name,
            Event_Id = eventId,
            Created_At = DateTime.UtcNow,
        };

        _dbContext.Attendees.Add(entity);
        _dbContext.SaveChanges();

        return new ResponseRegisteredJson
        {
            Id = entity.Id,
        };
    }

    private void Validate(Guid eventId, RequestRegisterEventJson request)
    {
        var eventEntity = _dbContext.Events.Find(eventId);

        if (eventEntity is null)
            throw new NotFoundException("An event with his id dont exist.");

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ErrorOnValidationException("The name is invalid");
        }

        if(!EmailIsValid(request.Email))
        {
            throw new ErrorOnValidationException("The e-mail is invalid");
        }

        var attendeeAlreadyRegistered = _dbContext
            .Attendees
            .Any(e => e.Email.Equals(request.Email) && e.Event_Id == eventId);

        if (attendeeAlreadyRegistered)
        {
            throw new ConflictException("You can not twice on the same event.");
        }

        var attendessForEvent = _dbContext.Attendees.Count(attendee => attendee.Event_Id == eventId);

        if(attendessForEvent > eventEntity.Maximum_Attendees)
        {
            throw new ErrorOnValidationException("There is no room for this event.");
        }

    }

    private bool EmailIsValid(string email)
    {
        try
        {
            new MailAddress(email);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
