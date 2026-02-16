import axios from 'axios';

const api = axios.create({
    baseURL: 'http://localhost:5042/api', 
    headers: {
        'Content-Type': 'application/json'
    }
});

// Request interceptor
// This code runs automatically before each request
api.interceptors.request.use((config) => {
    // 1. Try to find user ID in browser storage
    let currentUserId = localStorage.getItem('mockUserId');

    // 2. If no ID (first run), set "1" by default so everything works immediately
    if (!currentUserId) {
        currentUserId = "1";
        localStorage.setItem('mockUserId', "1");
    }

    // 3. Add the header that your MockAuthoriseHandler expects
    
    config.headers.Authorization = `Bearer ${currentUserId}`;

    console.log(`[API Request] Sending as User ID: ${currentUserId}`);
    
    return config;
});

export default api;