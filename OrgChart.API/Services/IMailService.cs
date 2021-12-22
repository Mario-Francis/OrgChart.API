using OrgChart.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.Services
{
    public interface IMailService
    {
        Task<bool> SendMail(Mail mail);
    }
}
