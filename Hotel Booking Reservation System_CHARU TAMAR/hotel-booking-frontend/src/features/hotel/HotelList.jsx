import { useEffect, useState } from 'react';
import { getHotels, addHotel, updateHotel, deleteHotel } from '../../services/HotelService';
import { Link } from 'react-router-dom';

const HotelList = ({ role }) => {
    const [hotels, setHotels] = useState([]);
    const [loading, setLoading] = useState(false);
    const [newHotel, setNewHotel] = useState({
        name: '',
        location: '',
        description: ''
    });

    useEffect(() => {
        loadHotels();
    }, []);

    // ✅ Load Hotels
    const loadHotels = async () => {
        setLoading(true); // Start loading
        try {
            const data = await getHotels();
            setHotels(data);
        } catch (error) {
            console.error('Error loading hotels:', error);
        } finally {
            setLoading(false); // Stop loading
        }
    };

    // ✅ Add Hotel (Admin Only)
    const handleAddHotel = async (e) => {
        e.preventDefault();
        if (!newHotel.name || !newHotel.location || !newHotel.description) {
            alert('Please fill all required fields');
            return;
        }

        try {
            const addedHotel = await addHotel(newHotel);
            setHotels((prev) => [...prev, addedHotel]);
            setNewHotel({ name: '', location: '', description: '' });
        } catch (error) {
            console.error('Error adding hotel:', error);
            alert('Failed to add hotel. Please try again.');
        }
    };

    // ✅ Update Hotel (Admin Only)
    const handleUpdateHotel = async (hotelId) => {
        const updatedName = prompt('Enter new hotel name:');
        if (!updatedName) return;

        try {
            const updatedHotel = await updateHotel(hotelId, { name: updatedName });
            setHotels((prev) =>
                prev.map((hotel) => (hotel.id === hotelId ? updatedHotel : hotel))
            );
        } catch (error) {
            console.error('Error updating hotel:', error);
        }
    };

    // ✅ Delete Hotel (Admin Only)
    const handleDeleteHotel = async (hotelId) => {
        if (window.confirm('Are you sure you want to delete this hotel?')) {
            try {
                await deleteHotel(hotelId);
                setHotels((prev) => prev.filter((hotel) => hotel.id !== hotelId));
            } catch (error) {
                console.error('Error deleting hotel:', error);
            }
        }
    };

    return (
        <div className="container mt-4">
            {/* Welcome Section */}
            <div className="mb-4 p-4 bg-light rounded shadow-sm">
                <h2>Welcome, {role} 👋</h2>
                <p className="text-muted">Let's get started with the Hotel Booking & Reservation.</p>
            </div>
            <h2 className="mb-3">Available Hotels</h2>

            {/* ✅ Add Hotel Form (Admin Only) */}
            {role === 'Admin' && (
                <form onSubmit={handleAddHotel} className="mb-4">
                    <div className="row g-2">
                        <div className="col-md-3">
                            <input
                                type="text"
                                value={newHotel.name}
                                onChange={(e) =>
                                    setNewHotel({ ...newHotel, name: e.target.value })
                                }
                                placeholder="Hotel Name"
                                required
                                className="form-control"
                            />
                        </div>
                        <div className="col-md-3">
                            <input
                                type="text"
                                value={newHotel.location}
                                onChange={(e) =>
                                    setNewHotel({ ...newHotel, location: e.target.value })
                                }
                                placeholder="Location"
                                className="form-control"
                            />
                        </div>
                        <div className="col-md-3">
                            <input
                                type="text"
                                value={newHotel.description}
                                onChange={(e) =>
                                    setNewHotel({ ...newHotel, description: e.target.value })
                                }
                                placeholder="Description"
                                className="form-control"
                            />
                        </div>
                        <div className="col-md-2">
                            <button type="submit" className="btn btn-success w-100">
                                Add Hotel
                            </button>
                        </div>
                    </div>
                </form>
            )}

            {/* ✅ Loading State */}
            {loading && (
                <div className="alert alert-info" role="alert">
                    Loading hotels...
                </div>
            )}

            {/* ✅ List of Hotels */}
            {!loading && (
                <div className="row">
                    {hotels.length > 0 ? (
                        hotels.map((hotel) => (
                            <div key={hotel.id} className="col-md-4 mb-4">
                                <div className="card shadow-sm">
                                    <img
                                        src={
                                            hotel.name === 'Taj Mahal Palace'
                                                ? '/images/hotel1.jpg'
                                                : hotel.name === 'ITC'
                                                    ? '/images/hotel2.jpg'
                                                    : hotel.name === 'The Leela'
                                                        ? '/images/hotel3.jpg'
                                                        : hotel.name === 'Crowne Plaza'
                                                            ? '/images/hotel4.jpg'
                                                            : hotel.name === 'Radisson Blu'
                                                                ? '/images/hotel5.jpg'
                                                                : hotel.name === 'Country Inn & Suites'
                                                                    ? '/images/hotel6.jpg'
                                                                    : '/images/hotel7.jpg' // default image if none match
                                        }
                                        className="card-img-top"
                                        alt={hotel.name}
                                        style={{ height: '200px', objectFit: 'cover' }}
                                    />
                                    <div className="card-body">
                                        <h5 className="card-title">{hotel.name}</h5>
                                        <p className="card-text">{hotel.description}</p>

                                        <div className="d-flex gap-2">
                                            {/* ✅ View Details for Everyone */}
                                            <Link to={`/hotel/${hotel.id}`} className="btn btn-primary">
                                                View Details
                                            </Link>

                                            {/* ✅ Update/Delete for Admin */}
                                            {role === 'Admin' && (
                                                <>
                                                    <button
                                                        onClick={() => handleUpdateHotel(hotel.id)}
                                                        className="btn btn-warning"
                                                    >
                                                        Update
                                                    </button>
                                                    <button
                                                        onClick={() => handleDeleteHotel(hotel.id)}
                                                        className="btn btn-danger"
                                                    >
                                                        Delete
                                                    </button>
                                                </>
                                            )}
                                        </div>
                                    </div>
                                </div>
                            </div>
                        ))
                    ) : (
                        <p>No hotels available at the moment.</p>
                    )}
                </div>
            )}
        </div>
    );
};

export default HotelList;
