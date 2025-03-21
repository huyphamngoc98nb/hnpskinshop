using Core.Entities.OrderAggregate;
using SkiShop.Core.Specifications;

namespace Core.Specifications;

public class OrderSpecifications : BaseSpecifications<Order>
{
    public OrderSpecifications(string email) : base(x => x.BuyerEmail == email)
    {
        AddInclude(x => x.OrderItems);
        AddInclude(x => x.DeliveryMethod);
        AddOrderByDescending(x => x.OrderDate);
    }

    public OrderSpecifications(string email, Guid Id) : base(x => x.BuyerEmail == email && x.Id == Id)
    {
        AddInclude("OrderItems");
        AddInclude("DeliveryMethod");
    }

    public OrderSpecifications(string paymentIntentId, bool isPaymentIntent) : base(x => x.PaymentIntentId == paymentIntentId)
    {
        AddInclude("OrderItems");
        AddInclude("DeliveryMethod");
    }
}