using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net;

namespace DxHackday.Controllers
{
    public class ErrorResult : JsonResult
    {
        public ErrorResult(ErrorCode code, Exception exception)
                : base(null)
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            StatusCode = (int)HttpStatusCode.InternalServerError;

            switch (code)
            {
                case ErrorCode.InvalidRequestLength:
                case ErrorCode.DuplicateRequestIds:
                case ErrorCode.InvalidRequestIds:
                    StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
                default:
                    break;
            }

            Value = string.IsNullOrEmpty(exception.Message)
                ? new CustomError(code, "If problem persists, please contact support.")
                : new CustomError(code, exception);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResult"/> class with value and serializer settings.
        /// </summary>
        /// <param name="value">Value for ErrorResult</param>
        /// <param name="serializerSettings">JsonSerializerSettings</param>
        public ErrorResult(object value, JsonSerializerSettings serializerSettings)
            : base(value, serializerSettings)
        {
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
