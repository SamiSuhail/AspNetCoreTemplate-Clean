namespace MyApp.Server.Infrastructure.Database;

internal static class Constants
{
    internal static class Schemas
    {
        public const string Auth = "auth";
        public const string Profile = "profile";
    }

    internal static class Tables
    {
        public const string Users = "users";
        public const string EmailConfirmations = "email_confirmations";
        public const string PasswordResetConfirmations = "password_reset_confirmations";
        public const string Members = "members";
        public const string Drivers = "drivers";
        public const string Passengers = "passengers";
        public const string VehicleDrivers = "vehicle_drivers";
        public const string Vehicles = "vehicles";
    }
}
