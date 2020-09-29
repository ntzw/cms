namespace Foundation.Modal.Result
{
    /// <summary>
    /// 处理程序返回类
    /// </summary>
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

        public static HandleResult Error(string errorMsg = "无效参数")
        {
            return new HandleResult
            {
                IsSuccess = false,
                Message = errorMsg
            };
        }
    }
}