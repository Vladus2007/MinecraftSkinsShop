import { useState, useEffect } from 'react';
import api from './api';
import './App.css';

function App() {
  const [skins, setSkins] = useState([]);
  const [myPurchases, setMyPurchases] = useState([]);
  const [statusMsg, setStatusMsg] = useState('');
  const [loading, setLoading] = useState(false);
  const [activeTab, setActiveTab] = useState('shop');

  const loadSkins = async () => {
    try {
      setLoading(true);
      const res = await api.get('/skins');
      setSkins(res.data);
      setStatusMsg('✅ Список скинов обновлен');
    } catch (err) {
      console.error(err);
      setStatusMsg('❌ Ошибка загрузки скинов');
    } finally {
      setLoading(false);
    }
  };

  const loadHistory = async () => {
    try {
      setLoading(true);
      const res = await api.get('/purchases/history');
      setMyPurchases(res.data);
      setStatusMsg('📊 История покупок загружена');
    } catch (err) {
      console.error(err);
      setStatusMsg('❌ Ошибка загрузки истории');
    } finally {
      setLoading(false);
    }
  };

  const buySkin = async (skinId) => {
    try {
      setLoading(true);
      await api.post('/purchases', { skinId });
      setStatusMsg(`🎉 Скин #${skinId} успешно куплен!`);
      loadHistory();
    } catch (err) {
      console.error(err);
      setStatusMsg('❌ Ошибка при покупке');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadSkins();
    loadHistory();
  }, []);

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleString('ru-RU', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  return (
    <div className="app">
      {/* Шапка */}
      <header className="header">
        <div className="header-content">
          <h1 className="logo">🎮 SkinShop</h1>
        </div>
      </header>

      {/* Статус сообщение */}
      {statusMsg && (
        <div className={`status-message ${statusMsg.includes('❌') ? 'error' : 'success'}`}>
          {statusMsg}
        </div>
      )}

      {/* Табы */}
      <div className="tabs">
        <button 
          className={`tab ${activeTab === 'shop' ? 'active' : ''}`}
          onClick={() => setActiveTab('shop')}
        >
          🛒 Магазин
        </button>
        <button 
          className={`tab ${activeTab === 'history' ? 'active' : ''}`}
          onClick={() => setActiveTab('history')}
        >
          📜 История покупок
        </button>
      </div>

      {/* Контент */}
      <main className="main-content">
        {/* Магазин */}
        {activeTab === 'shop' && (
          <div className="shop">
            <div className="shop-header">
              <h2>Доступные скины</h2>
              <button onClick={loadSkins} className="refresh-btn" disabled={loading}>
                {loading ? '⏳ Загрузка...' : '🔄 Обновить'}
              </button>
            </div>

            <div className="skins-grid">
              {skins.map(skin => (
                <div key={skin.id} className="skin-card">
                  <div className="skin-info">
                    <h3>{skin.name}</h3>
                    <div className="skin-price">
                      <span className="price-label">Цена:</span>
                      <span className="price-value">${skin.finalPriceUsd}</span>
                    </div>
                  </div>
                  <button 
                    onClick={() => buySkin(skin.id)}
                    className="buy-btn"
                    disabled={loading}
                  >
                    {loading ? '...' : 'Купить'}
                  </button>
                </div>
              ))}
              {skins.length === 0 && !loading && (
                <div className="empty-state">
                  <p>🛍️ Скины временно отсутствуют</p>
                </div>
              )}
            </div>
          </div>
        )}

        {/* История покупок */}
        {activeTab === 'history' && (
          <div className="history">
            <div className="history-header">
              <h2>Мои покупки</h2>
              <button onClick={loadHistory} className="refresh-btn" disabled={loading}>
                {loading ? '⏳ Загрузка...' : '🔄 Обновить'}
              </button>
            </div>

            <div className="purchases-list">
              {myPurchases.map((purchase, index) => (
                <div key={purchase.id || index} className="purchase-item">
                  <div className="purchase-icon">🎁</div>
                  <div className="purchase-details">
                    <div className="purchase-title">
                      Покупка #{purchase.id}
                      <span className="purchase-badge">Скин #{purchase.skinId}</span>
                    </div>
                    <div className="purchase-meta">
                      <span className="purchase-date">{formatDate(purchase.purchaseAt)}</span>
                      <span className="purchase-amount">${purchase.paidAmountUsd}</span>
                      <span className="purchase-btc">{purchase.btcPriceAtMoment} BTC</span>
                    </div>
                  </div>
                </div>
              ))}
              {myPurchases.length === 0 && !loading && (
                <div className="empty-state">
                  <p>📭 У вас пока нет покупок</p>
                  <button onClick={() => setActiveTab('shop')} className="shop-link">
                    Перейти в магазин
                  </button>
                </div>
              )}
            </div>
          </div>
        )}
      </main>
    </div>
  );
}

export default App;