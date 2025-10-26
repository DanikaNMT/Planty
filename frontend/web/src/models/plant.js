/**
 * @typedef {Object} Plant
 * @property {string} id
 * @property {string} name
 * @property {string | null} species
 * @property {string | null} description
 * @property {string} dateAdded
 * @property {string | null} lastWatered
 * @property {number | null} wateringIntervalDays
 * @property {string | null} location
 * @property {string | null} imageUrl
 * @property {string | null} nextWateringDue
 * @property {string | null} lastFertilized
 * @property {number | null} fertilizationIntervalDays
 * @property {string | null} nextFertilizationDue
 * @property {string | null} latestPictureUrl
 */

/**
 * @typedef {Object} Watering
 * @property {string} id
 * @property {string} wateredAt
 * @property {string | null} notes
 */

/**
 * @typedef {Object} Fertilization
 * @property {string} id
 * @property {string} fertilizedAt
 * @property {string | null} notes
 */

/**
 * @typedef {Object} CareEvent
 * @property {string} id
 * @property {string} type - "Watering", "Fertilization", or "Picture"
 * @property {string} timestamp
 * @property {string | null} notes
 * @property {string | null} imageUrl - Only for Picture type
 */

/**
 * @typedef {Object} CreatePlantRequest
 * @property {string} name
 * @property {string | null} species
 * @property {string | null} description
 * @property {number | null} wateringIntervalDays
 * @property {number | null} fertilizationIntervalDays
 * @property {string | null} location
 */
