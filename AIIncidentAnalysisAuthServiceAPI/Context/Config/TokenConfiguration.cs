using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIIncidentAnalysisAuthServiceAPI.Context.Config;

public class TokenConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.TokenValue);
        builder.Property(t => t.TokenRevoked);
        builder.Property(t => t.TokenExpired);
        
        builder.HasOne(t => t.PoliceOfficer)
            .WithMany(u => u.Tokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(t => t.ETokenType)
            .HasConversion(
                v => v.ToString(),
                v => (ETokenType)Enum.Parse(typeof(ETokenType), v))
            .IsRequired();
    }
}