using Core.Entities.OrderAggregate;

namespace API.DTOs;

public class OrderDTO{
    
    public Guid Id {get; set;}
    public DateTime OrderDate {get;set;} = DateTime.UtcNow;

    public required string BuyerEmail { get; set; }

    public ShippingAddress ShippingAddress { get; set; } = null!;

    public required string DeliveryMethod {get;set;}

    public required PaymentSummary PaymentSummary {get; set;}

    public required List<OrderItemDto> OrderItems {get;set;} = [];

    public decimal Subtotal { get; set; }

    public required string Status { get; set; }

    public required string PaymentIntentId { get; set; }

    public decimal ShippingPrice { get; set; }

    public decimal Total {get; set;}
}
