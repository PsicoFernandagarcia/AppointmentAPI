using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain
{
    public enum AppointmentStatus
    {
        CREATED,UPDATED,CANCELED,FINISHED,DID_NOT_KEEP
    }

    public enum RolesEnum
    {
        HOST=1,COMMON
    }
}
