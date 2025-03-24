import HotelList from "../features/hotel/HotelList";
import Footer from "../components/Footer";
import HeroSection from "../components/HeroSection";
const Home = () => {

  return (
    <div >
      {/* Hero Section */}
      <HeroSection />

      {/* Hotel List */}
       <HotelList />
     </div>
  );
};

export default Home;
