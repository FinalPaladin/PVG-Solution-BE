using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.RecaptchaService
{
    public interface IRecaptchaService
    {
        public Task<bool> Verify(string _token);
    }
}
