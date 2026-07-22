namespace IntegrationTests.Shared.Models.Config;

public record TestConfig
{
    public string DatabaseType { get; set; } = "SqlServer";
}