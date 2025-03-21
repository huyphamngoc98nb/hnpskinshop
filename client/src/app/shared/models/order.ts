export interface Order {
    id: string
    orderDate: string
    buyerEmail: string
    shippingAddress: ShippingAddress
    deliveryMethod: string
    paymentSummary: PaymentSummary
    orderItems: OrderItem[]
    subtotal: number
    status: string
    paymentIntentId: string
    shippingPrice: number
    total: number
}

export interface ShippingAddress {
    name: string
    line1: string
    line2?: string
    city: string
    state: string
    postalCode: string
    country: string
}

export interface PaymentSummary {
    last4: number
    brand: string
    expMonth: number
    expYear: number
}

export interface OrderItem {
    productId: string
    productName: string
    pictureUrl: string
    price: number
    quantity: number
}

export interface OrderToCreate {
    cartId: string;
    deliveryMethodId: string;
    shippingAddress: ShippingAddress;
    paymentSummary: PaymentSummary;
}
