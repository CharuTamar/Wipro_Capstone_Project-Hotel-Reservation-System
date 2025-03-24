import React from 'react';
import { Link } from 'react-router-dom';

const HeroSection = () => {
    return (
        <div className="text-center my-5">
            <h1 className="display-4 fw-bold">Welcome to Our Hotel Booking Platform</h1>
            <p className="lead">
                Discover the best hotels at unbeatable prices. Book now and enjoy a seamless experience!
            </p>
            <Link to="/login" className="btn btn-primary btn-lg me-2">
                Get Started
            </Link>
            <Link to="/register" className="btn btn-outline-secondary btn-lg">
                Register Now
            </Link>
        </div>
    );
}

export default HeroSection;