using System;
using System.Collections.Generic;
using System.Text;

namespace Appointment.Domain.Infrastructure
{
    public class ConnectionOptions
    {
        public static readonly string SECTION = "ConnectionStrings";
        public string Database { get; set; }
    }
}
