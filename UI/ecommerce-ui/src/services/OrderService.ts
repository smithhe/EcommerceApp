import axiosInstance from "../AxiosInstance.ts";
import {Order} from "../models/Order.ts";
import {CartItem} from "../models/CartItem.ts";
import {PaymentSource} from "../models/PaymentSource.ts";

const getUserOrders = async (userId: string) => {
    try {
        const response = await axiosInstance.get(`/api/order/user/all?UserId=${userId}`);

        if (response.status >= 200 && response.status < 300) {
            return response.data;
        }

        return {
            success: false,
            message: response.data.message || "Unexpected Error Occurred",
            validationErrors: []
        };
    } catch (error) {
        return {
            success: false,
            message: "Unexpected Error Occurred",
            validationErrors: []
        };
    }
}

const updateOrder = async (order: Order) => {
    try {
        const response = await axiosInstance.put(`/api/order/update`, {
            OrderToUpdate: order
        });

        if (response.status >= 200 && response.status < 300) {
            return response.data;
        }

        return {
            success: false,
            message: response.data.message || "Unexpected Error Occurred",
            validationErrors: []
        };
    } catch (error) {
        return {
            success: false,
            message: "Unexpected Error Occurred",
            validationErrors: []
        };
    }
}

const getOrderById = async (orderId: number) => {
    try {
        const response = await axiosInstance.get(`/api/order?Id=${orderId}`);

        if (response.status >= 200 && response.status < 300) {
            return response.data;
        }

        return {
            success: false,
            message: response.data.message || "Unexpected Error Occurred",
            validationErrors: []
        };
    } catch (error) {
        return {
            success: false,
            message: "Unexpected Error Occurred",
            validationErrors: []
        };
    }
}

const getOrderAfterSuccessfulCheckout = async (orderId: number) => {
    try {
        const response = await axiosInstance.get(`/api/checkout/order?Id=${orderId}`);

        if (response.status >= 200 && response.status < 300) {
            return response.data;
        }

        return {
            success: false,
            message: response.data.message || "Unexpected Error Occurred",
            validationErrors: []
        };
    } catch (error) {
        return {
            success: false,
            message: "Unexpected Error Occurred",
            validationErrors: []
        };
    }
}

const createOrder = async (cartItems: CartItem[], paymentSource: PaymentSource) => {
    try {
        const response = await axiosInstance.post(`/api/order/create`, {
            CartItems: cartItems,
            PaymentSource: paymentSource
        });

        if (response.status >= 200 && response.status < 300) {
            return response.data;
        }

        return {
            success: false,
            message: response.data.message || "Unexpected Error Occurred",
            validationErrors: []
        };
    } catch (error) {
        return {
            success: false,
            message: "Unexpected Error Occurred",
            validationErrors: []
        };
    }
}



const orderService = {
    getUserOrders,
    updateOrder,
    getOrderById,
    getOrderAfterSuccessfulCheckout,
    createOrder
}

export default orderService;