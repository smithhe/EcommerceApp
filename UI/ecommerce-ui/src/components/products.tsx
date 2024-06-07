import { useState, useEffect } from 'react';
import {Link, useNavigate, useParams} from 'react-router-dom';
import {toast, ToastContainer} from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { Product } from "../models/Product.ts";
import StarRatings from "./childComponents/starratings.tsx";
import '../styles/Products.css'
import {useAuth} from "../AuthContext.tsx";
import productService from "../services/ProductService";
import Modal from "react-modal";
import cartService from "../services/CartService.ts";
import {CartItem} from "../models/CartItem.ts";

const Products = () => {
    const [productList, setProductList] = useState<Product[]>([]);
    const { categoryId } = useParams<{ categoryId: string }>();
    const { isAuthenticated, claims } = useAuth();
    const navigate = useNavigate();
    const [modalIsOpen, setIsOpen] = useState(false);
    const [modalCount, setModalCount] = useState<number>(0);
    const [modalProductId, setModalProductId] = useState<number>(0);
    const [modalProductName, setModalProductName] = useState<string>('');
    const [modalProductDesc, setModalProductDesc] = useState<string>('');


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

        const product = productList.find((p) => p.id === productId);

        if (product == undefined)
        {
            toast.error('Invalid product ID');
            return;
        }

        setModalProductName(product.name);
        setModalProductDesc(product.description);
        setModalProductId(productId);
        setIsOpen(true);
    };

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

    const closeModal = async () => {
        setIsOpen(false);
        const response = await cartService.addItemToCart( new CartItem(0, modalProductId || 0, claims?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || '', modalCount));

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
            <Modal isOpen={modalIsOpen} style={customStyles} contentLabel="Add To Cart">
                <div className="simple-form mx-4 ">
                    <div className="row text-center">
                        <h1 className="card-title">{modalProductName}</h1>
                        <p className="card-text mt-2">{modalProductDesc}</p>
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
            <ToastContainer />
        </div>
    );
};

export default Products;
