import { Link } from "react-router-dom";
import { useAuth } from "../AuthContext.tsx";
import {useEffect, useState} from "react";

const NavMenu = () => {
    const { isAuthenticated, claims } = useAuth();
    const [localIsAuthenticated, setIsAuthenticated] = useState<boolean>();

    useEffect(() => {
        setIsAuthenticated(isAuthenticated);
    }, [isAuthenticated]);

    const getDisplayName = () : string => {
        if (isAuthenticated) {
            const firstName = claims?.FirstName;
            const lastName = claims?.LastName;

            return `${firstName} ${lastName}`;
        }

        return "User";
    }

    return (
        <nav className="navbar navbar-expand-lg bg-dark-subtle">
            <div className="container">
                <Link className="navbar-brand" to="/">TechGear Forge</Link>
                <button className="navbar-toggler" type="button" data-bs-toggle="collapse"
                        data-bs-target="#navbarToggler">
                    <span className="navbar-toggler-icon"></span>
                </button>
                <div className="collapse navbar-collapse" id="navbarToggler">
                    <ul className="navbar-nav me-auto mb-2 mb-lg-0">
                        <li className="nav-item">
                            <Link className="nav-link" to="/">Home</Link>
                        </li>
                        <li className="nav-item">
                            <a className="nav-link" href="/Categories">Categories</a>
                        </li>
                    </ul>

                    <div className="d-flex">
                        <ul className="navbar-nav me-auto mb-2 mb-lg-0">
                            {localIsAuthenticated ? (
                                <ul className="navbar-nav me-auto mb-2 mb-lg-0">
                                    <li>
                                        <a className="nav-link" href='/Profile'> Welcome, {getDisplayName()}</a>
                                    </li>

                                    <li>
                                        <a className="nav-link" href="/Cart"><i className="bi bi-cart4"></i> Cart</a>
                                    </li>

                                    <li>
                                    <a className="nav-link" href="/Orders"><i className="bi bi-card-list"></i> Orders</a>
                                    </li>

                                    <li className="nav-item">
                                    <a className="nav-link" href="/logout"><i className="bi bi-box-arrow-in-right"></i> Logout</a>
                                    </li>
                                </ul>
                            ) : (
                                <ul className="navbar-nav me-auto mb-2 mb-lg-0">
                                    <li className="nav-item">
                                        <a className="nav-link" href="/login"><i
                                            className="bi bi-box-arrow-in-right"></i> Login</a>
                                    </li>
                                </ul>
                            )}
                        </ul>
                    </div>
                </div>
            </div>
        </nav>
    )
}

export default NavMenu