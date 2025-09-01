// File: Models/Patient.cs
using System.ComponentModel.DataAnnotations;

namespace BlazorApp2.Models
{
    public class Patient
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Patient name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? Name { get; set; }
        
        [Required(ErrorMessage = "Contact information is required.")]
        [StringLength(20, ErrorMessage = "Contact cannot exceed 20 characters.")]
        public string? Contact { get; set; }
        
        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
    }
}
