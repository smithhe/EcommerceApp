import {Review} from "../../models/Review.ts";
import StarRatings from "./starratings.tsx";

// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-expect-error
const ProductReviews = ({ product }, ) => {
    return (
        <div className="row w-100">
            {product.customerReviews.map((review: Review, index: number) => (
                <div className="card mb-4" key={index}>
                    <div className="card-body">
                        <h5 className="card-title">{review.userName}</h5>
                        <div className="ratings">
                            <StarRatings averageRating={review.stars}/>
                        </div>
                        <p className="card-text mt-2">{review.comments}</p>
                    </div>
                </div>
            ))}
        </div>
    );
};

export default ProductReviews;
