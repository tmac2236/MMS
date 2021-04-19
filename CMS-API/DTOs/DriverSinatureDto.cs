using Microsoft.AspNetCore.Http;

namespace API.DTOs
{
    public class DriverSinatureDto
    {
        public IFormFile File { get; set; }
        public string LicenseNumber { get; set; }
        public string SignInDate { get; set; }

    }
}