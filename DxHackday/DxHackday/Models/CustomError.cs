using Newtonsoft.Json;
using System;
using System.Collections;

namespace DxHackday.Controllers
{
    public class CustomError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomError"/> class.
        /// </summary>
        /// <param name="errorCode">Custom Error code of type <see cref="ErrorCode"/></param>
        /// <param name="message">Custom error message</param>
        public CustomError(ErrorCode errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomError"/> class.
        /// </summary>
        /// <param name="errorCode">Custom Error code of type <see cref="ErrorCode"/></param>
        /// <param name="exception">Exception thrown by the API</param>
        public CustomError(ErrorCode errorCode, Exception exception)
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            ErrorCode = errorCode;
            Message = exception.Message;

            // AA: If the following is not done, we get an empty JSON Array in the serialized JSON
            Data = exception.Data.Keys.Count > 0 ? exception.Data : null;
        }

        /// <summary>
        /// Gets the custom error code
        /// </summary>
        /// <value>
        /// The custom error code
        /// </value>
        [JsonProperty(PropertyName = "errorCode")]
        public ErrorCode ErrorCode { get; }

        /// <summary>
        /// Gets the custom error message
        /// </summary>
        /// <value>
        /// The custom error message
        /// </value>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; }

        /// <summary>
        /// Gets the Exception data.
        /// </summary>
        /// <value>
        /// The Exception data.
        /// </value>
        [JsonProperty(PropertyName = "data")]
        public IDictionary Data { get; }

        /// <summary>
        /// Overridden method of <see cref="object.ToString"/>
        /// </summary>
        /// <returns>A stringified value of an instance of <see cref="CustomError"/> class</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
