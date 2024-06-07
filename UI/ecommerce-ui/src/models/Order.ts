import {OrderStatus} from "./OrderStatus.ts";
import {OrderItem} from "./OrderItem.ts";


export class Order {
    id: number;
    userId: string; // Using string to represent Guid
    createdDate: Date;
    status: OrderStatus;
    total: number;
    payPalRequestId: string; // Using string to represent Guid
    orderItems: OrderItem[];

    constructor(
        id: number,
        userId: string,
        createdDate: Date,
        status: OrderStatus,
        total: number,
        payPalRequestId: string,
        orderItems: OrderItem[] = []
    ) {
        this.id = id;
        this.userId = userId;
        this.createdDate = createdDate;
        this.status = status;
        this.total = total;
        this.payPalRequestId = payPalRequestId;
        this.orderItems = orderItems;
    }
}
