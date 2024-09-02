namespace TodoWebApp.Models.Models;

public class BaseResponseModel
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public object Data { get; set; }
}