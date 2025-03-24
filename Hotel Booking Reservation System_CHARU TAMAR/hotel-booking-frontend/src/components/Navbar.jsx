import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Navbar = () => {
    const { user, logout } = useAuth();

    return (
        <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
            <div className="container-fluid">
                <Link className="navbar-brand" to="/">
                    🏨 Hotel Booking System By Charu
                </Link>
                <button
                    className="navbar-toggler"
                    type="button"
                    data-bs-toggle="collapse"
                    data-bs-target="#navbarNav"
                    aria-controls="navbarNav"
                    aria-expanded="false"
                    aria-label="Toggle navigation"
                >
                    <span className="navbar-toggler-icon"></span>
                </button>
                <div className="collapse navbar-collapse" id="navbarNav">
                    <ul className="navbar-nav ms-auto">
                        {user ? (
                            <>
                                {user.role === 'User' && (
                                    <>
                                        <li className="nav-item">
                                            <Link className="nav-link" to="/booking/user">
                                                My Bookings
                                            </Link>
                                        </li>
                                        <li className="nav-item">
                                            <Link className="nav-link" to="/user">
                                                User Dashboard
                                            </Link>
                                        </li>
                                    </>
                                )}
                                {user.role === 'Admin' && (
                                    <li className="nav-item">
                                        <Link className="nav-link" to="/admin">
                                            Admin Dashboard
                                        </Link>
                                    </li>
                                )}
                                <li className="nav-item">
                                    <button
                                        className="nav-link btn btn-link"
                                        onClick={logout}
                                        style={{ color: 'white', textDecoration: 'none' }}
                                    >
                                        Logout
                                    </button>
                                </li>
                            </>
                        ) : (
                            <>
                                <li className="nav-item">
                                    <Link className="nav-link" to="/">
                                        Dashboard
                                    </Link>
                                </li>
                                <li className="nav-item">
                                    <Link className="nav-link" to="/register">
                                        Register
                                    </Link>
                                </li>
                                {/* 👇 Login as User and Admin options */}
                                <li className="nav-item">
                                    <Link className="nav-link" to="/login?role=user">
                                        Login as User
                                    </Link>
                                </li>
                                <li className="nav-item">
                                    <Link className="nav-link" to="/login?role=admin">
                                        Login as Admin
                                    </Link>
                                </li>
                            </>
                        )}
                    </ul>
                </div>
            </div>
        </nav>
    );
};

export default Navbar;
