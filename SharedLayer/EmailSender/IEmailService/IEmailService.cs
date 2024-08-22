using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServiceLayer.EmailSetting
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string message, string link, string htmlBody, string password = null);
    }
}
