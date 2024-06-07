import axiosInstance from "../AxiosInstance.ts";

const getAllCategories = async () => {
    try {
        const response = await axiosInstance.get(`/api/category/all`);

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

const categoryService = {
    getAllCategories
}

export default categoryService;