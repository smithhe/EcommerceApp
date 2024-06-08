import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import {Product} from "../models/Product.ts";
import {StarRatingsSvg} from "./childComponents/starratings.tsx";
import ProductReviews from "./childComponents/productreviews.tsx";
import {useAuth} from "../AuthContext.tsx";
import {Review} from "../models/Review.ts";
import productService from "../services/ProductService.ts";
import reviewService from "../services/ReviewService.ts";
import cartService from "../services/CartService.ts";
import Modal from "react-modal";
import {CartItem} from "../models/CartItem.ts";

// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-expect-error
import ReactStars from 'react-rating-stars-component';


const ProductDetail = () => {
    const { productId } = useParams<{ productId?: string }>();
    const { claims, isAuthenticated } = useAuth();
    const [product, setProduct] = useState<Product | null>(null);
    const [isEditing, setIsEditing] = useState<boolean>(false);
    const [userReview, setUserReview] = useState<Review | undefined>(undefined)
    const [userHasReview, setUserHasReview] = useState<boolean>(false);
    const [starRating, setStarRating] = useState<number>(0)
    const [comments, setComments] = useState<string>('');
    const [modalIsOpen, setIsOpen] = useState(false);
    const [modalCount, setModalCount] = useState<number>(0);
    const navigate = useNavigate();

    useEffect(() => {
        if (!productId) {
            toast.error('Invalid product ID');
            return;
        }

        const fetchProduct = async () => {
            const response = await productService.getProductById(parseInt(productId));
            if (response.success == false) {
                toast.error(response.message);
                return;
            }

            const userReviewResponse = await reviewService.getUserReview(claims?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || '', response.product.id)

            if (userReviewResponse.success && userReview != null)
            {
                setUserHasReview(true);
                setStarRating(userReviewResponse.userReview.starRatings);
                setUserReview(userReviewResponse.userReview);

                const index = response.product.customerReviews.findIndex((r: Review) => r.id === userReviewResponse.userReview.id);
                if (index > -1)
                {
                    response.product.customerReviews = response.product.customerReviews.slice(0, index).concat(response.product.customerReviews.slice(index + 1));
                }
            }

            setProduct(response.product);
        };

        fetchProduct();
    }, [claims, isAuthenticated, productId]);

    const ratingChanged = (newRating: number) => {
        setStarRating(newRating);
    };

    const createReview = async () => {
        const newReview: Review = new Review(0, product?.id || 0, claims?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || '', starRating, comments);

        const response = await reviewService.submitReview(newReview);

        if (response.success)
        {
            setUserReview(response.review);
            return;
        }

        toast.error(response.message);
    }

    const updateReview = async () => {
        if (userReview == undefined) return;

        const reviewToUpdate = userReview;

        reviewToUpdate.stars = starRating;
        reviewToUpdate.comments = comments;

        const response = await reviewService.updateReview(reviewToUpdate);

        if (response.success)
        {
            setUserReview(reviewToUpdate)
            return;
        }

        toast.error(response.message);
    }

    const deleteReview = async () => {
        if (userReview == undefined) return;

        const response = await reviewService.removeReview(userReview);

        if (response.success)
        {
            setUserReview(undefined);
            return;
        }

        toast.error(response.message);
    }

    const editReview = () => {
        setIsEditing(true);
    }

    const cancelEditing = () => {
        setIsEditing(false);
    }

    const customStyles = {
        content: {
            top: '50%',
            left: '50%',
            right: 'auto',
            bottom: 'auto',
            marginRight: '-50%',
            transform: 'translate(-50%, -50%)',
        },
    };

    const addToCartClick = () => {
        if (isAuthenticated) {
            setIsOpen(true);
        }
        else {
            navigate('/login');
        }
    };

    const closeModal = async () => {
        setIsOpen(false);
        const response = await cartService.addItemToCart( new CartItem(0, product?.id || 0, claims?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || '', modalCount));

        if (response.success)
        {
            toast.success(response.message);
            return;
        }

        toast.error(response.message);
    }

    const cancelCloseModal = () => {
        setIsOpen(false);
    }

    if (!product) {
        return <p><em>Loading...</em></p>;
    }

    return (
        <>
            <Modal isOpen={modalIsOpen} style={customStyles} contentLabel="Add To Cart">
                <div className="simple-form mx-4 ">
                    <div className="row text-center">
                        <h1 className="card-title">{product.name}</h1>
                        <p className="card-text mt-2">{product.description}</p>
                    </div>
                    <div className="row mt-3">
                        <div className="form-group text-center">
                            <label htmlFor="quantity" className="h5">Quantity:</label>
                            <input type="number" className="form-control" id="quantity" name="quantity" min="1" onChange={(e) => setModalCount(Number(e.target.value))}/>
                        </div>
                    </div>

                    <div className="row mt-4">
                        <button type="submit" className="btn btn-primary form-control" onClick={closeModal}>Add to Cart</button>
                        <button className="btn btn-secondary form-control mt-2" onClick={cancelCloseModal}>Cancel</button>
                    </div>
                </div>
            </Modal>
            <div className="row w-100">
                <div className="container mt-5">
                    <div className="container text-center">
                        <h1 className="display-4">{product.name}</h1>
                        <hr className="my-4"/>
                    </div>
                </div>
            </div>

            <div className="row w-100 mt-3">
                <div className="col text-center">
                    <img src={product.imageUrl} alt={product.name} className="img-top img-fluid"/>
                </div>
            </div>

            <div className="row w-100 mt-3">
                <div className="row">
                    <div className="col text-center">
                        <h1 className="display-6">Price: ${product.price}</h1>
                        {product == null ? (
                            <button type="button" className="btn btn-primary btn-lg">Add to Cart <i className="bi bi-cart4"></i></button>
                            ) : (
                            <button type="button" className="btn btn-primary btn-lg" onClick={addToCartClick}>Add to Cart <i className="bi bi-cart4"></i></button>
                            )}
                    </div>
                </div>

                <div className="row mt-4">
                    <div className="col text-center">
                        <h1 className="display-6">Description</h1>
                    </div>
                </div>

                <div className="row">
                    <div className="col text-center">
                        <h1 className="lead">{product.description}</h1>
                    </div>
                </div>
            </div>

            <div className="row w-100">
                <div className="container mt-5">
                    <div className="container text-center">
                        <h1 className="display-4">Customer Reviews</h1>
                        <hr className="my-4"/>
                    </div>
                </div>
            </div>

            <div className="row w-100">
                <div className="ratings text-center mb-3">
                    <h4>Average Rating</h4>
                    <span className="rating-stars">
                        <StarRatingsSvg averageRating={product.averageRating}/>
                    </span>
                </div>
            </div>

            {isAuthenticated ? (
                <>
                    {userHasReview ? (
                        <>
                            {isEditing ? (
                                <div className="row w-100 text-center">
                                    <div className="card mb-4">
                                        <div className="card-body">
                                            <h5 className="card-title">Update Your Review</h5>
                                            <form>
                                                <div className="form-group">
                                                    <div className="ratings">
                                                    <span className="rating-stars text-primary text-center">
                                                        <ReactStars count={5} onChange={ratingChanged} size={24}
                                                                    activeColor="#ffd700"  />
                                                    </span>
                                                    </div>
                                                </div>

                                                <div className="form-group">
                                                    <label htmlFor="comments">Comments (Max 500 characters):</label>
                                                    <textarea className="form-control" id="comments" name="comments"
                                                              rows={4} maxLength={500} required></textarea>
                                                </div>

                                                <button type="button" className="btn btn-primary mt-2"
                                                        onClick={() => updateReview()}>Save Changes
                                                </button>
                                                <button type="button" className="btn btn-danger mt-2 ms-2"
                                                        onClick={() => deleteReview()}>Delete
                                                </button>
                                                <button type="button" className="btn btn-secondary mt-2 ms-2"
                                                        onClick={() => cancelEditing()}>Cancel
                                                </button>
                                            </form>
                                        </div>
                                    </div>
                                </div>
                            ) : (
                                <div className="row w-100">
                                    <div className="card mb-4">
                                        <div className="card-body">
                                            <h5 className="card-title">{userReview?.userName}</h5>
                                            <div className="ratings">
                                                <span className="rating-stars text-primary">
                                                    <StarRatingsSvg averageRating={product.averageRating}/>
                                                </span>
                                            </div>
                                            <p className="card-text mt-2">{userReview?.comments}</p>
                                            <button type="button" className="btn btn-primary" onClick={() => editReview()}>Edit</button>
                                            <button type="button" className="btn btn-danger ms-2" onClick={() => deleteReview()}>Delete</button>
                                        </div>
                                    </div>
                                </div>
                            )}
                        </>
                    ) : (
                        <div className="row w-100 text-center">
                            <div className="card mb-4">
                                <div className="card-body">
                                    <h5 className="card-title">Submit Your Review</h5>
                                    <form className="align-content-center" onSubmit={() => createReview()}>
                                        <div className="form-group">
                                            <div className="ratings">
                                                <span className="rating-stars text-primary text-center">
                                                    <ReactStars count={5} onChange={ratingChanged} size={24}
                                                                activeColor="#ffd700"/>
                                                </span>
                                            </div>
                                        </div>

                                        <div className="form-group">
                                            <label htmlFor="comments">Comments (Max 500 characters):</label>
                                            <textarea className="form-control" id="comments" name="comments" rows={4}
                                                      maxLength={500} required
                                                      onChange={(e) => setComments(e.target.value)}></textarea>
                                        </div>

                                        <button type="submit" className="btn btn-primary mt-2">Submit Review</button>
                                    </form>
                                </div>
                            </div>
                        </div>
                    )}
                </>
            ) : (<></>)}

            {product.customerReviews == null || product.customerReviews.length === 0 ? (
                <div className="row w-100">
                    <div className="col text-center">
                        <h3 className="display-6">No Customer Reviews Yet</h3>
                    </div>
                </div>
            ) : (
                <div className="row w-100">
                    <ProductReviews product={product}/>
                </div>
            )}

            <ToastContainer/>
        </>
    );
};

export default ProductDetail;
