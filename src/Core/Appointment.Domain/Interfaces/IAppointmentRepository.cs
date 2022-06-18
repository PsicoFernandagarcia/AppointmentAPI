using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appointment.Domain.Entities;

namespace Appointment.Domain.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Entities.Appointment> Create(Entities.Appointment appointment);
        Task<Entities.Appointment> Update(Entities.Appointment appointment);
        Task<IEnumerable<AppointmentDto>> GetByUserId(int id, DateTime dateFrom);
        Task<Entities.Appointment> GetById(int id);
        Task<IEnumerable<AppointmentDto>> GetByFilter(int year, int userId);
        Task FinalizeAppointments();
    }
}