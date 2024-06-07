export class Review {
    id: number;
    productId: number;
    userName: string;
    stars: number;
    comments: string;

    constructor(
        id: number,
        productId: number,
        userName: string,
        stars: number,
        comments: string
    ) {
        this.id = id;
        this.productId = productId;
        this.userName = userName;
        this.stars = stars;
        this.comments = comments;
    }
}
