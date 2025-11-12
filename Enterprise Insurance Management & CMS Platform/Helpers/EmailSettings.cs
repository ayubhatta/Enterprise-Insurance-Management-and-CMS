using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Enterprise_Insurance_Management___CMS_Platform.Helpers
{
    public class EmailSettings
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Host { get; set; }
        public string? DisplayName { get; set; }
        public bool UseSSL { get; set; }
        public int Port { get; set; }
    }
}
