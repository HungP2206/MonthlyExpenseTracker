namespace MonthlyExpenseTracker.Common
{
    public class ServiceResult
    {
        public bool Succeeded { get; private set; }

        public string? ErrorMessage { get; private set; }
        public string? FieldName { get; private set; }
        //public string? ErrorCode { get; private set; }

        public static ServiceResult Success()
        {
            return new ServiceResult
            {
                Succeeded = true
            };
        }

        public static ServiceResult Failure(string errorMessage, string? fieldName = null)
        {
            return new ServiceResult
            {
                Succeeded = false,
                ErrorMessage = errorMessage,
                FieldName = fieldName
            };
        }
    }
}
