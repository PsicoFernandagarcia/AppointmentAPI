using Appointment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appointment.Domain.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Entities.Appointment> Create(Entities.Appointment appointment);
        Task<Entities.Appointment> Update(Entities.Appointment appointment);
        Task<IEnumerable<AppointmentDto>> GetByUserId(int id, DateTime dateFrom);
        Task<Entities.Appointment> GetById(int id);
        Task<IList<Domain.Entities.Appointment>> GetByIds(IEnumerable<int> ids);
        Task<IEnumerable<AppointmentDto>> GetByFilter(int? year, int userId, bool? isUnpaid);
        Task FinalizeAppointments();
        Task<bool> HasAnyAppointment(int patientId, int hostId);
        Task<IEnumerable<AppointmentYearInformationDto>> GetYearInformation(int year, int hostId, int? patientId);
        Task<IEnumerable<AppointmentDto>> GetLastAppointments(int hostId, int patientId, int totalCount);
        Task<int> ReassignAppointments(int userFrom, int userTo);
    }
}