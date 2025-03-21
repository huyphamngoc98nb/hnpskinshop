import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { OrderToCreate } from '../../shared/models/order';
import { Order } from '../../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  orderComplete = false;

  crateOrder(orderToCreate: OrderToCreate) {
    return this.http.post<Order>(this.baseUrl + 'orders', orderToCreate);
  }

  getOrdersForUser() {
    return this.http.get<Order[]>(this.baseUrl + 'orders');
  }

  getOrderDetailed(id: string) {
    return this.http.get<Order>(this.baseUrl + 'orders/' + id);
  }
  constructor() { }
}
