import {useEffect, useState} from "react";
import {useLocation} from "react-router-dom";
import authService from "../../AuthService";

const ConfirmEmail = () => {
    //const { userId, emailToken } = useParams<{ userId?: string, emailToken?: string  }>();
    const location = useLocation();
    const searchParams = new URLSearchParams(location.search);
    const userId = searchParams.get('userId');
    const emailToken = searchParams.get('emailToken');

    const [message, setMessage] = useState<string | undefined>(undefined);

    useEffect(() => {

        const attemptEmailConfirmation = async() => {
            const response = await authService.confirmEmail(userId || '', emailToken || '');

            if (response.success) {
                setMessage("Email Confirmed Successfully!");
                return;
            }

            setMessage("An Error Occurred While Confirming Your Email");
        }

        attemptEmailConfirmation().then(() => {console.log('Confirm Email Complete')});
    });

    if (!message) {
        return <div className="row w-100 text-center">
            <style>{`
                    .loading-container {
                      display: flex;
                      flex-direction: column;
                      justify-content: center;
                      align-items: center;
                      height: 100%;
                    }
                    .spinner-border {
                      width: 5rem;
                      height: 5rem;
                      border-width: .5rem;
                    }
                    .loading-text {
                      margin-top: 1rem;
                      font-size: 1.5rem;
                      font-weight: bold;
                    }`}
            </style>
            <div className="col h-100">
                <div className="loading-container">
                    <div className="spinner-border text-primary" role="status">
                    </div>
                    <p className="loading-text">Confirming Email</p>
                </div>
            </div>
        </div>
    }

    return (
        <p>{message}</p>
    );
}

export default ConfirmEmail;