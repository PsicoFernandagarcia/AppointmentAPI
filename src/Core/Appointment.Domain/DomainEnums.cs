namespace Appointment.Domain
{
    public enum AppointmentStatus
    {
        CREATED, UPDATED, CANCELED, FINISHED, DID_NOT_KEEP
    }

    public enum RolesEnum
    {
        HOST = 1, COMMON
    }

    public static class CacheKeys
    {
        public const string UsersPolicy = "Users";
        public const string Users = "users";

        public const string PaymentsPolicy = "Payments";
        public const string Payments = "payments";

        public const string AvailabilitiesPolicy = "Availabilities";
        public const string Availabilities = "availabilities";

        public const string AppointmentsPolicy = "AppointmentsPolicy";
        public const string Appointments = "appointments";
    }
}
