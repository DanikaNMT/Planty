import React, { useEffect, useState } from 'react';
import { getUserProfile, updateUserProfile, updatePassword } from '../api/user.js';
import { getSharesCreated, getSharesReceived, deleteShare, updateShare, createShare, searchUsers, ShareRole, getRoleName, getRoleDescription, ShareType } from '../api/shares.js';
import { getPlants } from '../api/plants.js';
import { getLocations } from '../api/locations.js';
import { ErrorMessage } from '../components/ErrorMessage.jsx';

export function SettingsPage({ navigate }) {
  const [activeTab, setActiveTab] = useState('profile'); // 'profile', 'password', 'shares-created', 'shares-received'
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);
  const [profileData, setProfileData] = useState({ userName: '', email: '' });
  const [passwordData, setPasswordData] = useState({ currentPassword: '', newPassword: '', confirmPassword: '' });
  const [savingProfile, setSavingProfile] = useState(false);
  const [savingPassword, setSavingPassword] = useState(false);
  const [profileError, setProfileError] = useState(null);
  const [passwordError, setPasswordError] = useState(null);
  const [profileSuccess, setProfileSuccess] = useState(false);
  const [passwordSuccess, setPasswordSuccess] = useState(false);

  // Sharing state
  const [sharesCreated, setSharesCreated] = useState([]);
  const [sharesReceived, setSharesReceived] = useState([]);
  const [loadingShares, setLoadingShares] = useState(false);
  const [sharesError, setSharesError] = useState(null);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [plants, setPlants] = useState([]);
  const [locations, setLocations] = useState([]);

  useEffect(() => {
    loadProfile();
  }, []);

  useEffect(() => {
    if (activeTab === 'shares-created') {
      loadSharesCreated();
    } else if (activeTab === 'shares-received') {
      loadSharesReceived();
    }
  }, [activeTab]);

  function loadProfile() {
    setLoading(true);
    setProfileError(null);
    getUserProfile()
      .then(data => {
        setProfile(data);
        setProfileData({ userName: data.userName, email: data.email });
      })
      .catch(e => setProfileError(e.message))
      .finally(() => setLoading(false));
  }

  function handleProfileSubmit(e) {
    e.preventDefault();
    setSavingProfile(true);
    setProfileError(null);
    setProfileSuccess(false);
    
    updateUserProfile(profileData)
      .then(updated => {
        setProfile(updated);
        setProfileData({ userName: updated.userName, email: updated.email });
        setProfileSuccess(true);
        setTimeout(() => setProfileSuccess(false), 3000);
      })
      .catch(e => setProfileError(e.message))
      .finally(() => setSavingProfile(false));
  }

  function handlePasswordSubmit(e) {
    e.preventDefault();
    
    if (passwordData.newPassword !== passwordData.confirmPassword) {
      setPasswordError('New password and confirmation do not match.');
      return;
    }

    if (passwordData.newPassword.length < 6) {
      setPasswordError('New password must be at least 6 characters.');
      return;
    }

    setSavingPassword(true);
    setPasswordError(null);
    setPasswordSuccess(false);
    
    updatePassword({
      currentPassword: passwordData.currentPassword,
      newPassword: passwordData.newPassword
    })
      .then(() => {
        setPasswordData({ currentPassword: '', newPassword: '', confirmPassword: '' });
        setPasswordSuccess(true);
        setTimeout(() => setPasswordSuccess(false), 3000);
      })
      .catch(e => setPasswordError(e.message))
      .finally(() => setSavingPassword(false));
  }

  function loadSharesCreated() {
    setLoadingShares(true);
    setSharesError(null);
    getSharesCreated()
      .then(data => setSharesCreated(data || []))
      .catch(e => setSharesError(e.message))
      .finally(() => setLoadingShares(false));
  }

  function loadSharesReceived() {
    setLoadingShares(true);
    setSharesError(null);
    getSharesReceived()
      .then(data => setSharesReceived(data || []))
      .catch(e => setSharesError(e.message))
      .finally(() => setLoadingShares(false));
  }

  function handleDeleteShare(shareId) {
    if (!confirm('Are you sure you want to revoke this share?')) return;
    
    deleteShare(shareId)
      .then(() => {
        if (activeTab === 'shares-created') {
          loadSharesCreated();
        } else {
          loadSharesReceived();
        }
      })
      .catch(e => alert('Error deleting share: ' + e.message));
  }

  function handleUpdateShareRole(shareId, newRole) {
    updateShare(shareId, newRole)
      .then(() => loadSharesCreated())
      .catch(e => alert('Error updating share: ' + e.message));
  }

  function openCreateShareModal() {
    setShowCreateModal(true);
    // Load plants and locations for the modal
    Promise.all([getPlants(), getLocations()])
      .then(([plantsData, locationsData]) => {
        setPlants(plantsData || []);
        setLocations(locationsData || []);
      })
      .catch(e => console.error('Error loading data:', e));
  }

  if (loading) {
    return (
      <div className="loading">
        <div className="spinner"></div>
        Loading settings...
      </div>
    );
  }

  return (
    <div>
      <div className="card" style={{ maxWidth: '800px', margin: '0 auto' }}>
        <h2 style={{ fontSize: '2rem', fontWeight: '700', color: 'var(--color-primary)', marginBottom: 'var(--spacing-lg)', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
          <span>⚙️</span>
          Settings
        </h2>

        {/* Tab Navigation */}
        <div style={{ 
          display: 'flex', 
          gap: 'var(--spacing-sm)', 
          marginBottom: 'var(--spacing-xl)',
          borderBottom: '2px solid var(--color-border)',
          overflowX: 'auto'
        }}>
          <button 
            onClick={() => setActiveTab('profile')}
            style={{
              padding: 'var(--spacing-md)',
              border: 'none',
              background: 'none',
              borderBottom: activeTab === 'profile' ? '3px solid var(--color-primary)' : '3px solid transparent',
              color: activeTab === 'profile' ? 'var(--color-primary)' : 'var(--color-text-light)',
              fontWeight: activeTab === 'profile' ? '600' : '400',
              cursor: 'pointer',
              whiteSpace: 'nowrap'
            }}
          >
            👤 Profile
          </button>
          <button 
            onClick={() => setActiveTab('password')}
            style={{
              padding: 'var(--spacing-md)',
              border: 'none',
              background: 'none',
              borderBottom: activeTab === 'password' ? '3px solid var(--color-primary)' : '3px solid transparent',
              color: activeTab === 'password' ? 'var(--color-primary)' : 'var(--color-text-light)',
              fontWeight: activeTab === 'password' ? '600' : '400',
              cursor: 'pointer',
              whiteSpace: 'nowrap'
            }}
          >
            🔒 Password
          </button>
          <button 
            onClick={() => setActiveTab('shares-created')}
            style={{
              padding: 'var(--spacing-md)',
              border: 'none',
              background: 'none',
              borderBottom: activeTab === 'shares-created' ? '3px solid var(--color-primary)' : '3px solid transparent',
              color: activeTab === 'shares-created' ? 'var(--color-primary)' : 'var(--color-text-light)',
              fontWeight: activeTab === 'shares-created' ? '600' : '400',
              cursor: 'pointer',
              whiteSpace: 'nowrap'
            }}
          >
            📤 Shares Created
          </button>
          <button 
            onClick={() => setActiveTab('shares-received')}
            style={{
              padding: 'var(--spacing-md)',
              border: 'none',
              background: 'none',
              borderBottom: activeTab === 'shares-received' ? '3px solid var(--color-primary)' : '3px solid transparent',
              color: activeTab === 'shares-received' ? 'var(--color-primary)' : 'var(--color-text-light)',
              fontWeight: activeTab === 'shares-received' ? '600' : '400',
              cursor: 'pointer',
              whiteSpace: 'nowrap'
            }}
          >
            📥 Shares Received
          </button>
        </div>

        {/* Profile Tab */}
        {activeTab === 'profile' && (
          <div>
            <h3 style={{ fontSize: '1.5rem', fontWeight: '600', marginBottom: 'var(--spacing-md)', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
              <span>👤</span>
              Profile Information
            </h3>

            <form onSubmit={handleProfileSubmit}>
              <div className="form-group">
                <label>👤 Username *</label>
                <input 
                  type="text"
                  value={profileData.userName} 
                  onChange={e => setProfileData({ ...profileData, userName: e.target.value })} 
                  required 
                  maxLength={100}
                />
              </div>
              
              <div className="form-group">
                <label>✉️ Email *</label>
                <input 
                  type="email"
                  value={profileData.email} 
                  onChange={e => setProfileData({ ...profileData, email: e.target.value })} 
                  required 
                  maxLength={255}
                />
              </div>
              
              <button type="submit" disabled={savingProfile} className="btn-primary" style={{ width: '100%' }}>
                {savingProfile ? '⏳ Saving...' : '💾 Update Profile'}
              </button>
            </form>
            
            {profileSuccess && (
              <div style={{ 
                marginTop: 'var(--spacing-md)', 
                padding: 'var(--spacing-md)', 
                backgroundColor: '#d4edda', 
                color: '#155724',
                borderRadius: '8px',
                border: '1px solid #c3e6cb'
              }}>
                ✓ Profile updated successfully!
              </div>
            )}
            <ErrorMessage error={profileError} />
          </div>
        )}

        {/* Password Tab */}
        {activeTab === 'password' && (
          <div>
            <h3 style={{ fontSize: '1.5rem', fontWeight: '600', marginBottom: 'var(--spacing-md)', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
              <span>🔒</span>
              Change Password
            </h3>

            <form onSubmit={handlePasswordSubmit}>
              <div className="form-group">
                <label>🔑 Current Password *</label>
                <input 
                  type="password"
                  value={passwordData.currentPassword} 
                  onChange={e => setPasswordData({ ...passwordData, currentPassword: e.target.value })} 
                  required 
                  autoComplete="current-password"
                />
              </div>
              
              <div className="form-group">
                <label>🔒 New Password *</label>
                <input 
                  type="password"
                  value={passwordData.newPassword} 
                  onChange={e => setPasswordData({ ...passwordData, newPassword: e.target.value })} 
                  required 
                  minLength={6}
                  autoComplete="new-password"
                />
                <small style={{ color: 'var(--color-text-light)', fontSize: '0.85rem' }}>
                  Minimum 6 characters
                </small>
              </div>
              
              <div className="form-group">
                <label>🔒 Confirm New Password *</label>
                <input 
                  type="password"
                  value={passwordData.confirmPassword} 
                  onChange={e => setPasswordData({ ...passwordData, confirmPassword: e.target.value })} 
                  required 
                  minLength={6}
                  autoComplete="new-password"
                />
              </div>
              
              <button type="submit" disabled={savingPassword} className="btn-primary" style={{ width: '100%' }}>
                {savingPassword ? '⏳ Changing...' : '🔑 Change Password'}
              </button>
            </form>
            
            {passwordSuccess && (
              <div style={{ 
                marginTop: 'var(--spacing-md)', 
                padding: 'var(--spacing-md)', 
                backgroundColor: '#d4edda', 
                color: '#155724',
                borderRadius: '8px',
                border: '1px solid #c3e6cb'
              }}>
                ✓ Password changed successfully!
              </div>
            )}
            <ErrorMessage error={passwordError} />
          </div>
        )}

        {/* Shares Created Tab */}
        {activeTab === 'shares-created' && (
          <SharesCreatedTab 
            shares={sharesCreated}
            loading={loadingShares}
            error={sharesError}
            onDelete={handleDeleteShare}
            onUpdateRole={handleUpdateShareRole}
            onCreateNew={openCreateShareModal}
          />
        )}

        {/* Shares Received Tab */}
        {activeTab === 'shares-received' && (
          <SharesReceivedTab 
            shares={sharesReceived}
            loading={loadingShares}
            error={sharesError}
          />
        )}
      </div>

      {/* Create Share Modal */}
      {showCreateModal && (
        <CreateShareModal 
          plants={plants}
          locations={locations}
          onClose={() => setShowCreateModal(false)}
          onSuccess={() => {
            setShowCreateModal(false);
            loadSharesCreated();
          }}
        />
      )}
    </div>
  );
}

// Shares Created Tab Component
function SharesCreatedTab({ shares, loading, error, onDelete, onUpdateRole, onCreateNew }) {
  if (loading) {
    return (
      <div className="loading">
        <div className="spinner"></div>
        Loading shares...
      </div>
    );
  }

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 'var(--spacing-lg)' }}>
        <h3 style={{ fontSize: '1.5rem', fontWeight: '600', margin: 0 }}>
          Shares I've Created
        </h3>
        <button onClick={onCreateNew} className="btn-primary">
          ➕ Create Share
        </button>
      </div>

      <ErrorMessage error={error} />

      {shares.length === 0 ? (
        <div style={{ 
          padding: 'var(--spacing-xl)', 
          textAlign: 'center', 
          color: 'var(--color-text-light)',
          backgroundColor: 'var(--color-bg-light)',
          borderRadius: '8px'
        }}>
          <p style={{ fontSize: '3rem', margin: '0 0 var(--spacing-md) 0' }}>🤝</p>
          <p>You haven't shared any plants or locations yet.</p>
          <p style={{ fontSize: '0.9rem', marginTop: 'var(--spacing-sm)' }}>
            Click "Create Share" to share plants or locations with others!
          </p>
        </div>
      ) : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--spacing-md)' }}>
          {shares.map(share => {
            // Defensive checks for navigation properties
            if (!share.sharedWithUser) {
              console.error('Share missing sharedWithUser:', share);
              return null;
            }
            
            return (
            <div key={share.id} style={{
              padding: 'var(--spacing-md)',
              border: '1px solid var(--color-border)',
              borderRadius: '8px',
              backgroundColor: 'var(--color-bg-light)'
            }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: 'var(--spacing-sm)' }}>
                <div style={{ flex: 1 }}>
                  <div style={{ fontWeight: '600', fontSize: '1.1rem', marginBottom: 'var(--spacing-xs)' }}>
                    {share.shareType === 0 && '🌱 ' + share.plantName}
                    {share.shareType === 1 && '📍 ' + share.locationName}
                    {share.shareType === 2 && '🌿 Entire Collection'}
                  </div>
                  <div style={{ color: 'var(--color-text-light)', fontSize: '0.9rem' }}>
                    Shared with: <strong>{share.sharedWithUser.email}</strong>
                  </div>
                  <div style={{ color: 'var(--color-text-light)', fontSize: '0.85rem', marginTop: 'var(--spacing-xs)' }}>
                    Created: {new Date(share.createdAt).toLocaleDateString()}
                  </div>
                </div>
                <div style={{ display: 'flex', gap: 'var(--spacing-sm)', alignItems: 'center' }}>
                  <select 
                    value={share.role}
                    onChange={(e) => onUpdateRole(share.id, parseInt(e.target.value))}
                    style={{
                      padding: 'var(--spacing-sm)',
                      borderRadius: '4px',
                      border: '1px solid var(--color-border)',
                      fontSize: '0.9rem'
                    }}
                  >
                    <option value={ShareRole.Viewer}>Viewer</option>
                    <option value={ShareRole.Carer}>Carer</option>
                    <option value={ShareRole.Editor}>Editor</option>
                    <option value={ShareRole.Owner}>Owner</option>
                  </select>
                  <button 
                    onClick={() => onDelete(share.id)}
                    style={{
                      padding: 'var(--spacing-sm) var(--spacing-md)',
                      backgroundColor: '#dc3545',
                      color: 'white',
                      border: 'none',
                      borderRadius: '4px',
                      cursor: 'pointer',
                      fontSize: '0.9rem'
                    }}
                  >
                    🗑️ Revoke
                  </button>
                </div>
              </div>
              <div style={{ 
                fontSize: '0.85rem', 
                color: 'var(--color-text-light)',
                fontStyle: 'italic',
                marginTop: 'var(--spacing-sm)',
                paddingTop: 'var(--spacing-sm)',
                borderTop: '1px solid var(--color-border)'
              }}>
                {getRoleDescription(share.role)}
              </div>
            </div>
          )})}
        </div>
      )}
    </div>
  );
}

