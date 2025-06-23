using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WaitingList.Database.Database;
using WaitingList.Database.Entities;
using WaitingList.SseManager.Managers;
using WaitingListBackend.Interfaces;
using WaitingListBackend.Models;

namespace WaitingListBackend.Repositories;

/// <summary>
/// Provides methods for managing and interacting with party data in the database.
/// </summary>
public class PartyRepository(ApplicationDbContext applicationDbContext, SseChannelManager sseChannelManager) : BaseRepository(applicationDbContext, sseChannelManager), IPartyRepository
{
    /// <summary>
    /// Saves a party entity to the database. If the entity does not exist, it will be added; otherwise, it will be updated.
    /// </summary>
    /// <param name="party">The party entity to be saved.</param>
    /// <returns>A result object that contains the saved party entity and any related messages.</returns>
    public ResultObject<PartyEntity> SaveParty(PartyEntity party)
    {
        var resultObject = new ResultObject<PartyEntity>();
        var validationResult = Validate(party);
        resultObject.Messages.AddRange(validationResult);
        if (resultObject.Messages.Count > 0)
        {
            resultObject.Records.Add(party);
            return resultObject;
        }

        try
        {
            EntityEntry<PartyEntity> partyEntry;
            if (party.Id  == Guid.Empty)
            {
                partyEntry = ApplicationDbContext.Parties.Add(party);
            }
            else
            {
                partyEntry = ApplicationDbContext.Parties.Update(party);
            }

            ApplicationDbContext.SaveChanges();
            resultObject.Records.Add(partyEntry.Entity);
        }
        catch (Exception exception)
        {
            resultObject.Messages.AddError(exception.Message);       
        }

        return resultObject;
    }

    /// <summary>
    /// Removes a party entity from the repository and commits the changes to the database.
    /// Possible to get an exception where the record has already been deleted by the background service
    /// </summary>
    /// <param name="party">The party entity to be removed.</param>
    public ResultObject<PartyEntity> RemoveParty(PartyEntity party)
    {
        var result = new ResultObject<PartyEntity>();
        ApplicationDbContext.Parties.Remove(party);
        try
        {
            ApplicationDbContext.SaveChanges();
            ApplicationDbContext.Entry(party).State = EntityState.Deleted;
            result.Messages.AddSuccess("Removal was successful!");       
        }
        catch (Exception exception)
        {
            result.Messages.AddError(exception.Message);       
        }
        
        return result;   
    }

    /// <summary>
    /// Retrieves a party entity by the given session ID from the database, including its associated waiting list entity.
    /// </summary>
    /// <param name="sessionId">The session ID associated with the party to retrieve.</param>
    /// <returns>A result object containing the matching party entity, if found, along with any associated messages.</returns>
    public ResultObject<PartyEntity> GetParty(string sessionId)
    {
       var result = new ResultObject<PartyEntity>();
       var party = ApplicationDbContext.Parties.Include((x) => x.WaitingListEntity).SingleOrDefault((x) => x.SessionId == sessionId && x.ServiceEndedAt == null);
       if (party != null)
       {
           result.Records.Add(party);
       }
       else
       {
           result.Messages.AddError($"Party with session ID {sessionId} not found. Please check if you used a different browser.");      
       }

       return result;   
    }

    /// <summary>
    /// Validates the provided party entity for required fields and business rules.
    /// </summary>
    /// <param name="party">The party entity to be validated.</param>
    /// <returns>A collection of validation messages indicating errors, warnings, or other relevant information.</returns>
    private MessageList Validate(PartyEntity party)
    {
        var result = new MessageList();
        if (string.IsNullOrWhiteSpace(party.Name))
        {
            result.AddError("Please fill in a name");
        }

        if (party.Size == 0)
        {
            result.AddError("Please fill in a size above 0");       
        }

        return result;   
    }
}