namespace AIIncidentAnalysisAuthServiceAPI.Extensions;

public static class PoliciesExtensions
{
    private const string Admin = "Admin";

    public static void AddPoliciesExtensions(this IServiceCollection service)
    {
        service.AddAuthorizationBuilder()
            .AddPolicy(Admin, policy => { policy.RequireRole(Admin); })
            .AddPolicy("ReadOnly", policy => { policy.RequireRole("ReadOnly", Admin); })
            .AddPolicy("ReadWrite", policy => { policy.RequireRole("ReadWrite", Admin); });
    }
}