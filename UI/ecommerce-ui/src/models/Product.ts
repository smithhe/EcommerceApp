import {Review} from "./Review.ts";

export class Product {
    id: number;
    categoryId: number;
    name: string;
    description: string;
    price: number;
    averageRating: number;
    quantityAvailable: number;
    imageUrl: string;
    customerReviews: Review[];

    constructor(
        id: number,
        categoryId: number,
        name: string,
        description: string,
        price: number,
        averageRating: number,
        quantityAvailable: number,
        imageUrl: string,
        reviews: Review[] = []
    ) {
        this.id = id;
        this.categoryId = categoryId;
        this.name = name;
        this.description = description;
        this.price = price;
        this.averageRating = averageRating;
        this.quantityAvailable = quantityAvailable;
        this.imageUrl = imageUrl;
        this.customerReviews = reviews;
    }
}
