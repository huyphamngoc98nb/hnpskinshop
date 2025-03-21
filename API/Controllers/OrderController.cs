using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class OrdersController : BaseApiController
{
    private readonly ICartService cartService;
    private readonly IUnitOfWork unitOfWork;

    public OrdersController(ICartService cartService, IUnitOfWork unitOfWork)
    {
        this.cartService = cartService;
        this.unitOfWork = unitOfWork;
    }
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderDTO createOrderDTO)
    {
        var email = User.GetEmail();

        var cart = await cartService.GetCartAsync(createOrderDTO.CartId.ToString());

        if (cart == null) return BadRequest("Cart not found");

        if (cart.PaymentIntentId == null) return BadRequest("No payment intent for this order");

        var items = new List<OrderItem>();

        foreach (var item in cart.Items)
        {
            var productItem = await unitOfWork.Repository<Product>().GetById(item.ProductId);

            if (productItem == null) return BadRequest("Problem with the order");

            var itemOrdered = new ProductItemOrdered
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                PictureUrl = item.PictureUrl
            };

            var orderItem = new OrderItem
            {
                ItemOrdered = itemOrdered,
                Price = productItem.Price,
                Quantity = item.Quantity
            };

            items.Add(orderItem);
        }

        var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetById(createOrderDTO.DeliveryMethodId);

        if (deliveryMethod == null) return BadRequest("No delivery method selected");

        var order = new Order
        {
            OrderItems = items,
            DeliveryMethod = deliveryMethod,
            ShippingAddress = createOrderDTO.ShippingAddress,
            Subtotal = items.Sum(x => x.Price * x.Quantity),
            PaymentSummary = createOrderDTO.PaymentSummary,
            PaymentIntentId = cart.PaymentIntentId,
            BuyerEmail = email
        };

        unitOfWork.Repository<Order>().Create(order);

        if (await unitOfWork.Complete())
        {
            return order;
        }

        return BadRequest("Problem creating order");
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDTO>>> GetOrderForUser()
    {
        var spec = new OrderSpecifications(User.GetEmail());

        var order = await unitOfWork.Repository<Order>().ListAsync(spec);

        var orderToReturn = order.Select(o => o.toDto()).ToList();

        return Ok(orderToReturn);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDTO>> GetOrderById(Guid id)
    {
        var spec = new OrderSpecifications(User.GetEmail(), id);

        var order = await unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

        if (order == null) return NotFound();

        return order.toDto();
    }
}