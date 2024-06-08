using MySqlConnector;

namespace OutOfOffice.Server.Config;

public class DatabaseConfiguration : BaseConfiguration
{
    public string Server { get; private init; }
    public string User { get; private init; }
    internal string Password { get; private init; }
    public string DatabaseName { get; private init; }
    public DatabaseConfiguration(IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection("Database");
        Server = section.GetRequiredSection("Server").Get<string?>() ?? throw GetSimpleMissingException("Server");
        User = section.GetRequiredSection("User").Get<string?>() ?? throw GetSimpleMissingException("User");
        Password = section.GetRequiredSection("Password").Get<string?>() ?? throw GetSimpleMissingException("Password");
        DatabaseName = section.GetRequiredSection("DatabaseName").Get<string?>() ?? throw GetSimpleMissingException("DatabaseName");
    }

    public string GetConnectionString()
    {
        var connectionStringBuilder = new MySqlConnectionStringBuilder()
        {
            Server = Server,
            UserID = User,
            Password = Password,
            Database = DatabaseName
        };
        return connectionStringBuilder.ConnectionString;
    }
}