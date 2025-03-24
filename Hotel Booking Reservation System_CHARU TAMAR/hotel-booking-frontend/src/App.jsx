import AppRoutes from './routes/AppRoutes';
import Navbar from './components/Navbar';
import Footer from './components/Footer';

const App = () => {
  return (
    <div className="container-fluid">
      <Navbar />
      <div style={{ minHeight: '100vh', paddingBottom: '60px' }}>
        <AppRoutes />
        <Footer />
      </div>

      {/* <Footer /> */}
    </div>
  );
};

export default App;