// Shares Received Tab Component
function SharesReceivedTab({ shares, loading, error }) {
  if (loading) {
    return (
      <div className="loading">
        <div className="spinner"></div>
        Loading shares...
      </div>
    );
  }

  return (
    <div>
      <h3 style={{ fontSize: '1.5rem', fontWeight: '600', marginBottom: 'var(--spacing-lg)' }}>
        Shares I've Received
      </h3>

      <ErrorMessage error={error} />

      {shares.length === 0 ? (
        <div style={{ 
          padding: 'var(--spacing-xl)', 
          textAlign: 'center', 
          color: 'var(--color-text-light)',
          backgroundColor: 'var(--color-bg-light)',
          borderRadius: '8px'
        }}>
          <p style={{ fontSize: '3rem', margin: '0 0 var(--spacing-md) 0' }}>📬</p>
          <p>No one has shared plants or locations with you yet.</p>
        </div>
      ) : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--spacing-md)' }}>
          {shares.map(share => {
            // Defensive checks for navigation properties
            if (!share.owner) {
              console.error('Share missing owner:', share);
              return null;
            }
            
            return (
            <div key={share.id} style={{
              padding: 'var(--spacing-md)',
              border: '1px solid var(--color-border)',
              borderRadius: '8px',
              backgroundColor: 'var(--color-bg-light)'
            }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start' }}>
                <div style={{ flex: 1 }}>
                  <div style={{ fontWeight: '600', fontSize: '1.1rem', marginBottom: 'var(--spacing-xs)' }}>
                    {share.shareType === 0 && '🌱 ' + share.plantName}
                    {share.shareType === 1 && '📍 ' + share.locationName}
                    {share.shareType === 2 && '🌿 Entire Collection'}
                  </div>
                  <div style={{ color: 'var(--color-text-light)', fontSize: '0.9rem' }}>
                    Shared by: <strong>{share.owner.email}</strong>
                  </div>
                  <div style={{ color: 'var(--color-text-light)', fontSize: '0.85rem', marginTop: 'var(--spacing-xs)' }}>
                    Shared on: {new Date(share.createdAt).toLocaleDateString()}
                  </div>
                </div>
                <div style={{
                  padding: 'var(--spacing-sm) var(--spacing-md)',
                  backgroundColor: 'var(--color-primary)',
                  color: 'white',
                  borderRadius: '4px',
                  fontSize: '0.9rem',
                  fontWeight: '600'
                }}>
                  {getRoleName(share.role)}
                </div>
              </div>
              <div style={{ 
                fontSize: '0.85rem', 
                color: 'var(--color-text-light)',
                fontStyle: 'italic',
                marginTop: 'var(--spacing-sm)',
                paddingTop: 'var(--spacing-sm)',
                borderTop: '1px solid var(--color-border)'
              }}>
                {getRoleDescription(share.role)}
              </div>
            </div>
          )})}
        </div>
      )}
    </div>
  );
}

