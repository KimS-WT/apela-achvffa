using Microsoft.Data.Sqlite;
using System.Globalization;

// This tool reads the SQLite file created by the web app and prints recent EmailLog entries.
var dbPath = @"c:\Users\kimsu\StudyHall\VSCodeAIProjects\ChoctawGivingCircle-Achvffa\ChoctawGivingCircle\app.db";
if (!File.Exists(dbPath))
{
    Console.WriteLine($"Database not found at: {dbPath}");
    return;
}

using var conn = new SqliteConnection($"Data Source={dbPath}");
conn.Open();

var cmd = conn.CreateCommand();
cmd.CommandText = @"SELECT Id, RecipientEmail, EmailType, Subject, CreatedAt
FROM EmailLogs
ORDER BY CreatedAt DESC
LIMIT 20;";

using var reader = cmd.ExecuteReader();
Console.WriteLine("Recent EmailLogs (top 20):\n");
while (reader.Read())
{
    var id = reader.GetInt32(0);
    var recip = reader.IsDBNull(1) ? "(null)" : reader.GetString(1);
    var type = reader.IsDBNull(2) ? "(null)" : reader.GetString(2);
    var subj = reader.IsDBNull(3) ? "(no subject)" : reader.GetString(3);
    var created = reader.IsDBNull(4) ? "(unknown)" : reader.GetDateTime(4).ToLocalTime().ToString("g", CultureInfo.CurrentCulture);

    Console.WriteLine($"[{id}] {created} | {type} | To: {recip}\n  Subject: {subj}\n");
}

conn.Close();
