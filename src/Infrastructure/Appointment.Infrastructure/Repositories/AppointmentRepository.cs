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
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Domain.Entities.Appointment> Create(Domain.Entities.Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }
        public async Task<Domain.Entities.Appointment> Update(Domain.Entities.Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<Domain.Entities.Appointment> GetById(int id)
            => await _context.Appointments.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<AppointmentDto>> GetByUserId(int id, DateTime dateFrom)
            => await _context.Appointments
                .Where(a => a.DateFrom >= dateFrom && (a.HostId == id || a.PatientId == id))
                .OrderBy(a => a.DateFrom)
                .Select(a => new AppointmentDto()
                {
                    Id = a.Id,
                    DateFrom = a.DateFrom.ToUniversalTime(),
                    DateTo = a.DateTo.ToUniversalTime(),
                    With = a.With,
                    HostId = a.HostId,
                    HostName = $"{a.Host.LastName} {a.Host.Name}",
                    Color = a.Color,
                    CreatedById = a.CreatedById,
                    CreatedBy = a.CreatedBy.UserName,
                    PatientId = a.PatientId,
                    PatientName = $"{a.Patient.LastName} {a.Patient.Name} ",
                    IsDeleted = a.IsDeleted,
                    Status = a.Status.ToString(),
                    Title = a.Title,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync();

        public async Task<IEnumerable<AppointmentYearInformationDto>> GetYearInformation(int year, int hostId, int? patientId)
        {
            var info = await _context.Appointments
                .Where(a =>
                            a.HostId == hostId
                            && (patientId == null || a.PatientId == patientId)
                            && a.DateTo.Year == year
                )
                .GroupBy(a => new { a.DateFrom.Month })
                .Select(a => new AppointmentYearInformationDto
                {
                    Month = a.Key.Month,
                    TotalAppointments = a.Count(),
                    TotalCanceled = a.Where(ap => ap.Status == Domain.AppointmentStatus.CANCELED).Count()
                }).ToListAsync();
            return info;
        }

        public async Task<IEnumerable<AppointmentDto>> GetByFilter(int year, int userId)
            => await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => (
                                a.HostId == userId
                                || a.PatientId == userId
                            )
                            && a.DateTo.Year == year
                )
                .OrderBy(a => a.Patient.LastName)
                .ThenByDescending(a => a.DateFrom)
                .Select(a => new AppointmentDto()
                {
                    Id = a.Id,
                    DateFrom = a.DateFrom.ToUniversalTime(),
                    DateTo = a.DateTo.ToUniversalTime(),
                    With = a.With,
                    HostId = a.HostId,
                    HostName = $"{a.Host.LastName} {a.Host.Name}",
                    Color = a.Color,
                    CreatedById = a.CreatedById,
                    CreatedBy = a.CreatedBy.UserName,
                    PatientId = a.PatientId,
                    PatientName = $"{a.Patient.LastName} {a.Patient.Name} ",
                    IsDeleted = a.IsDeleted,
                    Status = a.Status.ToString(),
                    Title = a.Title,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync();

        public async Task FinalizeAppointments()
        {
            var sqlCommand = @" UPDATE AppointmentDb.Appointments 
                                SET Status = 3 
                                WHERE   DateTo < CURDATE() 
                                        AND Status = 0
                            ";
            await this._context.Database.ExecuteSqlRawAsync(sqlCommand);
        }

        public async Task<bool> HasAnyAppointment(int patientId, int hostId)
            => await _context.Appointments.AnyAsync(a => a.PatientId == patientId && a.HostId == hostId);

        public async Task<IEnumerable<AppointmentDto>> GetLastAppointments(int hostId, int patientId, int totalCount)
            => await _context.Appointments
                .Where(a => a.HostId >= hostId &&
                            a.PatientId == patientId
                            && a.Status != Domain.AppointmentStatus.CANCELED
                )
                .OrderByDescending(a => a.DateFrom)
                .Take(Math.Abs(totalCount))
                .Select(a => new AppointmentDto()
                {
                    Id = a.Id,
                    DateFrom = a.DateFrom.ToUniversalTime(),
                    DateTo = a.DateTo.ToUniversalTime(),
                    With = a.With,
                    HostId = a.HostId,
                    HostName = $"{a.Host.LastName} {a.Host.Name}",
                    Color = a.Color,
                    CreatedById = a.CreatedById,
                    CreatedBy = a.CreatedBy.UserName,
                    PatientId = a.PatientId,
                    PatientName = $"{a.Patient.LastName} {a.Patient.Name} ",
                    IsDeleted = a.IsDeleted,
                    Status = a.Status.ToString(),
                    Title = a.Title,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync();

        public async Task<int> ReassignAppointments(int userFrom, int userTo)
            => await _context.Appointments
                                .Where(a => a.PatientId == userFrom)
                                .ExecuteUpdateAsync(a => a.SetProperty(ap => ap.PatientId, userTo));

    }
}