// Create Share Modal Component
function CreateShareModal({ plants, locations, onClose, onSuccess }) {
  const [shareType, setShareType] = useState(ShareType.Plant);
  const [selectedItem, setSelectedItem] = useState('');
  const [userSearch, setUserSearch] = useState('');
  const [searchResults, setSearchResults] = useState([]);
  const [selectedUser, setSelectedUser] = useState(null);
  const [role, setRole] = useState(ShareRole.Viewer);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);
  const [searchTimeout, setSearchTimeout] = useState(null);

  useEffect(() => {
    // Clear search when changing share type
    setSelectedItem('');
  }, [shareType]);

  useEffect(() => {
    // Debounced user search
    if (searchTimeout) clearTimeout(searchTimeout);
    
    if (userSearch.trim().length < 2) {
      setSearchResults([]);
      return;
    }

    const timeout = setTimeout(() => {
      searchUsers(userSearch)
        .then(results => setSearchResults(results || []))
        .catch(e => console.error('Search error:', e));
    }, 300);

    setSearchTimeout(timeout);

    return () => {
      if (searchTimeout) clearTimeout(searchTimeout);
    };
  }, [userSearch]);

  function handleSubmit(e) {
    e.preventDefault();
    setError(null);

    if (shareType !== ShareType.Collection && !selectedItem) {
      setError('Please select a plant or location to share');
      return;
    }

    if (!selectedUser) {
      setError('Please select a user to share with');
      return;
    }

    setSubmitting(true);

    const shareData = {
      shareType,
      plantId: shareType === ShareType.Plant ? selectedItem : null,
      locationId: shareType === ShareType.Location ? selectedItem : null,
      sharedWithUserEmail: selectedUser.email,
      role
    };

    createShare(shareData)
      .then(() => onSuccess())
      .catch(e => setError(e.message))
      .finally(() => setSubmitting(false));
  }

  const items = shareType === ShareType.Plant ? plants : locations;

  return (
    <div style={{
      position: 'fixed',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: 'rgba(0,0,0,0.5)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      zIndex: 1000,
      padding: 'var(--spacing-md)'
    }} onClick={onClose}>
      <div style={{
        backgroundColor: 'white',
        borderRadius: '12px',
        maxWidth: '500px',
        width: '100%',
        maxHeight: '90vh',
        overflow: 'auto',
        padding: 'var(--spacing-xl)'
      }} onClick={e => e.stopPropagation()}>
        <h2 style={{ marginBottom: 'var(--spacing-lg)', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
          <span>🤝</span>
          Create Share
        </h2>

        <form onSubmit={handleSubmit}>
          {/* Share Type */}
          <div className="form-group">
            <label>What do you want to share?</label>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--spacing-sm)' }}>
              <label style={{ display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)', cursor: 'pointer' }}>
                <input 
                  type="radio"
                  value={ShareType.Plant}
                  checked={shareType === ShareType.Plant}
                  onChange={() => setShareType(ShareType.Plant)}
                />
                <span>🌱 Plant</span>
              </label>
              <label style={{ display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)', cursor: 'pointer' }}>
                <input 
                  type="radio"
                  value={ShareType.Location}
                  checked={shareType === ShareType.Location}
                  onChange={() => setShareType(ShareType.Location)}
                />
                <span>📍 Location</span>
              </label>
              <label style={{ display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)', cursor: 'pointer' }}>
                <input 
                  type="radio"
                  value={ShareType.Collection}
                  checked={shareType === ShareType.Collection}
                  onChange={() => setShareType(ShareType.Collection)}
                />
                <span>🌿 Entire Collection (all plants and locations)</span>
              </label>
            </div>
          </div>

          {/* Select Item - Only show for Plant and Location */}
          {shareType !== ShareType.Collection && (
            <div className="form-group">
              <label>{shareType === ShareType.Plant ? '🌱 Select Plant *' : '📍 Select Location *'}</label>
              <select 
                value={selectedItem}
                onChange={e => setSelectedItem(e.target.value)}
                required
              >
                <option value="">-- Select {shareType === ShareType.Plant ? 'a plant' : 'a location'} --</option>
                {items.map(item => (
                  <option key={item.id} value={item.id}>
                    {item.name}
                  </option>
                ))}
              </select>
            </div>
          )}

          {/* User Search */}
          <div className="form-group">
            <label>👤 Find User *</label>
            <input 
              type="text"
              placeholder="Search by email or username..."
              value={userSearch}
              onChange={e => setUserSearch(e.target.value)}
              autoComplete="off"
            />
            {searchResults.length > 0 && (
              <div style={{
                marginTop: 'var(--spacing-sm)',
                border: '1px solid var(--color-border)',
                borderRadius: '4px',
                maxHeight: '200px',
                overflowY: 'auto'
              }}>
                {searchResults.map(user => (
                  <div 
                    key={user.id}
                    onClick={() => {
                      setSelectedUser(user);
                      setUserSearch(user.email);
                      setSearchResults([]);
                    }}
                    style={{
                      padding: 'var(--spacing-sm)',
                      cursor: 'pointer',
                      borderBottom: '1px solid var(--color-border)',
                      backgroundColor: selectedUser?.id === user.id ? 'var(--color-bg-light)' : 'white'
                    }}
                  >
                    <div style={{ fontWeight: '600' }}>{user.userName}</div>
                    <div style={{ fontSize: '0.85rem', color: 'var(--color-text-light)' }}>{user.email}</div>
                  </div>
                ))}
              </div>
            )}
            {selectedUser && (
              <div style={{
                marginTop: 'var(--spacing-sm)',
                padding: 'var(--spacing-sm)',
                backgroundColor: 'var(--color-bg-light)',
                borderRadius: '4px',
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center'
              }}>
                <div>
                  <div style={{ fontWeight: '600' }}>{selectedUser.userName}</div>
                  <div style={{ fontSize: '0.85rem', color: 'var(--color-text-light)' }}>{selectedUser.email}</div>
                </div>
                <button 
                  type="button"
                  onClick={() => {
                    setSelectedUser(null);
                    setUserSearch('');
                  }}
                  style={{ 
                    background: 'none', 
                    border: 'none', 
                    cursor: 'pointer',
                    fontSize: '1.2rem'
                  }}
                >
                  ✕
                </button>
              </div>
            )}
          </div>

          {/* Role Selection */}
          <div className="form-group">
            <label>🔑 Permission Level *</label>
            <select 
              value={role}
              onChange={e => setRole(parseInt(e.target.value))}
              required
            >
              <option value={ShareRole.Viewer}>Viewer - Can only view</option>
              <option value={ShareRole.Carer}>Carer - Can view and perform care actions</option>
              <option value={ShareRole.Editor}>Editor - Can view, care, and edit</option>
              <option value={ShareRole.Owner}>Owner - Full control</option>
            </select>
            <small style={{ color: 'var(--color-text-light)', fontSize: '0.85rem', display: 'block', marginTop: 'var(--spacing-xs)' }}>
              {getRoleDescription(role)}
            </small>
          </div>

          <ErrorMessage error={error} />

          {/* Buttons */}
          <div style={{ display: 'flex', gap: 'var(--spacing-md)', marginTop: 'var(--spacing-lg)' }}>
            <button 
              type="button" 
              onClick={onClose}
              disabled={submitting}
              style={{
                flex: 1,
                padding: 'var(--spacing-md)',
                backgroundColor: 'var(--color-bg-light)',
                border: '1px solid var(--color-border)',
                borderRadius: '8px',
                cursor: 'pointer'
              }}
            >
              Cancel
            </button>
            <button 
              type="submit"
              disabled={submitting}
              className="btn-primary"
              style={{ flex: 1 }}
            >
              {submitting ? '⏳ Creating...' : '✓ Create Share'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

