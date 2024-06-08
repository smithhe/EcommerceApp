import {useEffect, useState} from "react";
import {Order} from "../models/Order.ts";
import {OrderStatus} from "../models/OrderStatus.ts";
import {useNavigate} from "react-router-dom";
import orderService from "../services/OrderService.ts";
import {useAuth} from "../AuthContext.tsx";
import {toast, ToastContainer} from "react-toastify";


const Orders = () => {
    const navigate = useNavigate();
    const { isAuthenticated, claims} = useAuth();

    const [orders, setOrders] = useState<Order[] | undefined>(undefined);

    useEffect(() => {
        if (isAuthenticated === false)
        {
            console.log('Unauthenticated');
            return;
        }

        const loadOrders = async () => {
            const response = await orderService.getUserOrders(claims?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || '');

            if (response.success === false)
            {
                toast.error(response.message);
                return;
            }

            setOrders(response.orders);
        }

        loadOrders().then(() => {
            console.log('Orders Loaded');
        })
    }, [isAuthenticated, claims]);

    const formatDate = (dateString: string): string => {
        //6/6/2024 10:39:00 PM
        const date = new Date(dateString);

        const year = date.getFullYear();
        const month = (date.getMonth() + 1).toString().padStart(2, '0'); // Months are 0-based
        const day = date.getDate().toString().padStart(2, '0');

        let hours = date.getHours();
        const minutes = date.getMinutes().toString().padStart(2, '0');

        const ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12;
        hours = hours ? hours : 12; // The hour '0' should be '12'
        const strHours = hours.toString().padStart(2, '0');

        return `${month}/${day}/${year} ${strHours}:${minutes} ${ampm}`;
    }

    const orderDetailsClick = (orderId: number) => {
        navigate(`/orderdetail/${orderId}`)
    }

    const cancelOrderClick = async (order: Order) => {
        if (order.status != OrderStatus.Processing)
        {
            toast.warn('Order cannot be cancelled at this time.');
            return;
        }

        order.status = OrderStatus.Cancelled;

        const response = await orderService.updateOrder(order);

        if (response.success)
        {
            const getUserOrdersResponse = await orderService.getUserOrders(claims?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || '');

            if (getUserOrdersResponse.success)
            {
                setOrders(response.orders);
            }
        }
        else {
            toast.error(response.message);
        }
    }

    if (orders == undefined) {
        return <p><em>Loading...</em></p>;
    }

    return (
        <div className="container mt-4">
            <div className="row w-100 mt-4">
                <div className="col-md-6">
                    <h1>
                        <i className="bi bi-card-list"></i> Orders
                    </h1>
                </div>
            </div>

            <table className="table table-striped">
                <thead>
                <tr>
                    <th className="text-center">Order ID</th>
                    <th className="text-center">Date Placed</th>
                    <th className="text-center">Total</th>
                    <th className="text-center">Status</th>
                    <th className="text-center">Actions</th>
                </tr>
                </thead>
                <tbody>
                {
                    orders.map((order, index) => (
                        <tr key={index}>
                            <td className="text-center">{order.id}</td>
                            <td className="text-center">{formatDate(order.createdDate)}</td>
                            <td className="text-center">{order.total.toFixed(2)}</td>
                            <td className="text-center">{OrderStatus[order.status]}</td>
                            <td>
                                <div className="row text-center">
                                    <div className="col-2">&nbsp;</div>
                                    <div className="col-4">
                                        <button className="btn btn-primary" onClick={() => orderDetailsClick(order.id)}>Order Details</button>
                                    </div>
                                    <div className="col-4">
                                        {order.status === OrderStatus.Processing ? (
                                            <button className="btn btn-danger" onClick={() => cancelOrderClick(order)}>Cancel Order</button>
                                        ) : (
                                            <button className="btn btn-danger" disabled>Cancel Order</button>
                                        )}
                                    </div>
                                    <div className="col-2">&nbsp;</div>
                                </div>
                            </td>
                        </tr>
                    ))
                }
                </tbody>
            </table>

            <ToastContainer/>
        </div>
    );
}

export default Orders;