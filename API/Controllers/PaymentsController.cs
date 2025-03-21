using System.Reflection.Metadata;
using API.Extensions;
using API.SignalR;
using Core;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata;
using SkiShop.Core.Interfaces;
using Stripe;

namespace API.Controllers;

public class PaymentsController : BaseApiController
{
    private readonly IPaymentService paymentService;
    private readonly IUnitOfWork context;
    private readonly ILogger<PaymentsController> logger;
    private readonly IConfiguration configuration;
    private readonly IHubContext<NotificationHub> hubContext;
    private readonly string _whSecret = "";
    public PaymentsController(IPaymentService paymentService, IUnitOfWork context, 
        ILogger<PaymentsController> logger, IConfiguration configuration, IHubContext<NotificationHub> hubContext)
    {
        this.paymentService = paymentService;
        this.context = context;
        this.logger = logger;
        this.configuration = configuration;
        this.hubContext = hubContext;
        _whSecret = configuration["StripeSettings:WhSecret"]!;
    }

    [Authorize]
    [HttpPost("{cartId}")]
    public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
    {
        var cart = await paymentService.CreateOrUpdatePaymentIntent(cartId);

        if (cart == null) return BadRequest("Problem with your cart");

        return Ok(cart);
    }

    [HttpGet("delivery-methods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethod()
    {
        return Ok(await context.Repository<DeliveryMethod>().GetAll());
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebHook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = ConstructStripeEvent(json);
            if (stripeEvent.Data.Object is not PaymentIntent intent)
            {
                return BadRequest("Invalid event data");
            }

            await HandlePaymentIntentSucceeded(intent);

            return Ok();
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe webhook error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Stripe webhook error");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error");
        }
    }

    private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
    {
        if (intent.Status == "succeeded")
        {
            var spec = new OrderSpecifications(intent.Id, true);

            var order = await context.Repository<Order>().GetEntityWithSpec(spec) ?? throw new Exception("Order not found");

            if ((long)order.GetTotal() * 100 != intent.Amount)
            {
                order.Status = OrderStatus.PaymentMismatch;
            }
            else
            {
                order.Status = OrderStatus.PaymentReceived;
            }

            await context.Complete();

            var connectId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);

            if (!string.IsNullOrEmpty(connectId)){
                await hubContext.Clients.Client(connectId).SendAsync("OrderCompleteNotification", order.toDto());
            }
        }
    }

    private Event ConstructStripeEvent(string json)
    {
        try
        {
            return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falied to construct stripe event");
            throw new StripeException("Invalid signture");
        }
    }
}