import { Component, inject, OnInit, output } from '@angular/core';
import { CheckoutService } from '../../../core/services/checkout.service';
import { MatRadioModule } from '@angular/material/radio';
import { CurrencyPipe } from '@angular/common';
import { CartService } from '../../../core/services/cart.service';
import { DeliveryMethod } from '../../../shared/models/deliveryMethod';

@Component({
  selector: 'app-checkout-delivery',
  standalone: true,
  imports: [
    MatRadioModule,
    CurrencyPipe
  ],
  templateUrl: './checkout-delivery.component.html',
  styleUrl: './checkout-delivery.component.scss'
})
export class CheckoutDeliveryComponent implements OnInit {
  checkoutServices = inject(CheckoutService);
  cartServices = inject(CartService);
  deliverComplete = output<boolean>();

  ngOnInit(): void {
    this.checkoutServices.getDeliveryMethods().subscribe(
      {
        next: methods => {
          if (this.cartServices.cart()?.deliveryMethodId) {
            const method = methods.find(x => x.id === this.cartServices.cart()?.deliveryMethodId);
            if (method) {
              this.cartServices.selectedDelivery.set(method);
              this.deliverComplete.emit(true);
            }
          }
        }
      }
    );
  }

  updateDeliveryMethod(method: DeliveryMethod) {
    this.cartServices.selectedDelivery.set(method);
    const cart = this.cartServices.cart();
    if (cart) {
      cart.deliveryMethodId = method.id;
      this.cartServices.setCart(cart);
      this.deliverComplete.emit(true);
    }
  }
}
