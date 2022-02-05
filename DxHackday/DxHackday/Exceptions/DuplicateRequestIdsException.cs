using System;

namespace DxHackday.Controllers
{
    public class DuplicateRequestIdsException : Exception
    {
        public DuplicateRequestIdsException(params int[] duplicateRequestIds) : base($"Duplicate request ids({string.Join(",",duplicateRequestIds)}) found in batch.")
        {
        }
    }
}
