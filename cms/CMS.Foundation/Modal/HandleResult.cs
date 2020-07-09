namespace Foundation.Modal
{
    public class HandleResult
    {
        public bool IsSuccess { get; set; }
        
        public string Message { get; set; }
        
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