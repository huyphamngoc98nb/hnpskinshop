import {v4 as uuidv4} from 'uuid';

export type CartType = {
    id: string;
    items: CartItem[];
    deliverMethodId?: string;
    paymentIntentId?: string;
    clientSecret?: string;
}

export type CartItem = {
    productId: string;
    productName: string;
    price: number;
    quantity: number;
    pictureUrl: string;
    brand: string;
    type: string;
}

export class Cart implements CartType {
    id = uuidv4();
    items: CartItem[] = [];
    deliveryMethodId?: string;
    paymentIntentId?: string;
    clientSecret?: string;
}