using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appointment.Infrastructure.Repositories
{
    public class AvailabilityRepository : IAvailabilityRepository
    {
        private readonly AppDbContext _context;

        public AvailabilityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Availability> Update(Availability availability)
        {
            _context.Availabilities.Update(availability);
            await _context.SaveChangesAsync();
            return availability;
        }
        public async Task<Availability> Insert(Availability availability)
        {
            await _context.Availabilities.AddAsync(availability);
            await _context.SaveChangesAsync();
            return availability;
        }

        public async Task<IEnumerable<Availability>> Insert(IEnumerable<Availability> availabilities)
        {
            await _context.Availabilities.AddRangeAsync(availabilities);
            await _context.SaveChangesAsync();
            return availabilities;
        }

        public async Task<Availability> GetById(int id)
            => await _context.Availabilities.Where(
                    a => a.Id == id
                )
                .FirstOrDefaultAsync();

        public async Task<Availability> GetByAppointmentId(int appointmentId)
            => await _context.Availabilities.Where(
                    a => a.AppointmentId == appointmentId
                )
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<AvailabilityDto>> GetByFilter(int hostId, DateTime from, DateTime to, bool showOnlyAvailable)
        => await _context.Availabilities.Where(
                    a => a.HostId == hostId
                                 && a.DateOfAvailability >= from.ToLocalTime()
                                 && a.DateOfAvailability <= to.ToLocalTime()
                                 && a.IsEmpty == (showOnlyAvailable ? showOnlyAvailable : a.IsEmpty)
                )
                .OrderBy(a => a.DateOfAvailability)
                .Select(a =>

                    new AvailabilityDto()
                    {
                        HostId = a.HostId,
                        Id = a.Id,
                        AmountOfTime = a.AmountOfTime,
                        DateOfAvailability = a.DateOfAvailability.ToUniversalTime(),
                        IsEmpty = a.IsEmpty,
                        AppointmentId = a.AppointmentId,
                        AppointmentWith = a.AppointmentWith,
                    }
                )
                .ToListAsync();

        public async Task<IEnumerable<Availability>> GetOverlapped(int hostId, DateTime from, DateTime to)
            => await _context.Availabilities.Where(
                a => a.HostId == hostId
                     && (a.DateOfAvailability >= from
                         || a.DateOfAvailability.AddMinutes(a.AmountOfTime - 1) >= from)
                     && (a.DateOfAvailability < to
                        || a.DateOfAvailability.AddMinutes(a.AmountOfTime - 1) < to)
            ).ToListAsync();

        public async Task Delete(IEnumerable<int> ids)
        {
            _context.Availabilities.RemoveRange(
                await _context.Availabilities.Where(
                        x => ids.Contains(x.Id)
                    )
                    .ToListAsync());
            await _context.SaveChangesAsync();
        }

    }
}