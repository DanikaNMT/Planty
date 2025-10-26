import React, { useEffect, useState } from 'react';
import { getUserProfile, updateUserProfile, updatePassword } from '../api/user.js';
import { ErrorMessage } from '../components/ErrorMessage.jsx';

export function SettingsPage({ navigate }) {
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

  useEffect(() => {
    loadProfile();
  }, []);

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
      <div className="card" style={{ maxWidth: '600px', margin: '0 auto' }}>
        <h2 style={{ fontSize: '2rem', fontWeight: '700', color: 'var(--color-primary)', marginBottom: 'var(--spacing-xl)', display: 'flex', alignItems: 'center', gap: 'var(--spacing-sm)' }}>
          <span>⚙️</span>
          Account Settings
        </h2>

        {/* Profile Section */}
        <div style={{ marginBottom: 'var(--spacing-xl)', paddingBottom: 'var(--spacing-xl)', borderBottom: '2px solid var(--color-border)' }}>
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

        {/* Password Section */}
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
      </div>
    </div>
  );
}
