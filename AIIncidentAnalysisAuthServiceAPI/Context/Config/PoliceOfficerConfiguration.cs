using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIIncidentAnalysisAuthServiceAPI.Context.Config;

public class PoliceOfficerConfiguration : IEntityTypeConfiguration<PoliceOfficer>
{
    public void Configure(EntityTypeBuilder<PoliceOfficer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(15).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(15).IsRequired();
        builder.Property(x => x.DateOfBirth).IsRequired().HasColumnType("datetime2");
        builder.Property(x => x.DateOfJoining).IsRequired().HasColumnType("datetime2");

        builder.Property(x => x.IdentificationNumber).HasMaxLength(25).IsRequired();
        builder.HasIndex(x => x.IdentificationNumber).IsUnique();

        builder.Property(x => x.BadgeNumber).HasMaxLength(10).IsRequired();
        builder.HasIndex(x => x.BadgeNumber).IsUnique();

        builder.Property(x => x.Cpf).HasMaxLength(14).IsRequired();
        builder.HasIndex(e => e.Cpf).IsUnique();

        builder.Property(x => x.Email).HasMaxLength(50).IsRequired();
        builder.HasIndex(e => e.Email).IsUnique();

        builder.Property(x => x.PhoneNumber).HasMaxLength(15).IsRequired();
        builder.HasIndex(e => e.PhoneNumber).IsUnique();
        
        builder.Property(t => t.ERank)
            .HasConversion(
                v => v.ToString(),
                v => (ERank)Enum.Parse(typeof(ERank), v))
            .IsRequired();
        
        builder.Property(t => t.EDepartment)
            .HasConversion(
                v => v.ToString(),
                v => (EDepartment)Enum.Parse(typeof(EDepartment), v))
            .IsRequired();
        
        builder.Property(t => t.EOfficerStatus)
            .HasConversion(
                v => v.ToString(),
                v => (EOfficerStatus)Enum.Parse(typeof(EOfficerStatus), v))
            .IsRequired();
        
        builder.Property(t => t.EAccessLevel)
            .HasConversion(
                v => v.ToString(),
                v => (EAccessLevel)Enum.Parse(typeof(EAccessLevel), v))
            .IsRequired();
        
        builder.HasMany(u => u.Tokens)
            .WithOne(t => t.PoliceOfficer)
            .HasForeignKey(t => t.UserId);
    }
}