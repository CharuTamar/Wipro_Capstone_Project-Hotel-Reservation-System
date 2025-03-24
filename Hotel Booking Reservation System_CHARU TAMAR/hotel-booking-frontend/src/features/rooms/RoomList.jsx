import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

const RoomList = ({ rooms, role, onAddRoom, onUpdateRoom, onDeleteRoom }) => {
  const { user } = useAuth();
  const navigate = useNavigate();

  // ✅ State for New Room Form
  const [newRoom, setNewRoom] = useState({
    roomType: '',
    pricePerNight: '',
    isAvailable: true,
  });

  // ✅ Handle Booking (User Only)
  const handleBookNow = (roomId) => {
    if (user) {
      console.log('Navigating with Room ID:', roomId);
      navigate(`/booking/${roomId}`);
    } else {
      console.log('Redirecting to login...');
      navigate('/login');
    }
  };

  // ✅ Handle Add New Room (Admin Only)
  const handleAddRoom = (e) => {
    e.preventDefault();

    if (!newRoom.roomType || !newRoom.pricePerNight) {
      alert('Please fill in all fields');
      return;
    }

    const roomToAdd = {
      ...newRoom,
      pricePerNight: Number(newRoom.pricePerNight),
    };

    onAddRoom(roomToAdd);

    // ✅ Reset Form After Submission
    setNewRoom({
      roomType: '',
      pricePerNight: '',
      isAvailable: true,
    });
  };

  return (
    <div>
      {/* ✅ Add New Room Form (Admin Only) */}
      {role === 'Admin' && (
        <div className="card mb-4 shadow">
          <div className="card-body">
            <h5 className="card-title">Add New Room</h5>
            <form onSubmit={handleAddRoom}>
              <div className="mb-3">
                <input
                  type="text"
                  className="form-control"
                  placeholder="Room Type"
                  value={newRoom.roomType}
                  onChange={(e) =>
                    setNewRoom({ ...newRoom, roomType: e.target.value })
                  }
                  required
                />
              </div>
              <div className="mb-3">
                <input
                  type="number"
                  className="form-control"
                  placeholder="Price Per Night"
                  value={newRoom.pricePerNight}
                  onChange={(e) =>
                    setNewRoom({ ...newRoom, pricePerNight: e.target.value })
                  }
                  required
                />
              </div>
              <div className="form-check mb-3">
                <input
                  type="checkbox"
                  className="form-check-input"
                  id="isAvailable"
                  checked={newRoom.isAvailable}
                  onChange={(e) =>
                    setNewRoom({ ...newRoom, isAvailable: e.target.checked })
                  }
                />
                <label className="form-check-label" htmlFor="isAvailable">
                  Available
                </label>
              </div>
              <button type="submit" className="btn btn-success">
                Add Room
              </button>
            </form>
          </div>
        </div>
      )}

      {/* ✅ Existing Room List */}
      {rooms.length > 0 ? (
        <div className="row">
          {rooms.map((room) => (
            <div key={room.id} className="col-md-4 mb-3">
              <div className="card shadow-sm">
                <div className="card-body">
                  <h5 className="card-title">{room.roomType}</h5>
                  <p>💰 {room.pricePerNight} per night</p>
                  <p>
                    {room.isAvailable ? '✅ Available' : '❌ Not Available'}
                  </p>

                  {/* ✅ Book Now (User Only) */}
                  {role === 'User' && (
                    <button
                      className="btn btn-primary mt-2"
                      onClick={() => handleBookNow(room.id)}
                    >
                      Book Now
                    </button>
                  )}

                  {/* ✅ Update/Delete (Admin Only) */}
                  {role === 'Admin' && (
                    <div className="mt-2">
                      <button
                        className="btn btn-warning me-2"
                        onClick={() => onUpdateRoom(room.id)}
                      >
                        Update
                      </button>
                      <button
                        className="btn btn-danger"
                        onClick={() => onDeleteRoom(room.id)}
                      >
                        Delete
                      </button>
                    </div>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      ) : (
        <p>No rooms available.</p>
      )}
    </div>
  );
};

export default RoomList;
