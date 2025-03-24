import axios from 'axios';

const API_URL = 'http://localhost:5099/api/Booking';

// ✅ Get headers from token
const getAuthHeaders = (token) => {
  if (!token) {
    throw new Error('Unauthorized: No token available');
  }
  return {
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    withCredentials: true,
  };
};

// ✅ Create Booking
export const createBooking = async (roomId, bookingData, token) => {
    try {
      const response = await axios.post(
        `http://localhost:5099/api/Booking/${roomId}`,
        bookingData,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      return response.data;
    } catch (error) {
      // ✅ Extract and display the error message from the backend response
      console.error(`❌ Error ${error.response.status}:`, error.response.data?.message);
      throw new Error(error.response.data?.message || 'Failed to create booking');
    }
  };
  

// ✅ Update Booking
export const updateBooking = async (id, data, token) => {
  try {
    const response = await axios.put(`${API_URL}/${id}`, data, getAuthHeaders(token));
    return response.data;
  } catch (error) {
    handleRequestError(error);
  }
};

// ✅ Cancel Booking
export const cancelBooking = async (id, token) => {
  try {
    const response = await axios.delete(`${API_URL}/${id}`, getAuthHeaders(token));
    return response.data;
  } catch (error) {
    handleRequestError(error);
  }
};

// ✅ Get User Bookings
export const getUserBookings = async (token) => {
    try {
      const response = await axios.get(`${API_URL}/user`, getAuthHeaders(token));
      return response.data;
    } catch (error) {
      handleRequestError(error);
    }
  };
  

// ✅ Handle Errors Consistently
const handleRequestError = (error) => {
  if (error.response) {
    const { status, data } = error.response;
    console.error(`❌ Error ${status}:`, data);

    // ✅ Custom error messages based on status
    if (status === 403) throw new Error('You don’t have permission to book this room.');
    if (status === 400) throw new Error('Invalid request data.');
    throw new Error(data?.message || 'Request failed.');
  } else {
    throw new Error('Network error. Please try again.');
  }
};
