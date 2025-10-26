import { apiFetch } from './client.js';

export function getUserProfile() {
  return apiFetch('/api/user/profile');
}

export function updateUserProfile(data) {
  return apiFetch('/api/user/profile', {
    method: 'PUT',
    body: JSON.stringify({
      userName: data.userName,
      email: data.email
    })
  });
}

export function updatePassword(data) {
  return apiFetch('/api/user/password', {
    method: 'PUT',
    body: JSON.stringify({
      currentPassword: data.currentPassword,
      newPassword: data.newPassword
    })
  });
}
