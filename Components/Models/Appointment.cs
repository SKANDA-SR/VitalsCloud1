// File: Models/Appointment.cs
using System.ComponentModel.DataAnnotations;

namespace BlazorApp2.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a patient.")]
        public int? PatientId { get; set; }
        public Patient? Patient { get; set; }

        [Required(ErrorMessage = "Doctor's name is required.")]
        public string? DoctorName { get; set; }

        [Required(ErrorMessage = "Please select a service.")]
        public string? Service { get; set; }

        [Required(ErrorMessage = "Please specify the appointment date.")]
        public DateTime? AppointmentDate { get; set; } = DateTime.Today;
    }
}