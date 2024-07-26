using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser.Interfaces;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser;

public class TransactionHandlerStrategy : ITransactionHandlerStrategy
{
    public async Task<UpdateDtoResponse> ExecuteInTransactionAsync(AppDbContext appDbContext, Func<Task<UpdateDtoResponse>> action)
    {
        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await action();
            if (!result.Success)
            {
                await transaction.RollbackAsync();
                return result;
            }

            await transaction.CommitAsync();
            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return new UpdateDtoResponse(false, "Profile update failed due to an internal error.");
        }
    }
}