using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Infrastructure.ExternalServices.Dtos
{
    public class GetTokenModel
    {
        public string TokenType { get; set; }
        public string Channel { get; set; }
        public string Role { get; set; }
        public string Uid { get; set; }
    }
    public class GetTokenModelResponse
    {
        public string Token { get; set; }
    }
}
