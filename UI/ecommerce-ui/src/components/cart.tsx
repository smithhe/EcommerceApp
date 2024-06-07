import {useEffect, useState} from "react";
import {CartItem} from "../models/CartItem.ts";
import {useAuth} from "../AuthContext.tsx";
import productService from "../services/ProductService.ts";
import {Product} from "../models/Product.ts";
import {useNavigate} from "react-router-dom";
import cartService from "../services/CartService.ts";
import {toast} from "react-toastify";

const Cart = () => {
    const {isAuthenticated, claims} = useAuth();
    const navigate = useNavigate();
    const [cartItems, setCartItems] = useState<CartItem[]>([]);
    const [products, setProducts] = useState<Product[]>([]);
    const [cartTotal, setCartTotal] = useState<number>(0);

    useEffect(() => {
        if (isAuthenticated === false)
        {
            navigate('/login');
            return;
        }

        const loadCart = async () => {
            const getCartItemsResponse = await cartService.getItemsInCart(claims?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || '');

            if (getCartItemsResponse.success === false)
            {
                toast.error(getCartItemsResponse.message);
                return;
            }

            setCartItems(getCartItemsResponse.cartItems);

            if (cartItems.length === 0)
            {
                return;
            }

            const productIds = Array.from(new Set(cartItems.map(c => c.productId)));

            const localProducts = []
            for (const id of productIds) {
                const getProductResponse = await productService.getProductById(id);

                if (getProductResponse.success)
                {
                    localProducts.push(getProductResponse.product);
                }
            }

            setProducts(localProducts);
            calculateCartTotal(localProducts);
        }

        const calculateCartTotal = (productList: Product[]) => {
            let localCartTotal = 0;

            cartItems.forEach((cartItem) => {
                const product = productList[cartItem.productId];
                localCartTotal += cartItem.quantity * product.price;
            })

            setCartTotal(localCartTotal);
        }

        loadCart();
    }, [cartItems]);

    const startShoppingClick = () => {
        navigate('/categories');
    }

    return (
        <>
            <div className="container mt-4">
                <div className="row w-100 mt-4">
                    <div className="col-md-6">
                        <h1>
                            <i className="bi bi-cart4"></i> Cart
                        </h1>
                    </div>
                    <div className="col-md-6 text-end">
                        <button className="btn btn-danger">Clear Cart</button>
                    </div>
                </div>


                <table className="table table-striped">
                    <thead>
                    <tr>
                        <th>Product Name</th>
                        <th>Quantity</th>
                        <th>Price</th>
                        <th>Item Total</th>
                        <th>Actions</th>
                    </tr>
                    </thead>
                    <tbody>
                    {cartItems.map((item, index) => {

                        return (
                            <tr key={index}>
                                <td></td>
                                <td>{item.quantity}</td>
                                <td>@product?.Price.ToString("C")</td>
                                <td>@price.ToString("C")</td>
                                <td>
                                    <button className="btn btn-primary">Edit</button>
                                    <button className="btn btn-danger">X</button>
                                </td>
                            </tr>
                        )
                    })}
                    </tbody>
                </table>

                {cartItems.length != 0 ? (
                    <>
                        <div className="row w-100 text-center">
                            <h2>Total: ${cartTotal.toFixed(2)}</h2>
                        </div>
                        <div className="row w-100 mt-2">
                            <div className="col-md-3"></div>

                            <div className="col-md-6">
                                <button className="btn btn-success form-control">
                                    <i className="bi bi-paypal"></i> Checkout with PayPal
                                </button>
                            </div>
                        </div>
                    </>
                ) : (
                    <div className="row w-100 mt-2">
                        <div className="col-md-3"></div>

                        <div className="col-md-6">
                            <button className="btn btn-primary form-control" onClick={startShoppingClick}>
                                Nothing Here Yet, Start Shopping Now!
                            </button>
                        </div>
                    </div>
                )}
            </div>
        </>
    )
        ;
}

export default Cart;