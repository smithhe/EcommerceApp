export class CartItem {
    id: number;
    productId: number;
    userId: string;
    quantity: number;

    constructor(
        id: number,
        productId: number,
        userId: string,
        quantity: number
    ) {
        this.id = id;
        this.productId = productId;
        this.userId = userId;
        this.quantity = quantity;
    }
}
