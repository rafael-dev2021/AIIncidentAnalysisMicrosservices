using AIIncidentAnalysisAuthServiceAPI.Algorithms;
using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Algorithms;

public class AccountNumberGeneratorTests
{
    private static DbContextOptions<AppDbContext> CreateNewDbContextOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GenerateIdentificationNumberAsync_Should_Return_Unique_Identification_Number()
    {
        // Arrange
        var dbContextOptions = CreateNewDbContextOptions();

        await using var context = new AppDbContext(dbContextOptions);
        var generator = new AccountNumberGenerator(context);

        var existingUser = new PoliceOfficer();
        existingUser.SetIdentificationNumber("ABC123");
        existingUser.SetName("John Doe");
        existingUser.SetLastName("John Doe");
        existingUser.SetBadgeNumber("ABC123");
        existingUser.SetPhoneNumber("+5512345656");
        existingUser.SetCpf("123.566.234-23");
        existingUser.SetEmail("test@localhost.com");

        await context.Users.AddAsync(existingUser);
        await context.SaveChangesAsync();

        // Act
        var identificationNumber = await generator.GenerateIdentificationNumberAsync();

        // Assert
        Assert.NotEqual(existingUser.IdentificationNumber, identificationNumber);

        var isUnique = !await context.Users.AnyAsync(x => x.IdentificationNumber == identificationNumber);
        Assert.True(isUnique);
    }

    [Fact]
    public async Task GenerateBadgeNumberAsync_Should_Return_Unique_Badge_Number()
    {
        // Arrange
        var dbContextOptions = CreateNewDbContextOptions();

        await using var context = new AppDbContext(dbContextOptions);
        var generator = new AccountNumberGenerator(context);

        var existingUser = new PoliceOfficer();
        existingUser.SetIdentificationNumber("ABC123");
        existingUser.SetName("John Doe");
        existingUser.SetLastName("John Doe");
        existingUser.SetBadgeNumber("XYZ789");
        existingUser.SetPhoneNumber("+5512345656");
        existingUser.SetCpf("123.566.234-23");
        existingUser.SetEmail("test@localhost.com");

        await context.Users.AddAsync(existingUser);
        await context.SaveChangesAsync();

        // Act
        var badgeNumber = await generator.GenerateBadgeNumberAsync();

        // Assert
        Assert.NotEqual(existingUser.BadgeNumber, badgeNumber);

        var isUnique = !await context.Users.AnyAsync(x => x.BadgeNumber == badgeNumber);
        Assert.True(isUnique);
    }
}