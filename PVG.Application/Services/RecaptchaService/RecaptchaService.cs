using Amazon.S3;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using PVG.Application.Services.CloudflareR2Service;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.RecaptchaService
{
    public class RecaptchaService : BaseService, IRecaptchaService
    {
        private readonly RecaptchaSetting _recaptcha;
        public RecaptchaService(
            IOptions<AppSettings> options,
            IMapper mapper
            ) : base(options, mapper)
        {
            _recaptcha = options.Value.Recaptcha;
        }

        public async Task<bool> Verify(string _token)
        {
            try
            {
                var client = new HttpClient();

                var response = await client.PostAsync(
                    string.Format(_recaptcha.Url, _recaptcha.Key, _token),
                    null);

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<RS_RecaptchaModel>(json);

                if (!result.Success || result.Score < 0.5)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
