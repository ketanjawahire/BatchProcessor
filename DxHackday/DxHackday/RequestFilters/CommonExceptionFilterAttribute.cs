using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace DxHackday.Controllers
{
    public sealed class CommonExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            SetExceptionContextResult(context);
            base.OnException(context);
        }

        private static void SetExceptionContextResult(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case InvalidRequestLengthException ex:
                    context.Result = new ErrorResult(ErrorCode.InvalidRequestLength, ex);
                    break;
                case InvalidRequestIdsException ex:
                    context.Result = new ErrorResult(ErrorCode.InvalidRequestIds, ex);
                    break;
                case DuplicateRequestIdsException ex:
                    context.Result = new ErrorResult(ErrorCode.DuplicateRequestIds, ex);
                    break;
                case InvalidParentRequestsException ex:
                    context.Result = new ErrorResult(ErrorCode.InvalidParentRequests, ex);
                    break;
                case BatchInBatchException ex:
                    context.Result = new ErrorResult(ErrorCode.BatchInBatch, ex);
                    break;
            }
        }
    }
}
