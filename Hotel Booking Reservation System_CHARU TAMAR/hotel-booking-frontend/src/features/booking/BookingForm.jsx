import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { createBooking } from '../../services/BookingService';
import { useAuth } from '../../context/AuthContext';

const BookingForm = () => {
  const { roomId } = useParams();
  const navigate = useNavigate();
  const { token } = useAuth();
  const [bookingData, setBookingData] = useState({ checkInDate: '', checkOutDate: '' });

  const handleChange = (e) => {
    const { name, value } = e.target;
    const formattedDate = new Date(value).toISOString().slice(0, 10);
    setBookingData({ ...bookingData, [name]: formattedDate });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      if (!token) {
        throw new Error('You need to be logged in to make a booking.');
      }

      const checkIn = new Date(bookingData.checkInDate);
      const checkOut = new Date(bookingData.checkOutDate);

      // ✅ Frontend validation for date conflict
      if (checkOut <= checkIn) {
        throw new Error('Check-out date must be after check-in date.');
      }

      const booking = await createBooking(roomId, bookingData, token);

      if (booking && booking.room) {
        const numberOfNights = Math.ceil(
          (new Date(booking.checkOutDate) - new Date(booking.checkInDate)) /
          (1000 * 60 * 60 * 24)
        );
        const totalAmount = booking.room.pricePerNight * numberOfNights;

        alert('✅ Booking created successfully!');
        navigate(`/checkout/${totalAmount}`);
      } else {
        throw new Error('Failed to retrieve booking details.');
      }
    } catch (error) {
      console.error('❌ Booking failed:', error.message);
      alert(error.message);
    }
  };

  return (
    <form className="container mt-4 p-4 border rounded shadow bg-light" onSubmit={handleSubmit}>
      <h2 className="mb-4 text-center">Book Your Stay</h2>

      {/* Check-In Date */}
      <div className="mb-3">
        <label htmlFor="checkInDate" className="form-label">
          Check-In Date
        </label>
        <input
          type="date"
          id="checkInDate"
          name="checkInDate"
          value={bookingData.checkInDate}
          onChange={handleChange}
          className="form-control"
          required
        />
      </div>

      {/* Check-Out Date */}
      <div className="mb-3">
        <label htmlFor="checkOutDate" className="form-label">
          Check-Out Date
        </label>
        <input
          type="date"
          id="checkOutDate"
          name="checkOutDate"
          value={bookingData.checkOutDate}
          onChange={handleChange}
          className="form-control"
          required
        />
      </div>

      {/* Submit Button */}
      <div className="d-grid">
        <button type="submit" className="btn btn-primary">
          Confirm Booking
        </button>
      </div>
    </form>
  );
};

export default BookingForm;
