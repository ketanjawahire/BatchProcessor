using System;

namespace DxHackday.Controllers
{
    public class InvalidRequestLengthException : Exception
    {
        public InvalidRequestLengthException(int requestLength) : base($"Invalid request length({requestLength}). Allowed limit is 30 requests per batch.")
        {
        }
    }
}
