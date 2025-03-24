import axios from 'axios';

const API_URL = 'http://localhost:5099/api/payment';

export const createCheckoutSession = async (amount) => {
    try {
        const response = await axios.post(`${API_URL}/create-checkout-session`, {
            amount: amount,
            currency: 'usd'
        });
        window.location.href = response.data.url;
    } catch (error) {
        console.error('❌ Failed to create checkout session:', error);
        throw error;
    }
};
