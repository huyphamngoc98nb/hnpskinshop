using System.Reflection;

namespace Core;

public class ShoppingCart {
    public required Guid Id {get; set;}

    public List<CartItem> Items {get; set;} = [];

    public Guid? DeliveryMethodId {get; set;}

    public string? ClientSecret {get; set;}

    public string? PaymentIntentId {get; set;}
}