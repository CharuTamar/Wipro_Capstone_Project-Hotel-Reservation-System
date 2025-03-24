import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

const Login = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [role, setRole] = useState('');
    const { login } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        const params = new URLSearchParams(location.search);
        const roleFromParams = params.get('role');
        setRole(roleFromParams || '');
    }, [location]);

    const handleLogin = async (e) => {
        e.preventDefault();
        setError('');
    
        try {
            await login({ email, password });
            if (role === 'admin') {
                navigate('/admin');
            } else {
                navigate('/user');
            }
        } catch (error) {
            setError(error.message); // Display backend error
        }
    };

    return (
        <div style={{ padding: '20px', maxWidth: '400px', margin: 'auto' }}>
            <h2>{role === 'admin' ? 'Admin Login' : 'User Login'}</h2>
            <form onSubmit={handleLogin}>
                <input
                    type="email"
                    placeholder="Email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                    style={{
                        display: 'block',
                        width: '100%',
                        padding: '10px',
                        marginBottom: '10px',
                    }}
                />
                <input
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                    style={{
                        display: 'block',
                        width: '100%',
                        padding: '10px',
                        marginBottom: '10px',
                    }}
                />
                <button
                    type="submit"
                    style={{
                        width: '100%',
                        padding: '10px',
                        backgroundColor: '#4CAF50',
                        color: 'white',
                        border: 'none',
                        cursor: 'pointer',
                    }}
                >
                    {role === 'admin' ? 'Login as Admin' : 'Login as User'}
                </button>
                {error && (
                    <p style={{
                        color: 'red',
                        marginTop: '10px',
                        padding: '5px',
                        backgroundColor: '#ffebeb',
                        border: '1px solid #ff4d4f',
                        borderRadius: '5px'
                    }}>
                        {error}
                    </p>
                )}
            </form>
        </div>
    );
};

export default Login;
