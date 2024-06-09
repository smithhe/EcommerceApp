import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import authService from '../../AuthService';
import '../../styles/Register.css'

const Register: React.FC = () => {
    const [username, setUsername] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const [firstName, setFirstName] = useState<string>('');
    const [lastName, setLastName] = useState<string>('');
    const [email, setEmail] = useState<string>('');

    //const [error, setError] = useState<string>('');
    const navigate = useNavigate();

    let errorMessage: string = '';
    let displayError: boolean = false;

    let isProcessing: boolean = false;


    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        //setError('');

        displayError = false;
        isProcessing = true;

        try {
            const response = await authService.registerUser(username, firstName, lastName, email, password);

            if (response.success) {
                navigate('/login'); // Redirect to a protected route or dashboard
            }

            if (response.errors)
            {
                response.errors.forEach(error => {
                    errorMessage += "<div class=\"alert alert-danger\" role=\"alert\">"+ error +"</div>"
                })
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

        isProcessing = false;
    };

    return (
        <div className="container mt-5">
            <div className="row justify-content-center">

                {displayError ? (<div className="alert alert-danger text-center" role="alert">{errorMessage}</div>) : (<div></div>)}

                <div className="col-md-6">
                    <div className="login-form">
                        <h2 className="text-center mb-4">Login</h2>
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
                            <label htmlFor="username" className="form-label h5">First Name</label>
                            <input
                                type="text"
                                className="form-control"
                                value={firstName}
                                onChange={(e) => setFirstName(e.target.value)}
                            />
                        </div>

                        <div className="form-group">
                            <label htmlFor="username" className="form-label h5">Last Name</label>
                            <input
                                type="text"
                                className="form-control"
                                value={lastName}
                                onChange={(e) => setLastName(e.target.value)}
                            />
                        </div>

                        <div className="form-group">
                            <label htmlFor="username" className="form-label h5">Email</label>
                            <input
                                type="text"
                                className="form-control"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
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
                            <button type="button" className="btn btn-primary form-control mt-3">
                                <span className="spinner-border spinner-border-sm" role="status"></span>
                                Registering
                            </button>
                        ) : (
                            <button type="button" className="btn btn-primary form-control mt-3" onClick={handleSubmit}>Register</button>
                        )}
                    </div>
                </div>

            </div>
        </div>
    );
};

export default Register;
