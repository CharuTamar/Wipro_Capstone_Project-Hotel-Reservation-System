import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getHotelById } from '../../services/HotelService';
import { RoomService } from '../../features/rooms/RoomService';
import RoomList from '../rooms/RoomList';

const HotelDetails = ({ role }) => {
  const { hotelId } = useParams();
  const [hotel, setHotel] = useState(null);
  const [rooms, setRooms] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchHotelDetails = async () => {
      try {
        setLoading(true);
        const hotelData = await getHotelById(hotelId);
        setHotel(hotelData);

        const roomData = await RoomService.getRoomsByHotelId(hotelId);
        setRooms(Array.isArray(roomData) ? roomData : []);
      } catch (error) {
        console.error('Failed to load hotel details:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchHotelDetails();
  }, [hotelId]);

  // ✅ Handle Add Room (Admin Only)
  const handleAddRoom = async (newRoom) => {
    try {
      // ✅ Add hotelId to the new room object
      const roomToAdd = {
        ...newRoom,
        hotelId: Number(hotelId), // Ensure hotelId is passed as a number
        pricePerNight: Number(newRoom.pricePerNight),
      };

      const addedRoom = await RoomService.addRoom(roomToAdd);

      // ✅ Update state with the new room
      setRooms((prev) => [...prev, addedRoom]);
      console.log('Room added successfully:', addedRoom);
    } catch (error) {
      console.error('Error adding room:', error);
    }
  };

  // ✅ Handle Update Room (Admin Only)
  const handleUpdateRoom = async (roomId) => {
    const updatedRoomType = prompt('Enter new room type:');
    const updatedPrice = prompt('Enter new price per night:');
    const updatedAvailability = prompt('Is the room available? (true/false)');

    if (!updatedRoomType || !updatedPrice || updatedAvailability === null) return;

    try {
      const updatedRoom = await RoomService.updateRoom(roomId, {
        roomType: updatedRoomType,
        pricePerNight: Number(updatedPrice),
        isAvailable: updatedAvailability === 'true',
      });

      setRooms((prev) =>
        prev.map((room) => (room.id === roomId ? updatedRoom : room))
      );
    } catch (error) {
      console.error('Error updating room:', error);
    }
  };

  // ✅ Handle Delete Room (Admin Only)
  const handleDeleteRoom = async (roomId) => {
    if (window.confirm('Are you sure you want to delete this room?')) {
      try {
        await RoomService.deleteRoom(roomId);
        setRooms((prev) => prev.filter((room) => room.id !== roomId));
      } catch (error) {
        console.error('Error deleting room:', error);
      }
    }
  };

  if (loading) return <p>Loading hotel details...</p>;
  if (!hotel) return <p>No hotel details available.</p>;

  return (
    <div className="container mt-4">
      {/* ✅ Hotel Details */}
      <h2>{hotel.name}</h2>
      <p>{hotel.description}</p>
      <p>
        ⭐ {hotel.rating} | 💰 {hotel.pricePerNight} per night
      </p>

      {/* ✅ Room List */}
      <RoomList
        rooms={rooms}
        role={role}
        onAddRoom={handleAddRoom} 
        onUpdateRoom={handleUpdateRoom}
        onDeleteRoom={handleDeleteRoom}
      />
    </div>
  );
};

export default HotelDetails;
