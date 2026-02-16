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
      setStatusMsg('✅ Skin list updated');
    } catch (err) {
      console.error(err);
      setStatusMsg('❌ Error loading skins');
    } finally {
      setLoading(false);
    }
  };

  const loadHistory = async () => {
    try {
      setLoading(true);
      const res = await api.get('/purchases/history');
      setMyPurchases(res.data);
      setStatusMsg('📊 Purchase history loaded');
    } catch (err) {
      console.error(err);
      setStatusMsg('❌ Error loading history');
    } finally {
      setLoading(false);
    }
  };

  const buySkin = async (skinId) => {
    try {
      setLoading(true);
      await api.post('/purchases', { skinId });
      setStatusMsg(`🎉 Skin #${skinId} successfully purchased!`);
      loadHistory();
    } catch (err) {
      console.error(err);
      setStatusMsg('❌ Error making purchase');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadSkins();
    loadHistory();
  }, []);

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleString('en-US', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  return (
    <div className="app">
      {/* Header */}
      <header className="header">
        <div className="header-content">
          <h1 className="logo">🎮 SkinShop</h1>
        </div>
      </header>

      {/* Status Message */}
      {statusMsg && (
        <div className={`status-message ${statusMsg.includes('❌') ? 'error' : 'success'}`}>
          {statusMsg}
        </div>
      )}

      {/* Tabs */}
      <div className="tabs">
        <button 
          className={`tab ${activeTab === 'shop' ? 'active' : ''}`}
          onClick={() => setActiveTab('shop')}
        >
          🛒 Shop
        </button>
        <button 
          className={`tab ${activeTab === 'history' ? 'active' : ''}`}
          onClick={() => setActiveTab('history')}
        >
          📜 Purchase History
        </button>
      </div>

      {/* Content */}
      <main className="main-content">
        {/* Shop Tab */}
        {activeTab === 'shop' && (
          <div className="shop">
            <div className="shop-header">
              <h2>Available Skins</h2>
              <button onClick={loadSkins} className="refresh-btn" disabled={loading}>
                {loading ? '⏳ Loading...' : '🔄 Refresh'}
              </button>
            </div>

            <div className="skins-grid">
              {skins.map(skin => (
                <div key={skin.id} className="skin-card">
                  <div className="skin-info">
                    <h3>{skin.name}</h3>
                    <div className="skin-price">
                      <span className="price-label">Price:</span>
                      <span className="price-value">${skin.finalPriceUsd}</span>
                    </div>
                  </div>
                  <button 
                    onClick={() => buySkin(skin.id)}
                    className="buy-btn"
                    disabled={loading}
                  >
                    {loading ? '...' : 'Buy'}
                  </button>
                </div>
              ))}
              {skins.length === 0 && !loading && (
                <div className="empty-state">
                  <p>🛍️ No skins available at the moment</p>
                </div>
              )}
            </div>
          </div>
        )}

        {/* History Tab */}
        {activeTab === 'history' && (
          <div className="history">
            <div className="history-header">
              <h2>My Purchases</h2>
              <button onClick={loadHistory} className="refresh-btn" disabled={loading}>
                {loading ? '⏳ Loading...' : '🔄 Refresh'}
              </button>
            </div>

            <div className="purchases-list">
              {myPurchases.map((purchase, index) => (
                <div key={purchase.id || index} className="purchase-item">
                  <div className="purchase-icon">🎁</div>
                  <div className="purchase-details">
                    <div className="purchase-title">
                      Purchase #{purchase.id}
                      <span className="purchase-badge">Skin #{purchase.skinId}</span>
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
                  <p>📭 You haven't made any purchases yet</p>
                  <button onClick={() => setActiveTab('shop')} className="shop-link">
                    Go to Shop
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