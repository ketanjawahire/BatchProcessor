using System;

namespace DxHackday.Controllers
{
    public class InvalidRequestIdsException : Exception
    {
        public InvalidRequestIdsException(params int[] invalidRequestIds) : base($"Invalid request ids({string.Join(",", invalidRequestIds)}) found in batch.")
        {
        }
    }
}
