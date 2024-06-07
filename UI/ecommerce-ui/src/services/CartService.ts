import {CartItem} from "../models/CartItem.ts";
import axiosInstance from "../AxiosInstance.ts";

const addItemToCart = async (cartItem: CartItem) => {
    try {
        const response = await axiosInstance.post(`/api/cartitem/create`, {
            CartItemToCreate: cartItem
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

const removeItemFromCart = async (cartItem: CartItem) => {
    try {
        const response = await axiosInstance.delete(`/api/cartitem/delete`, {
            data: { CartItemToDelete: cartItem }
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

const clearCart = async (userId: string) => {
    try {
        const response = await axiosInstance.delete(`/api/cartitem/user/delete`, {
            data: { UserId: userId }
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

const updateItemInCart = async (cartItem: CartItem) => {
    try {
        const response = await axiosInstance.put(`/api/cartitem/update`, {
            CartItemToUpdate: cartItem
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

const getItemsInCart = async (userId: string) => {
    try {
        const response = await axiosInstance.get(`/api/cartitem/all?UserId=${userId}`);

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

const cartService = {
    addItemToCart,
    removeItemFromCart,
    clearCart,
    updateItemInCart,
    getItemsInCart
}

export default cartService;