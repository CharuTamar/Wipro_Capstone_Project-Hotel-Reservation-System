// src/features/rooms/RoomService.js
import axios from 'axios';

const API_URL = '/api/Room';

export const RoomService = {

  getRoomsByHotelId: async (hotelId) => {
    try {
      const response = await axios.get(`${API_URL}/${hotelId}`);
      return response.data || [];
    } catch (error) {
      console.error('Failed to load rooms:', error);
      return [];
    }
  },
  // ✅ Add Room
  addRoom: async (newRoom) => {
    try {
      const response = await axios.post(API_URL, newRoom);
      return response.data;
    } catch (error) {
      console.error('Error adding room:', error);
      throw error;
    }
  },
  // ✅ Update Room
  updateRoom: async (roomId, updatedRoom) => {
    try {
      const response = await axios.put(`${API_URL}/${roomId}`, updatedRoom);
      return response.data;
    } catch (error) {
      console.error('Error updating room:', error);
      throw error;
    }
  },
  // ✅ Delete Room
  deleteRoom: async (roomId) => {
    try {
      const response = await axios.delete(`${API_URL}/${roomId}`);
      return response.data;
    } catch (error) {
      console.error('Error deleting room:', error);
      throw error;
    }
  }
};
