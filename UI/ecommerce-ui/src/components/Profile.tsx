import {useEffect, useState} from "react";
import {useAuth} from "../AuthContext.tsx";
import {toast, ToastContainer} from "react-toastify";


const Profile = () => {
    const {isAuthenticated, claims, updateProfile, updatePassword} = useAuth();

    const [userName, setUserName] = useState('');
    const [newUsername, setNewUsername] = useState('');
    const [email, setEmail] = useState('');
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');

    const [currentPassword, setCurrentPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [newPasswordConfirm, setNewPasswordConfirm] = useState('');

    useEffect(() => {
        if (!isAuthenticated) {
            return;
        }

        setUserName(claims?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || '');
        setNewUsername(claims?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || '');
        setEmail(claims?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] || '');
        setFirstName(claims?.FirstName || '');
        setLastName(claims?.LastName || '');

    }, [isAuthenticated, claims, updateProfile, updatePassword]);

    const updateProfileClick = async () => {
        if ((firstName === null || firstName === undefined || firstName.trim() === '')
            || (lastName === null || lastName === undefined || lastName.trim() === '')
            || (newUsername === null || newUsername === undefined || newUsername.trim() === '')
            || (email === null || email === undefined || email.trim() === ''))
        {
            toast.warn('All field must have a value');
        }

        const response = await updateProfile(firstName, lastName, newUsername, userName, email);

        if (response.success) {
            toast.success(response.message);
            return;
        }

        if (response.validationErrors.length > 0) {
            response.validationErrors.forEach((error: string) => {
                toast.warn(error);
            });
        }

        if (response.message)
        {
            toast.error(response.message);
        }
    }

    const updatePasswordClick = async () => {
        if ((currentPassword === null || currentPassword === undefined || currentPassword.trim() === '')
        || (newPassword === null || newPassword === undefined || newPassword.trim() === '')
        || (newPasswordConfirm === null || newPasswordConfirm === undefined || newPasswordConfirm.trim() === ''))
        {
            toast.warn("All Password Fields are Required");
        }

        if (newPassword !== newPasswordConfirm) {
            toast.error('New Password and Confirmation do not match');
            return;
        }

        const response = await updatePassword(userName, currentPassword, newPassword);

        setCurrentPassword('');
        setNewPassword('');
        setNewPasswordConfirm('');

        if (response.success) {
            toast.success(response.message);
            return;
        }

        if (response.validationErrors.length > 0) {
            response.validationErrors.forEach((error: string) => {
                toast.warn(error);
            });
        }

        if (response.message)
        {
            toast.error(response.message);
        }
    }

    if (userName == '')
    {
        return <p><em>Loading...</em></p>;
    }

    return (
        <div className="container py-5">
            <div className="card mb-4 shadow-sm">
                <div className="card-body">
                    <h2 className="card-title text-center mb-4">User Profile</h2>
                    <div className="mb-3">
                        <label htmlFor="username" className="form-label">Username</label>
                        <input type="text" className="form-control" id="username" placeholder="Enter your username"
                               onChange={(e) => setNewUsername(e.target.value)} value={newUsername}/>
                    </div>
                    <div className="mb-3">
                        <label htmlFor="email" className="form-label">Email</label>
                        <input type="email" className="form-control" id="email" placeholder="Enter your email"
                               onChange={(e) => setEmail(e.target.value)} value={email}/>
                    </div>
                    <div className="row">
                        <div className="mb-3 col-md-6">
                            <label htmlFor="firstName" className="form-label">First Name</label>
                            <input type="text" className="form-control" id="firstName" placeholder="Enter your first name"
                                   onChange={(e) => setFirstName(e.target.value)} value={firstName}/>
                        </div>
                        <div className="mb-3 col-md-6">
                            <label htmlFor="lastName" className="form-label">Last Name</label>
                            <input type="text" className="form-control" id="lastName" placeholder="Enter your last name"
                                   onChange={(e) => setLastName(e.target.value)} value={lastName}/>
                        </div>
                    </div>
                    <div className="text-center">
                        <button type="button" className="btn btn-primary" onClick={updateProfileClick}>Update Profile</button>
                    </div>
                </div>
            </div>

            <div className="card mb-4 shadow-sm">
                <div className="card-body">
                    <h2 className="card-title text-center mb-4">Change Password</h2>
                    <div className="mb-3">
                        <label htmlFor="currentPassword" className="form-label">Current Password</label>
                        <input type="password" className="form-control" id="currentPassword"
                               placeholder="Enter current password"
                               onChange={(e) => setCurrentPassword(e.target.value)}/>
                    </div>
                    <div className="mb-3">
                        <label htmlFor="newPassword" className="form-label">New Password</label>
                        <input type="password" className="form-control" id="newPassword"
                               placeholder="Enter new password"
                               onChange={(e) => setNewPassword(e.target.value)}/>
                    </div>
                    <div className="mb-3">
                        <label htmlFor="confirmNewPassword" className="form-label">Confirm New Password</label>
                        <input type="password" className="form-control" id="confirmNewPassword"
                               placeholder="Confirm new password"
                               onChange={(e) => setNewPasswordConfirm(e.target.value)}/>
                    </div>
                    <div className="text-center">
                        <button type="button" className="btn btn-primary" onClick={updatePasswordClick}>Update
                            Password
                        </button>
                    </div>
                </div>
            </div>

            <ToastContainer/>
        </div>
    );
}

export default Profile;