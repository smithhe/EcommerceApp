import axiosInstance from "../AxiosInstance.ts";
import {Review} from "../models/Review.ts";

const submitReview = async (review: Review) => {
    try {
        const response = await axiosInstance.post(`/api/review/create`, {
            ReviewToCreate: review
        });

        if (response.data) {
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

const removeReview = async (review: Review) => {
    try {
        const response = await axiosInstance.delete(`/api/review/delete`, {
            data: {ReviewToDelete: review}
        });

        if (response.data) {
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

const updateReview = async (review: Review) => {
    try {
        const response = await axiosInstance.put(`/api/review/update`, {
            ReviewToUpdate: review
        });

        if (response.data) {
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

const getUserReview = async (userName: string, productId: number) => {
    try {
        const response = await axiosInstance.get(`/api/review/user?UserName=${userName}&ProductId=${productId}`);

        if (response.data) {
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


const reviewService = {
    submitReview,
    removeReview,
    updateReview,
    getUserReview
}

export default reviewService;