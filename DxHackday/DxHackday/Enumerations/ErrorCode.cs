namespace DxHackday.Controllers
{
    public enum ErrorCode
    {
        InvalidRequestLength = 10001,
        InvalidRequestIds,
        DuplicateRequestIds,
        InvalidParentRequests,
        BatchInBatch
    }
}
