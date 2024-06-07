import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import authService from '../../AuthService';
import '../../styles/Login.css'

const Login: React.FC = () => {
    const [username, setUsername] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const [isProcessing, setIsProcessing] = useState<boolean>(false);
    //const [error, setError] = useState<string>('');
    const navigate = useNavigate();

    let errorMessage: string = '';
    let displayError: boolean = false;


    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        //setError('');

        displayError = false;
        setIsProcessing(true);

        try {
            const response = await authService.login(username, password);

            if (response.success) {
                navigate('/'); // Redirect to a protected route or dashboard
            }

            if (response.message)
            {
                errorMessage = response.message;
                displayError = true;
            }
            else {
                errorMessage = 'Unexpected Error Occurred';
                displayError = true;
            }

        } catch (err) {
            //setError('Invalid credentials');
            errorMessage = 'Unexpected Error Occurred';
            displayError = true;
        }

        setIsProcessing(false);
    };

    return (
        <div className="container mt-5">
            <div className="row justify-content-center">

                {displayError ? (<div className="alert alert-danger text-center" role="alert">{errorMessage}</div>) : (<div></div>)}

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
