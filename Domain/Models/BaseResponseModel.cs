namespace Domain.Models;

public class BaseResponseModel
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public object Data { get; set; }

    public static BaseResponseModel Ok(object data)
    {
        return new BaseResponseModel { Success = true, Data = data };
    }

    public static BaseResponseModel Failed(string message)
    {
        return new BaseResponseModel { Success = false, ErrorMessage = message };
    }
}