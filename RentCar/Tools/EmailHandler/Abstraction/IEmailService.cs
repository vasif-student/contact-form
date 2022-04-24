using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentCar.Tools.EmailHandler.Abstraction
{
    public interface IEmailService
    {
        Task SendEMailAsync(MailRequest mailRequest);
    }
}
