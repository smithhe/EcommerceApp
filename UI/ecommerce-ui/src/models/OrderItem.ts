export class OrderItem {
    id: number;
    orderId: number;
    productName: string;
    productDescription: string;
    productSku: string;
    quantity: number;
    price: number;

    constructor(
        id: number,
        orderId: number,
        productName: string,
        productDescription: string,
        productSku: string,
        quantity: number,
        price: number
    ) {
        this.id = id;
        this.orderId = orderId;
        this.productName = productName;
        this.productDescription = productDescription;
        this.productSku = productSku;
        this.quantity = quantity;
        this.price = price;
    }
}
