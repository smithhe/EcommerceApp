import { useState, useEffect } from 'react';
import {Link, useNavigate, useParams} from 'react-router-dom';
import {toast, ToastContainer} from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { Product } from "../models/Product.ts";
import StarRatings from "./childComponents/starratings.tsx";
import '../styles/Products.css'
import {useAuth} from "../AuthContext.tsx";
import productService from "../services/ProductService";

const Products = () => {
    const [productList, setProductList] = useState<Product[]>([]);
    const { categoryId } = useParams<{ categoryId: string }>();
    const { isAuthenticated } = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        if (!categoryId) {
            toast.error('Invalid category ID');
            return;
        }

        const fetchProducts = async () => {
            const response = await productService.getAllProducts(parseInt(categoryId));
            if (response.success) {
                setProductList(response.products);
            } else {
                toast.error(response.message);
            }
        };

        fetchProducts();
    }, [categoryId]);

    const addToCartClick = (productId: number) => {
        if (isAuthenticated === false) {
            navigate('/login');
        }

        toast.info('Adding to cart');
        console.log(productId);
    };

    if (productList.length === 0) {
        return <p><em>Loading...</em></p>;
    }

    return (
        <div className="container mt-5">
            <div className="text-center mb-3">
                <h1 className="products-header">Browse Our Products</h1>
            </div>

            <div className="row">
                {productList.map((product, index) => (
                    <div className="col-md-4 mb-4" key={index}>
                        <div className="card product-card clickable-card">
                            <Link to={`/ProductDetail/${product.id}`} style={{ textDecoration: 'none' }}>
                                <img src={product.imageUrl} alt={product.name} className="card-img-top"/>
                                <div className="card-body">
                                    <h5 className="card-title">{product.name}</h5>
                                    <p className="card-text">${product.price}</p>
                                    <div className="ratings">
                                        <span className="rating-stars">
                                            <StarRatings averageRating={product.averageRating}/>
                                        </span>
                                    </div>
                                </div>
                            </Link>
                            <div className="row w-100 text-center mb-3">
                                <div className="col-md-3"></div>
                                <div className="col-md-6">
                                    <button className="btn btn-primary" style={{textDecoration: 'none'}} onClick={() => addToCartClick(product.id)}>Add to Cart</button>
                                </div>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
            <ToastContainer />
        </div>
    );
};

export default Products;
