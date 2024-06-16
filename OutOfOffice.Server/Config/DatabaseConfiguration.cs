using MySqlConnector;
using OutOfOffice.Server.Core.Extensions;

namespace OutOfOffice.Server.Config;

public class DatabaseConfiguration
{
    public string Server { get; }
    public string User { get; }
    internal string Password { get; }
    public string DatabaseName { get; }
    public DatabaseConfiguration(IConfiguration configuration)
    {
        var section = configuration.GetSectionOrThrow("Database");

        Server = section.GetOrThrow<string>("Server");
        User = section.GetOrThrow<string>("User");
        Password = section.GetOrThrow<string>("Password");
        DatabaseName = section.GetOrThrow<string>("DatabaseName");
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