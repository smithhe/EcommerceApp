import {useEffect, useState} from "react";
import {Order} from "../models/Order.ts";
import {useNavigate, useParams} from "react-router-dom";
import orderService from "../services/OrderService.ts";
import LoadingIcon from "./childComponents/LoadingIcon.tsx";


const PaymentReturn = () => {
    const { orderId } = useParams<{ orderId?: string }>();
    const [newOrder, setNewOrder] = useState<Order>();
    const [showError, setShowError] = useState(false);
    const [error, setError] = useState<string>();
    const navigate = useNavigate();

    useEffect(() => {
        if (orderId == undefined)
        {
            return;
        }

        const loadOrder = async () => {
            setShowError(false);
            const response = await orderService.getOrderAfterSuccessfulCheckout(parseInt(orderId));

            if (response.success === false)
            {
                setShowError(true);
                setError(response.message);
            }

            setNewOrder(response.order);
        }

        loadOrder();
    });

    if (!newOrder) {
        return <LoadingIcon/>;
    }

    const getTotal = () : string => {
        let total = 0;
        newOrder.orderItems.forEach((item) => {
            total += item.quantity * item.price;
        });

        return total.toFixed(2);
    }

    const continueShoppingClick = () => {
        navigate('/categories');
    }

    return (
        <>
            {showError ? (
                <div className="container mt-5">
                    <div className="row justify-content-center">
                        <div className="alert alert-danger text-center" role="alert">
                            {error}
                        </div>
                    </div>
                </div>
            ) : (
                <div className="container mt-4">
                    <div className="row">
                        <div className="col-md-12 text-center">
                            <h1 className="display-4">Thank You!</h1>
                            <p className="lead">Your order has been placed successfully.</p>
                            <div className="alert alert-success" role="alert">
                                <h4 className="alert-heading">Order Confirmation</h4>
                                <p>Your order number is <strong>#{newOrder.id}</strong>. You will receive an email confirmation
                                    shortly.</p>
                            </div>
                        </div>
                    </div>

                    <div className="row mt-2">
                        <div className="col-md-8 offset-md-2">
                            <h4>Order Details</h4>
                            <div className="table-responsive">
                                <table className="table">
                                    <thead>
                                    <tr>
                                        <th>Product Name</th>
                                        <th>Quantity</th>
                                        <th>Price</th>
                                        <th>Total</th>
                                    </tr>
                                    </thead>
                                    <tbody>
                                    {newOrder.orderItems.map((item, index) => (
                                        <tr key={index}>
                                            <td>{item.productName}</td>
                                            <td>{item.quantity}</td>
                                            <td>{item.price.toFixed(2)}</td>
                                            <td>{(item.quantity * item.price).toFixed(2)}</td>
                                        </tr>
                                    ))}
                                    </tbody>
                                    <tfoot>
                                    <tr>
                                        <th colSpan={3}>Grand Total</th>
                                        <th>{getTotal()}</th>
                                    </tr>
                                    </tfoot>
                                </table>
                            </div>
                            <div className="text-center mt-4">
                                <button className="btn btn-primary" onClick={continueShoppingClick}>Continue Shopping</button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </>
    );
};

export default PaymentReturn;