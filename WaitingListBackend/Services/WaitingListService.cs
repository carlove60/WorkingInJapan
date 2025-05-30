using MySqlX.XDevAPI.Common;
using WaitingListBackend.Entities;
using WaitingListBackend.Interfaces;

namespace WaitingListBackend.Services;

/// <summary>
/// 
/// </summary>
public class WaitingListService : IWaitingListService
{
    private readonly IWaitingListRepository _waitingListRepository;
    private readonly IPartyRepository _partyRepository;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="waitingListRepository"></param>
    /// <param name="partyRepository"></param>
    public WaitingListService(IWaitingListRepository waitingListRepository, IPartyRepository partyRepository)
    {
        _waitingListRepository = waitingListRepository;
        _partyRepository = partyRepository;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="partyEntity"></param>
    /// <param name="waitingListId"></param>
    /// <returns></returns>
    public ResultObject<WaitingListEntity> AddPartyToWaitingList(PartyEntity partyEntity, Guid waitingListId)
    {
        var response = new ResultObject<WaitingListEntity>();
        
        var party = _partyRepository.AddParty(partyEntity);
        if (party.IsError)
        {
            response.Messages.AddRange(party.Messages);
            return response;       
        }

        // Ensure that multiple parties don't try to make a request at the same time and both reserve (a) seat(s)
        lock (this._waitingListRepository)
        {
           var waitingListResult =  _waitingListRepository.GetWaitingList(partyEntity.WaitingListEntity.Id);
           if (waitingListResult.Messages.Count == 0 && waitingListResult.Records.Count > 0)
           {
               var waitingList = waitingListResult.Records.First();
               waitingList.Parties.Add(party.Records.First());
           }
        }
        
        return response;
    }

    public ResultObject<WaitingListEntity> CheckIn(Guid waitingListId, Guid partyId)
    {
        throw new NotImplementedException();
    }

    public ResultObject<WaitingListEntity> GetWaitingList(Guid id)
    {
        throw new NotImplementedException();
    }
    
    public ResultObject<WaitingListEntity> GetWaitingList(string name = Constants.DefaultWaitingListName)
    {
        var result = new ResultObject<WaitingListEntity>();
        return _waitingListRepository.GetWaitingList(name);
       
    }

    public ResultObject<WaitingListEntity> GetMetaData()
    {
        return _waitingListRepository.GetWaitingList();
    }
}