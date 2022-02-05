using System;

namespace DxHackday.Controllers
{
    public class BatchInBatchException : Exception
    {
        public BatchInBatchException() : base("Batch in batch detected. Please remove batch request & try again.")
        {
        }
    }
}
