using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Formatting
{
    public class InputJsonFormatter : TextInputFormatter
    {
        public InputJsonFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        public async override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            using (var reader = new StreamReader(context.HttpContext.Request.Body))
            {
                var requestBody = await reader.ReadToEndAsync();
                var createdvalue = Activator.CreateInstance(context.ModelType);
                JsonConvert.PopulateObject(requestBody, createdvalue);
                return InputFormatterResult.Success(createdvalue);
            }
        }
    }
}
