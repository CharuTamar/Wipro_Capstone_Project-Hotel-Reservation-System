import axios from 'axios';

const API_URL = 'http://localhost:5099/api/Hotel';

// ✅ Get all hotels
export const getHotels = async () => {
  const response = await axios.get(API_URL);
  return response.data;
};

// ✅ Get hotel by ID
export const getHotelById = async (id) => {
  const response = await axios.get(`${API_URL}/${id}`);
  return response.data;
};

// ✅ Add hotel
export const addHotel = async (hotel) => {
  const response = await axios.post(API_URL, hotel);
  return response.data;
};

// ✅ Update hotel
export const updateHotel = async (id, updatedHotel) => {
  const response = await axios.put(`${API_URL}/${id}`, updatedHotel);
  return response.data;
};

// ✅ Delete hotel
export const deleteHotel = async (id) => {
  const response = await axios.delete(`${API_URL}/${id}`);
  return response.data;
};
