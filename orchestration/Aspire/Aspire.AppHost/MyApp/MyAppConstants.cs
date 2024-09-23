namespace Aspire.AppHost.MyApp;

public static class MyAppConstants
{
    public static class Services
    {
        public const string DatabaseServer = "MyAppDbServer";
        public const string Database = "MyAppDb";
        public const string DbDeploy = "MyAppDbDeploy";
        public const string Queue = "MyAppQueue";
        public const string Api = "MyAppApi";
    }

    public static class Database
    {
        public const string NameParameter = "MyAppDbName";
        public const string UsernameParameter = "MyAppDbUsername";
        public const string PasswordParameter = "MyAppDbPassword";
        public const string PortParameter = "MyAppDbPort";
    }

    public static class Queue
    {
        public const string UsernameParameter = "MyAppQueueUsername";
        public const string PasswordParameter = "MyAppQueuePassword";
        public const string PortParameter = "MyAppQueuePort";
        public const string ManagementUiPortParameter = "MyAppQueueManagementUiPort";
    }
}