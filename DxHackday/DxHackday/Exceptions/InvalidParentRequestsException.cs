using System;

namespace DxHackday.Controllers
{
    public class InvalidParentRequestsException : Exception
    {
        public InvalidParentRequestsException() : base("Invalid parent request ids found in batch.")
        {
        }
    }
}
