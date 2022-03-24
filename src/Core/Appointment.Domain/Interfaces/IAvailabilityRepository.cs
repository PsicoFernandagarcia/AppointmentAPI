using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appointment.Domain.Entities;

namespace Appointment.Domain.Interfaces
{
    public interface IAvailabilityRepository
    {
        Task<Availability> Update(Availability availability);
        Task<Availability> Insert(Availability availability);
        Task<IEnumerable<Availability>> Insert(IEnumerable<Availability> availabilities);
        Task<Availability> GetById(int id);
        Task<IEnumerable<AvailabilityDto>> GetByFilter(int hostId, DateTime from, DateTime to,bool showOnlyAvailable);
        Task<IEnumerable<Availability>> GetOverlapped(int hostId, DateTime from, DateTime to);
        Task Delete(IEnumerable<int> ids);
    }
}