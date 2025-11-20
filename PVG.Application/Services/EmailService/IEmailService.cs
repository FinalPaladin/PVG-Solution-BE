using Microsoft.AspNetCore.Http;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.EmailService
{
    public interface IEmailService
    {
        public Task<RS_EmailModel> SendEmailRequest(string _subject, string _htmlBody, IEnumerable<IFormFile>? attachments = null);
    }
}
