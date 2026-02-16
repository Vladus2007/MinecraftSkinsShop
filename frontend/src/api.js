import axios from 'axios';


const api = axios.create({
    baseURL: 'http://localhost:5042/api', 
    headers: {
        'Content-Type': 'application/json'
    }
});

// ИНТЕРЦЕПТОР (ПЕРЕХВАТЧИК)
// Перед каждым запросом этот код выполняется автоматически
api.interceptors.request.use((config) => {
    // 1. Пытаемся найти ID пользователя в хранилище браузера
    let currentUserId = localStorage.getItem('mockUserId');

    // 2. Если ID нет (первый запуск), ставим "1" по умолчанию, чтобы всё работало сразу
    if (!currentUserId) {
        currentUserId = "1";
        localStorage.setItem('mockUserId', "1");
    }

    // 3. Добавляем заголовок, который ждет ваш MockAuthoriseHandler
    // Формат: "Bearer {int}"
    config.headers.Authorization = `Bearer ${currentUserId}`;

    console.log(`[API Request] Sending as User ID: ${currentUserId}`);
    
    return config;
});

export default api;