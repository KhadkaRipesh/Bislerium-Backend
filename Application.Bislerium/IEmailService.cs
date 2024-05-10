using Domain.Bislerium.DTOs.EmailHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium
{
    public interface IEmailService
    {
        Task SendMail(EmailMessage email);
    }
}
