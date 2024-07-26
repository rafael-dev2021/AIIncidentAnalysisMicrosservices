using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser.Interfaces;

public interface ITransactionHandlerStrategy
{
    Task<UpdateDtoResponse> ExecuteInTransactionAsync(AppDbContext appDbContext, Func<Task<UpdateDtoResponse>> action);
}