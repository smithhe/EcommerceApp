import axiosInstance from "../AxiosInstance.ts";

const getAllProducts = async (categoryId: number) => {
    try {
        const response = await axiosInstance.get(`/api/product/all?categoryId=${categoryId}`);

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

const getProductById = async (productId: number) => {
    try {
        const response = await axiosInstance.get(`/api/product/get?ProductId=${productId}`);

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

const productService = {
    getAllProducts,
    getProductById
}

export default productService;