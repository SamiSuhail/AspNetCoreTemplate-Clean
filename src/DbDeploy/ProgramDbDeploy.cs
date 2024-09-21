using MyApp.DbDeploy;

string connectionString = Helpers.GetConnectionStringFromFile();

#if DEBUG
Helpers.DropPostgresqlDatabase(connectionString);
#endif

var result = Helpers.DeployDatabase(connectionString);

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();
#if DEBUG
    Console.ReadLine();
#endif
    return -1;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Success!");
Console.ResetColor();
return 0;

public partial class ProgramDbDeploy { }