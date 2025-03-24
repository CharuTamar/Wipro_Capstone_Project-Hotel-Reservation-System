import { useParams } from 'react-router-dom';
import { useState } from 'react';
import { createCheckoutSession } from '../../services/PaymentService';

const Checkout = () => {
  const { amount } = useParams();
  const [loading, setLoading] = useState(false);

  const handlePayment = async () => {
    try {
      setLoading(true);

      const numericAmount = parseFloat(amount);

      if (isNaN(numericAmount) || numericAmount <= 0) {
        alert('Invalid payment amount');
        return;
      }

      const response = await createCheckoutSession(numericAmount, 'usd');

      if (response?.url) {
        console.log('✅ Redirecting to:', response.url);
        window.location.href = response.url; // ✅ Let Stripe handle the redirect naturally
      }
    } catch (error) {
      console.error('❌ Payment failed:', error);
      alert('Failed to initiate payment.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div 
      className="d-flex justify-content-center align-items-center" 
      style={{ height: '90vh', marginTop: '-5vh' }}
    >
      <div className="card shadow-lg p-4 rounded" style={{ maxWidth: '400px', width: '100%' }}>
        <div className="card-body text-center">
          <h2 className="card-title mb-4">Checkout</h2>
          <p className="card-text fs-5">
            <strong>Total Amount:</strong> ${amount}
          </p>
          <button 
            onClick={handlePayment} 
            className="btn btn-success w-100 mt-3"
            disabled={loading}
          >
            {loading ? 'Processing...' : 'Pay Now'}
          </button>
        </div>
      </div>
    </div>
  );
};

export default Checkout;
