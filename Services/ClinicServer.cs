using Microsoft.EntityFrameworkCore;
using BlazorApp2.Data;
using BlazorApp2.Models;

namespace BlazorApp2.Services
{
    public class ClinicService
    {
        private readonly ApplicationDbContext _context;

        public ClinicService(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Patient Methods ---
        public Task<List<Patient>> GetPatientsAsync() => _context.Patients.ToListAsync();

        public Task<Patient?> GetPatientByIdAsync(int id) => _context.Patients.FindAsync(id).AsTask();

        public async Task AddPatientAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        // --- Appointment Methods ---
        public Task<List<Appointment>> GetAppointmentsAsync()
        {
            // Use .Include() to also load the related Patient details
            return _context.Appointments.Include(a => a.Patient).ToListAsync();
        }

        public async Task AddAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            _context.Entry(appointment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAppointmentAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
