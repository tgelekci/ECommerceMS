namespace OrderService.DataTransferObjects;

public class UpdateOrderStatusRequest
{
    public string Status { get; set; } = string.Empty;
}