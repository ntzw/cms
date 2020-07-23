namespace Foundation.Modal.Result
{
    public class HandleResult : ResultBase
    {
        public object Data { get; set; }

        public static HandleResult Success(object data = null)
        {
            return new HandleResult
            {
                IsSuccess = true,
                Data = data,
            };
        }

        public static HandleResult Error(string errorMsg)
        {
            return new HandleResult
            {
                IsSuccess = false,
                Message = errorMsg
            };
        }
    }
}