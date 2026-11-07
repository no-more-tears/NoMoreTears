namespace NoMoreTears.Shared.RabbitMq;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMqSettings";

    public string HostName { get; set; } = "localhost";

    public int Port { get; set; } = 5672;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string VirtualHost { get; set; } = "/";
}