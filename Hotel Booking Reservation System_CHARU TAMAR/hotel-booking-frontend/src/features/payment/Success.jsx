import { useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import axios from 'axios';

const Success = () => {
  const [searchParams] = useSearchParams();
  const sessionId = searchParams.get('session_id');

  useEffect(() => {
    const verifyPayment = async () => {
      try {
        const response = await axios.get(`http://localhost:5099/api/payment/verify-session?sessionId=${sessionId}`);
        console.log(response.data); // ✅ Output the verification response
      } catch (error) {
        console.error('Failed to verify payment:', error);
      }
    };

    if (sessionId) verifyPayment(); // ✅ Trigger verification if session ID is present
  }, [sessionId]); // ✅ Dependency array to watch for sessionId changes

  return (
    <div style={{ textAlign: 'center', marginTop: '50px' }}>
      <h1>✅ Payment Successful!</h1>
      <p>Session ID: {sessionId}</p>
      <a href="/">Go to Homepage</a>
    </div>
  );
};

export default Success;
