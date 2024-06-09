import {useEffect, useState} from "react";
import {CartItem} from "../models/CartItem.ts";
import {useAuth} from "../AuthContext.tsx";
import productService from "../services/ProductService.ts";
import {Product} from "../models/Product.ts";
import {useNavigate} from "react-router-dom";
import cartService from "../services/CartService.ts";
import {toast, ToastContainer} from "react-toastify";
import Modal from "react-modal";
import orderService from "../services/OrderService.ts";
import {PaymentSource} from "../models/PaymentSource.ts";
import LoadingIcon from "./childComponents/LoadingIcon.tsx";

const Cart = () => {
    const {isAuthenticated, claims} = useAuth();
    const navigate = useNavigate();

    const [cartItems, setCartItems] = useState<CartItem[]>([]);
    const [cartTotal, setCartTotal] = useState<number>();
    const [modalIsOpen, setIsOpen] = useState(false);
    const [products, setProducts] = useState<Product[]>([]);
    const [modalCount, setModalCount] = useState<number>(0);
    const [modalProductName, setModalProductName] = useState<string>('');
    const [modalProductDesc, setModalProductDesc] = useState<string>('');
    const [modalCartItem, setModalCartItem] = useState<CartItem>();
    const [isProcessing, setIsProcessing] = useState<boolean>(false);

    useEffect(() => {
        if (!isAuthenticated)
        {
            return;
        }

        const loadCart = async () => {
            const getCartItemsResponse = await cartService.getItemsInCart(claims?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || '');

            if (getCartItemsResponse.success === false)
            {
                toast.error(getCartItemsResponse.message);
                return;
            }

            const localCartItems: CartItem[] = getCartItemsResponse.cartItems;

            if (localCartItems.length === 0)
            {
                return;
            }

            const productIds = Array.from(new Set(localCartItems.map(c => c.productId)));
            const localProducts: Product[] = []

            for (const id of productIds) {
                const getProductResponse = await productService.getProductById(id);

                if (getProductResponse.success)
                {
                    localProducts.push(getProductResponse.product);
                }
            }

            let localCartTotal = 0;

            localCartItems.forEach((cartItem) => {
                const product = localProducts.find(p => p.id === cartItem.productId);

                if (product)
                {
                    localCartTotal += cartItem.quantity * product.price;
                }
            })

            setCartTotal(localCartTotal);
            setProducts(localProducts);
            setCartItems(localCartItems);
        }



        loadCart().then(() => {console.log('Cart Loaded')});
    }, [isAuthenticated, claims]);

    const startShoppingClick = () => {
        navigate('/categories');
    }

    const editCartItemClick = async (cartItem: CartItem) =>  {
        const cartItemProduct = products.find(p => p.id === cartItem.productId);

        if (cartItemProduct == undefined)
        {
            toast.error('Unable to edit product');
            return;
        }

        setModalCartItem(cartItem)
        setModalProductName(cartItemProduct.name)
        setModalProductDesc(cartItemProduct.description);
        setModalCount(cartItem.quantity)
        setIsOpen(true);
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

    const removeCartItem = async (cartItem: CartItem) =>  {
        const response = await cartService.removeItemFromCart(cartItem);

        if (response.success)
        {
            setCartItems(cartItems.filter(c => c.id === cartItem.id));
            return;
        }

        toast.error(response.message);
    }

    const clearCart = async () => {
        const response = await cartService.clearCart(claims?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || '');

        if (response.success)
        {
            setCartItems([]);
            return;
        }

        toast.error(response.message);
    }

    const closeModal = async () => {
        setIsOpen(false);

        if (modalCartItem == undefined)
        {
            toast.error('Unable to update cart item');
            return;
        }

        modalCartItem.quantity = modalCount;

        const updateCartItemResponse = await cartService.updateItemInCart(modalCartItem);

        if (updateCartItemResponse.success)
        {
            const localCartItems = cartItems.filter(c => c.id === modalCartItem!.id);
            localCartItems.push(modalCartItem);

            //calculateCartTotal(products);
        } else if (updateCartItemResponse.validationErrors.length > 0)
        {
            updateCartItemResponse.validationErrors.forEach((validationError: string) => {
                toast.warn(validationError);
            });
        }
        else {
            toast.error(updateCartItemResponse.message);
        }
    }

    const cancelCloseModal = () => {
        setIsOpen(false);
    }

    const startStandardCheckout = async () => {
        setIsProcessing(true);
        const response = await orderService.createOrder(cartItems, PaymentSource.Standard);

        if (response.success && response.redirectUrl)
        {
            window.location.href = response.redirectUrl;
        }
        else
        {
            toast.error(response.message);
        }
        setIsProcessing(false);
    }

    if (!cartItems)
    {
        return <LoadingIcon/>;
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
                        <button className="btn btn-danger" onClick={clearCart}>Clear Cart</button>
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
                        const product = products.find(p => p.id === item.productId);

                        if (product == undefined)
                        {
                            return;
                        }

                        return (
                            <tr key={index}>
                                <td>{product.name}</td>
                                <td>{item.quantity}</td>
                                <td>${product.price.toFixed(2)}</td>
                                <td>${(product.price * item.quantity).toFixed(2)}</td>
                                <td>
                                    <button className="btn btn-primary" onClick={() => editCartItemClick(item)}>Edit
                                    </button>
                                    <button className="btn btn-danger ms-2" onClick={() => removeCartItem(item)}>X</button>
                                </td>
                            </tr>
                        )
                    })}
                    </tbody>
                </table>

                {cartItems.length != 0 ? (
                    <>
                        <div className="row w-100 text-center">
                            <h2>Total: ${cartTotal?.toFixed(2)}</h2>
                        </div>
                        <div className="row w-100 mt-2">
                            <div className="col-md-3"></div>

                            <div className="col-md-6">
                                <button className="btn btn-primary form-control" disabled>
                                    <i className="bi bi-paypal"></i> Checkout with PayPal
                                </button>
                            </div>
                        </div>
                        <div className="row w-100 mt-2">
                            <div className="col-md-3"></div>

                            <div className="col-md-6">
                                {isProcessing ? (
                                    <button type="submit" className="btn btn-success form-control">
                                        <span className="spinner-border spinner-border-sm" role="status"></span>
                                        Creating Order
                                    </button>
                                ) : (
                                    <button className="btn btn-success form-control" onClick={startStandardCheckout}>
                                        <i className="bi bi-cash"></i> Checkout
                                    </button>
                                )}
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

                <Modal isOpen={modalIsOpen} style={customStyles} contentLabel="Update Quantity">
                    <div className="simple-form mx-4 ">
                        <div className="row text-center">
                            <h1 className="card-title">{modalProductName}</h1>
                            <p className="card-text mt-2">{modalProductDesc}</p>
                        </div>
                        <div className="row mt-3">
                            <div className="form-group text-center">
                                <label htmlFor="quantity" className="h5">Quantity:</label>
                                <input type="number" className="form-control" id="quantity" name="quantity" min="1" value={modalCount}
                                       onChange={(e) => setModalCount((Number(e.target.value)))}/>
                            </div>
                        </div>

                        <div className="row mt-4">
                            <button type="submit" className="btn btn-primary form-control" onClick={closeModal}>Add to
                                Cart
                            </button>
                            <button className="btn btn-secondary form-control mt-2" onClick={cancelCloseModal}>Cancel
                            </button>
                        </div>
                    </div>
                </Modal>
                <ToastContainer/>
            </div>
        </>
    );
}

export default Cart;