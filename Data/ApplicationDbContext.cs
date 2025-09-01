// File Path: BlazorApp2/Data/ApplicationDbContext.cs

using Microsoft.EntityFrameworkCore;
using BlazorApp2.Models; // Make sure this matches your Models folder

namespace BlazorApp2.Data // This line creates the namespace you need
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}