using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Middleware
{
    public class HandlerExceptionMiddleware
    {
        RequestDelegate Next { get; }
        ILogger<HandlerExceptionMiddleware> Logger { get; }

        public HandlerExceptionMiddleware(RequestDelegate next, ILogger<HandlerExceptionMiddleware> logger)
        {
            this.Next = next;
            this.Logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.Next(context);
            }
            catch (Exception ex)
            {
                await HandlerExceptionAsync(context, ex, this.Logger);
            }
        }

        private async Task HandlerExceptionAsync(HttpContext context, Exception ex, ILogger<HandlerExceptionMiddleware> logger)
        {
            SerializedError error;
            int errorCode;

            switch (ex)
            {
                //case Exception e:
                default:
                    logger.LogError(ex, "Error from server");
                    error = SerializeErrors(ex);
                    errorCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = errorCode;
            if (error != null)
            {
                var result = JsonConvert.SerializeObject(new { error }, 
                    new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                await context.Response.WriteAsync(result);
            }
        }

        private static SerializedError SerializeErrors(Exception ex)
        {
            SerializedError error;

            if (ex is AggregateException aggregateException)
            {
                error = new SerializedAggregateError();

                ((SerializedAggregateError)error).InnerErrors =
                    aggregateException.InnerExceptions
                        .Select(SerializeErrors);
            }
            else
            {
                error = new SerializedError();
            }
            error.Message = ex.Message;
            if (ex.InnerException != null)
            {
                error.InnerError = SerializeErrors(ex.InnerException);
            }

            return error;
        }


        private class SerializedError
        {
            public string Message { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public SerializedError InnerError { get; set; }

        }

        private class SerializedAggregateError : SerializedError
        {
            public IEnumerable<SerializedError> InnerErrors { get; set; }
        }

    }
}
