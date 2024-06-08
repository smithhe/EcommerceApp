import {useNavigate, useParams} from "react-router-dom";
import {useEffect, useState} from "react";
import {Order} from "../models/Order.ts";
import orderService from "../services/OrderService.ts";
import {toast, ToastContainer} from "react-toastify";
import DateUtils from "../utils/DateUtils.ts";
import {OrderStatus} from "../models/OrderStatus.ts";


const OrderDetail = () => {
    const { orderId } = useParams<{ orderId?: string }>();

    const navigate = useNavigate();

    const [order, setOrder] = useState<Order | null>(null);

    useEffect(() => {
        if (!orderId) {
            console.error("orderId is missing");
            return;
        }

        const loadOrder = async(orderId: number) => {
            const response = await orderService.getOrderById(orderId);

            if (response.success === false) {
                toast.error(response.message);
                return;
            }

            setOrder(response.order);
        }


        loadOrder(Number.parseInt(orderId)).then(() => {});
    }, []);

    const backToOrdersClick = () => {
        navigate('/orders');
    }

    if (!order)
    {
        return <p><em>Loading...</em></p>;
    }

    return (
        <div className="container bg-white shadow-sm rounded-3">
            <div className="header-container">
                <h2>Order Details</h2>
                <button className="btn btn-borderless" onClick={backToOrdersClick}>
                    <i className="bi bi-arrow-left"></i> Back to Orders
                </button>
            </div>

            <div className="card mb-4">
                <div className="card-body">
                    <div className="row">
                        <div className="col-md-6 mb-3 mb-md-0">
                            <p>
                                <strong>Order ID:</strong> {order.id}
                            </p>
                            <p>
                                <strong>Creation Date:</strong> {DateUtils.formatDate(order.createdDate)}
                            </p>
                        </div>
                        <div className="col-md-6">
                            <p>
                                <strong>Status:</strong> {OrderStatus[order.status]}
                            </p>
                            <p><strong>Total:</strong> ${order.total.toFixed(2)}</p>
                        </div>
                    </div>
                </div>
            </div>

            <h3 className="mb-3">Items in Order</h3>
            <div className="table-responsive">
                <table className="table table-striped rounded-3">
                    <thead>
                    <tr>
                        <th scope="col">#</th>
                        <th scope="col">Item Name</th>
                        <th scope="col">Quantity</th>
                        <th scope="col">Price</th>
                    </tr>
                    </thead>
                    <tbody>
                    {
                        order.orderItems.map((item, index) => (
                            <tr key={index}>
                                <th scope="row">{item.id}</th>
                                <td>{item.productName}</td>
                                <td>{item.quantity}</td>
                                <td>${item.price}</td>
                            </tr>
                        ))
                    }
                    </tbody>
                </table>
            </div>

            <ToastContainer/>
        </div>
    );
}

export default OrderDetail;