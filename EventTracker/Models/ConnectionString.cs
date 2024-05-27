using System.Data.SqlClient;
using System.Security;

namespace EventTracker.Models;
public class ConnectionString
{
    public string ApplicationName { get; set; }
    public string DatabaseName { get; set; }
    public string Server { get; set; }
    public SqlCredential SqlCredential { get; set; }

    public ConnectionString(string applicationName, string databaseName, string server, SqlCredential sqlCredential)
    {
        ApplicationName = applicationName;
        DatabaseName = databaseName;
        SqlCredential = sqlCredential;
        Server = server;
    }

    internal bool IsValid() =>
        !string.IsNullOrWhiteSpace(ApplicationName) &&
        !string.IsNullOrWhiteSpace(DatabaseName) &&
        !string.IsNullOrWhiteSpace(Server) &&
        SqlCredential is not null;

    public override string ToString()
    {
        return $"Application Name={ApplicationName};Server={Server};Database={DatabaseName};MultipleActiveResultSets=true;User Id={SqlCredential.UserId};Password={SqlCredential.Password};Trusted_Connection=Yes;";
    }

    public static SecureString ToSecureString(string input)
    {
        var secure = new SecureString();
        foreach (char c in input)
        {
            secure.AppendChar(c);
        }
        secure.MakeReadOnly();
        return secure;
    }

}
