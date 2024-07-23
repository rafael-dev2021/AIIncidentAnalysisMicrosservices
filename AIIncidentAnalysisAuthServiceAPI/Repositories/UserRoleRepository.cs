using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Models.Enums;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories;

public class UserRoleRepository(RoleManager<IdentityRole> roleManager, UserManager<PoliceOfficer> userManager)
    : IUserRoleRepository
{
    private const string Admin = "Admin";
    private const string User = "User";
    private const string ReadOnly = "ReadOnly";
    private const string ReadWrite = "ReadWrite";

    public async Task CreateRoleIfNotExistsAsync(string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            IdentityRole role = new()
            {
                Name = roleName,
                NormalizedName = roleName.ToUpper()
            };
            await roleManager.CreateAsync(role);
        }
    }

    public async Task CreateUserIfNotExistsAsync(UserDetails userDetails)
    {
        if (await userManager.FindByEmailAsync(userDetails.Email!) == null)
        {
            var user = new PoliceOfficer
            {
                Email = userDetails.Email,
                UserName = userDetails.Email,
                NormalizedEmail = userDetails.Email!.ToUpper(),
                NormalizedUserName = userDetails.Email.ToUpper(),
                PhoneNumber = userDetails.PhoneNumber,
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            user.SetIdentificationNumber(userDetails.IdentificationNumber);
            user.SetName(userDetails.Name);
            user.SetLastName(userDetails.LastName);
            user.SetBadgeNumber(userDetails.BadgeNumber);
            user.SetCpf(userDetails.Cpf);
            user.SetRole(userDetails.Role);
            user.SetDateOfBirth(userDetails.DateOfBirth);
            user.SetDateOfJoining(userDetails.DateOfJoining);
            user.SetERank(userDetails.ERank);
            user.SetEDepartment(userDetails.EDepartment);
            user.SetEOfficerStatus(userDetails.EOfficerStatus);
            user.SetEAccessLevel(userDetails.EAccessLevel);

            var result = await userManager.CreateAsync(user, "@Visual24k+");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, userDetails.Role!);
            }
        }
    }

    public async Task UserAsync()
    {
        var readOnlyUserDetails = new UserDetails
        {
            IdentificationNumber = "R12345",
            Name = ReadOnly,
            LastName = ReadOnly,
            BadgeNumber = "R12345",
            Cpf = "123.456.789-12",
            Email = "readonly@localhost.com",
            PhoneNumber = "+5512345687",
            Role = ReadOnly,
            DateOfBirth = new DateTime(2000, 5, 20),
            DateOfJoining = DateTime.Now.AddYears(-1),
            ERank = ERank.Constable,
            EDepartment = EDepartment.HomicideDivision,
            EOfficerStatus = EOfficerStatus.Active,
            EAccessLevel = EAccessLevel.ReadOnly
        };

        var readWriteUserDetails = new UserDetails
        {
            IdentificationNumber = "RW12345",
            Name = ReadWrite,
            LastName = ReadWrite,
            BadgeNumber = "RW12345",
            Cpf = "123.456.789-13",
            Email = "readwrite@localhost.com",
            PhoneNumber = "+5512345681",
            Role = ReadWrite,
            DateOfBirth = new DateTime(2000, 2, 15),
            DateOfJoining = DateTime.Now.AddYears(-2),
            ERank = ERank.Sergeant,
            EDepartment = EDepartment.TrafficDivision,
            EOfficerStatus = EOfficerStatus.Active,
            EAccessLevel = EAccessLevel.ReadWrite
        };

        var adminUserDetails = new UserDetails
        {
            IdentificationNumber = "A12345",
            Name = Admin,
            LastName = Admin,
            BadgeNumber = "A12345",
            Cpf = "123.456.789-14",
            Email = "admin@localhost.com",
            PhoneNumber = "+5512345621",
            Role = Admin,
            DateOfBirth = new DateTime(1990, 2, 15),
            DateOfJoining = DateTime.Now.AddYears(-5),
            ERank = ERank.Captain,
            EDepartment = EDepartment.Administrative,
            EOfficerStatus = EOfficerStatus.Active,
            EAccessLevel = EAccessLevel.Admin
        };

        var regularUserDetails = new UserDetails
        {
            IdentificationNumber = "U12345",
            Name = User,
            LastName = User,
            BadgeNumber = "U12345",
            Cpf = "123.456.789-15",
            Email = "user@localhost.com",
            PhoneNumber = "+5512545621",
            Role = User,
            DateOfBirth = new DateTime(2000, 6, 15),
            DateOfJoining = DateTime.Now.AddYears(-1),
            ERank = ERank.Undefined,
            EDepartment = EDepartment.Undefined,
            EOfficerStatus = EOfficerStatus.Active,
            EAccessLevel = EAccessLevel.Undefined
        };

        await CreateUserIfNotExistsAsync(readOnlyUserDetails);
        await CreateUserIfNotExistsAsync(readWriteUserDetails);
        await CreateUserIfNotExistsAsync(adminUserDetails);
        await CreateUserIfNotExistsAsync(regularUserDetails);
    }

    public async Task RoleAsync()
    {
        await CreateRoleIfNotExistsAsync(Admin);
        await CreateRoleIfNotExistsAsync(User);
        await CreateRoleIfNotExistsAsync(ReadOnly);
        await CreateRoleIfNotExistsAsync(ReadWrite);
    }
}