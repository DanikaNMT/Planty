import { apiFetch } from './client.js';

/**
 * Share Types
 */
export const ShareType = {
  Plant: 0,
  Location: 1
};

/**
 * Share Roles (ordered by permission level)
 */
export const ShareRole = {
  Viewer: 0,    // Can only view
  Carer: 1,     // Can view + water/fertilize/picture
  Editor: 2,    // Can view + care + edit
  Owner: 3      // Full control including sharing and deleting
};

/**
 * Get human-readable role name
 */
export function getRoleName(role) {
  const names = {
    [ShareRole.Viewer]: 'Viewer',
    [ShareRole.Carer]: 'Carer',
    [ShareRole.Editor]: 'Editor',
    [ShareRole.Owner]: 'Owner'
  };
  return names[role] || 'Unknown';
}

/**
 * Get role description
 */
export function getRoleDescription(role) {
  const descriptions = {
    [ShareRole.Viewer]: 'Can only view',
    [ShareRole.Carer]: 'Can view and perform care actions',
    [ShareRole.Editor]: 'Can view, care, and edit',
    [ShareRole.Owner]: 'Full control including sharing and deleting'
  };
  return descriptions[role] || '';
}

/**
 * Create a new share (share a plant or location with another user)
 * @param {Object} data - Share data
 * @param {number} data.shareType - ShareType.Plant or ShareType.Location
 * @param {string} data.plantId - Plant ID (if sharing a plant)
 * @param {string} data.locationId - Location ID (if sharing a location)
 * @param {string} data.sharedWithUserEmail - Email of user to share with
 * @param {number} data.role - ShareRole value
 */
export function createShare(data) {
  return apiFetch('/api/shares', {
    method: 'POST',
    body: JSON.stringify({
      shareType: data.shareType,
      plantId: data.plantId || null,
      locationId: data.locationId || null,
      sharedWithUserEmail: data.sharedWithUserEmail,
      role: data.role
    })
  });
}

/**
 * Get all shares created by the current user (things I've shared with others)
 */
export function getSharesCreated() {
  return apiFetch('/api/shares/created');
}

/**
 * Get all shares received by the current user (things others have shared with me)
 */
export function getSharesReceived() {
  return apiFetch('/api/shares/received');
}

/**
 * Update a share (change the role)
 * @param {string} shareId - Share ID
 * @param {number} role - New ShareRole value
 */
export function updateShare(shareId, role) {
  return apiFetch(`/api/shares/${shareId}`, {
    method: 'PUT',
    body: JSON.stringify({ role })
  });
}

/**
 * Delete a share (revoke access)
 * @param {string} shareId - Share ID
 */
export function deleteShare(shareId) {
  return apiFetch(`/api/shares/${shareId}`, {
    method: 'DELETE'
  });
}

/**
 * Search for users by email or username
 * @param {string} searchTerm - Search query
 */
export function searchUsers(searchTerm) {
  if (!searchTerm || searchTerm.trim().length === 0) {
    return Promise.resolve([]);
  }
  return apiFetch(`/api/user/search?q=${encodeURIComponent(searchTerm)}`);
}
