import {SignInResponseResult} from "./models/SignInResponseResult.ts";
import axiosInstance from "./AxiosInstance.ts";

interface AuthenticateResponse {
    signInResult: SignInResponseResult;
    token: string;
    twoFactorToken: string;
}

const login = async (username: string, password: string) => {
    try {
        const response = await axiosInstance.post<AuthenticateResponse>(`/api/login`, { username, password });

        if (response == null || response.data == undefined) {
            return {
                success: false,
                message: "Unexpected Error Occurred"
            }
        }

        if (response.status == 401) {
            return {
                success: false,
                message: "Invalid Credentials"
            }
        }
        else if (response.status === 403) {
            return {
                success: false,
                message: "Account is Locked or Disabled"
            }
        }

        if (response.data.signInResult === SignInResponseResult.TwoFactorRequired) {
            return {
                success: false,
                message: "Two Factor Authentication Required"
            }
        }

        if (response.data.signInResult === SignInResponseResult.EmailNotConfirmed)
        {
            return {
                success: false,
                message: "Email Confirmation Required"
            }
        }

        //localStorage.setItem('authToken', JSON.stringify(response.data.token));

        return {
            success: true,
            token: response.data.token
        }

    } catch (error) {
        throw new Error('Login failed');
    }
}

const logout = async (username: string) => {
    try {
        await axiosInstance.post(`/api/logout`, { username });
    }
    catch (error) {
        console.log(error);
    }

    //
}

interface CreateUserResponse {
    success: boolean,
    confirmationLink: string
    errors: string[]
}

const registerUser = async (userName: string, firstName: string, lastName: string, emailAddress: string, password: string) => {
    const response = await axiosInstance.post<CreateUserResponse>(`/api/register`, {
        UserName: userName,
        FirstName: firstName,
        LastName: lastName,
        EmailAddress: emailAddress,
        Password: password
    });

    if (response.data != null)
    {
        return response.data;
    }

    return {
        success: false,
        errors: ['Unexpected Error Occurred']
    };
}

interface ConfirmEmailResponse {
    success: boolean,
    message: string
}

const confirmEmail = async (userId: string, emailToken: string) => {
    const response = await axiosInstance.post<ConfirmEmailResponse>(`/api/user/confirm-email`, {
        UserId: userId,
        EmailToken: emailToken
    });

    if (response.data != null)
    {
        return response.data;
    }

    return {
        success: false,
        errors: ['Unexpected Error Occurred']
    };
}

const isAuthenticated = (): boolean => {
    const token = localStorage.getItem('authToken');

    return !!token;
}


const authService = {
    login,
    logout,
    registerUser,
    confirmEmail,
    isAuthenticated
};

export default authService;