import { Routes, Route } from 'react-router-dom';
import { useAuth } from '../context/AuthContext'; // ✅ Import AuthContext
import Home from '../pages/Home';
import Login from '../features/auth/Login';
import Register from '../features/auth/Register';
import HotelList from '../features/hotel/HotelList';
import HotelDetails from '../features/hotel/HotelDetails';
import ProtectedRoute from './ProtectedRoute';
import BookingForm from '../features/booking/BookingForm';
import Checkout from '../features/payment/Checkout';
import Success from '../features/payment/Success';
import Cancel from '../features/payment/Cancel';
import MyBookings from '../features/booking/MyBookings';

function App() {
    // ✅ Get role from AuthContext
    const { user } = useAuth();
    const role = user?.role || 'Guest'; // Default to Guest if no user or role

    return (
        <Routes>
            {/* ✅ Public Routes */}
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />

            {/* ✅ Everyone can view Hotel List and Details */}
            <Route path="/hotels" element={<HotelList />} />
            <Route path="/hotel/:hotelId" element={<HotelDetails role={role} />} />

            {/* ✅ Protected Admin Route */}
            <Route element={<ProtectedRoute allowedRoles={['Admin']} />}>
                <Route path="/admin" element={<HotelList role={role} />} />
            </Route>

            <Route element={<ProtectedRoute allowedRoles={['User']} />}>
                <Route path="/user" element={<HotelList role={role} />} />
            </Route>

            {/* Other routes */}
            <Route path="/booking/user" element={<MyBookings />} />

            {/* ✅ Unauthorized Route */}
            <Route path="/unauthorized" element={<h2>Unauthorized Access</h2>} />

            {/* ✅ Protected Routes for User and Admin */}
            <Route element={<ProtectedRoute allowedRoles={['User', 'Admin']} />}>
                <Route path="/booking/:roomId" element={<BookingForm />} />
                <Route path="/checkout/:amount" element={<Checkout />} />
            </Route>

            {/* ✅ Success and Cancel Routes */}
            <Route path="/payment/success" element={<Success />} />
            <Route path="/cancel" element={<Cancel />} />
        </Routes>
    );
}

export default App;
