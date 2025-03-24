import { useState, useEffect } from 'react';
import { getUserBookings } from '../../services/BookingService';
import { useAuth } from '../../context/AuthContext';

const MyBookings = () => {
  const { token } = useAuth();
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchBookings = async () => {
      try {
        setLoading(true);
        const data = await getUserBookings(token);
        setBookings(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    if (token) fetchBookings();
  }, [token]);

  const calculateTotalAmount = (checkInDate, checkOutDate, pricePerNight) => {
    const checkIn = new Date(checkInDate);
    const checkOut = new Date(checkOutDate);
    const days = Math.ceil((checkOut - checkIn) / (1000 * 60 * 60 * 24));
    return days * pricePerNight;
  };

  return (
    <div className="container mt-4">
      <h2 className="mb-4">My Bookings</h2>

      {loading && <div className="alert alert-info">Loading bookings...</div>}
      {error && <div className="alert alert-danger">{error}</div>}

      {!loading && bookings.length === 0 && (
        <div className="alert alert-warning">No bookings found!</div>
      )}

      {!loading && bookings.length > 0 && (
        <div className="table-responsive">
          <table className="table table-bordered table-hover">
            <thead className="table-dark">
              <tr>
                <th>#</th>
                <th>Check-in Date</th>
                <th>Check-out Date</th>
                <th>Room Type</th>
                <th>Price/Night</th>
                <th>Total Amount</th>
                <th>Payment Status</th>
              </tr>
            </thead>
            <tbody>
              {bookings.map((booking, index) => (
                <tr key={booking.id}>
                  <td>{index + 1}</td>
                  <td>{new Date(booking.checkInDate).toLocaleDateString()}</td>
                  <td>{new Date(booking.checkOutDate).toLocaleDateString()}</td>
                  <td>{booking.room.roomType || 'N/A'}</td>
                  <td>${booking.room.pricePerNight || 0}</td>
                  <td>
                    ${calculateTotalAmount(
                      booking.checkInDate,
                      booking.checkOutDate,
                      booking.room.pricePerNight || 0
                    )}
                  </td>
                  <td>
                    {booking.isPaid ? (
                      <span className="badge bg-success">Paid</span>
                    ) : (
                      <span className="badge bg-danger">Pending</span>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default MyBookings;
