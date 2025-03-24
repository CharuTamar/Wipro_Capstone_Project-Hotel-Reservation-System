const Footer = () => {
    return (
        <footer className="p-3 bg-dark text-light text-center"
            style={{
                position: 'fixed',
                bottom: 0,
                left: 0,
                width: '100%',
                zIndex: 10
            }}
        >
            © {new Date().getFullYear()} Hotel Booking. All Rights Reserved.
        </footer>
    );
}

export default Footer;
