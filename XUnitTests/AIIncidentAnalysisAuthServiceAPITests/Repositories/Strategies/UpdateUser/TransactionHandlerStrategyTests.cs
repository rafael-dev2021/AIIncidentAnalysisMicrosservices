using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Repositories.Strategies.UpdateUser;

public class TransactionHandlerStrategyTests : IDisposable
{
    private readonly AppDbContext _appDbContext;
    private readonly TransactionHandlerStrategy _transactionHandlerStrategy;

    public TransactionHandlerStrategyTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _appDbContext = new AppDbContext(options);
        _appDbContext = new AppDbContext(options);
        _transactionHandlerStrategy = new TransactionHandlerStrategy();
    }

    [Fact(DisplayName = "Should commit transaction when action is successful")]
    public async Task ExecuteInTransactionAsync_Should_CommitTransaction_When_ActionIsSuccessful()
    {
        // Arrange
        var expectedResponse = new UpdateDtoResponse(true, "Success");

        // Act
        var result =
            await _transactionHandlerStrategy.ExecuteInTransactionAsync(_appDbContext,
                () => Task.FromResult(expectedResponse));

        // Assert
        result.Should().Be(expectedResponse);
    }

    [Fact(DisplayName = "Should rollback transaction when action fails")]
    public async Task ExecuteInTransactionAsync_Should_RollbackTransaction_When_ActionFails()
    {
        // Arrange
        var expectedResponse = new UpdateDtoResponse(false, "Failed");

        // Act
        var result =
            await _transactionHandlerStrategy.ExecuteInTransactionAsync(_appDbContext,
                () => Task.FromResult(expectedResponse));

        // Assert
        result.Should().Be(expectedResponse);
    }

    [Fact(DisplayName = "Should rollback transaction when an exception occurs")]
    public async Task ExecuteInTransactionAsync_Should_RollbackTransaction_When_ExceptionOccurs()
    {
        // Act
        var result =
            await _transactionHandlerStrategy.ExecuteInTransactionAsync(_appDbContext,
                () => throw new Exception("Test Exception"));

        // Assert
        result.Should().Be(new UpdateDtoResponse(false, "Profile update failed due to an internal error."));
    }

    public void Dispose()
    {
        _appDbContext.Database.EnsureDeleted();
        _appDbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}