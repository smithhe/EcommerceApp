import axios from 'axios';

// Create an Axios instance
const axiosInstance = axios.create({
    baseURL: 'http://localhost:5128', // Replace with your API base URL
});

// Add a request interceptor
axiosInstance.interceptors.request.use(
    (config) => {
        // Get the token from local storage
        let token = localStorage.getItem('authToken');

        // If token exists, add it to the request headers
        if (token != null) {
            token = token.replace(/['"]+/g, '');
            config.headers.Authorization = `Bearer ${token}`;
        }
        else {
            console.log('no token found');
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

export default axiosInstance;
