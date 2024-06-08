import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
//import authService from '../../AuthService';
import '../../styles/Login.css'
import {useAuth} from "../../AuthContext.tsx";

const Login = () => {
    const [username, setUsername] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const [isProcessing, setIsProcessing] = useState<boolean>(false);
    const [error, setError] = useState<string>('');
    const [displayError, setDisplayError] = useState<boolean>(false);
    const navigate = useNavigate();
    const { isAuthenticated, login } = useAuth();

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();

        setDisplayError(false);
        setIsProcessing(true);

        try {
            const response = await login(username, password);

            if (response.success) {
                navigate('/'); // Redirect to a protected route or dashboard
            }

            if (response.message)
            {
                setError(response.message);
                setDisplayError(true);
            }
            else {
                setError('Unexpected Error Occurred')
                setDisplayError(true);
            }

        } catch (err) {
            setError('Unexpected Error Occurred')
            setDisplayError(true);
        }

        setIsProcessing(false);
    };

    if (isAuthenticated) {
        navigate('/');
    }

    return (
        <div className="container mt-5">
            <div className="row justify-content-center">

                {displayError ? (<div className="alert alert-danger text-center" role="alert">{error}</div>) : (<div></div>)}

                <div className="col-md-6">
                    <div className="login-form">
                        <h2 className="text-center mb-4">Login</h2>
                        <form onSubmit={handleSubmit}>
                            <div className="form-group">
                                <label htmlFor="username" className="form-label h5">Username</label>
                                <input
                                    type="text"
                                    className="form-control"
                                    value={username}
                                    onChange={(e) => setUsername(e.target.value)}
                                />
                            </div>
                            <div className="form-group">
                                <label htmlFor="password" className="form-label h5">Password</label>
                                <input
                                    type="password"
                                    className="form-control"
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                />
                            </div>
                            {isProcessing ? (
                                <button type="submit" className="btn btn-primary form-control mt-3">
                                    <span className="spinner-border spinner-border-sm" role="status"></span>
                                    Logging In
                                </button>
                            ) : (
                                <button type="submit" className="btn btn-primary form-control mt-3">Login</button>
                            )}
                        </form>

                        <p className="text-center mt-3">Don't have an account? <a href="/Register">Register here</a></p>
                    </div>
                </div>

            </div>
        </div>
    );
};

export default Login;
