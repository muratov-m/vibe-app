const API_BASE_URL = import.meta.env.VIBE_API_BASE_URL || '';

export const ragSearchService = {
  /**
   * Search for user profiles using RAG
   * @param {Object} request - Search request
   * @param {string} request.query - Natural language query
   * @param {number} request.topK - Number of results (default: 5)
   * @param {boolean} request.generateResponse - Generate LLM response (default: true)
   * @param {number} request.minSimilarity - Min similarity threshold (default: 0.3)
   * @param {Object} request.filters - Optional filters
   * @param {string} request.filters.country - Filter by country
   * @param {boolean} request.filters.hasStartup - Filter by has startup
   * @returns {Promise<Object>} Search response
   */
  async search(request) {
    const response = await fetch(`${API_BASE_URL}/api/ragsearch/search`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || 'Search failed');
    }

    return await response.json();
  },

  /**
   * Get list of all countries
   * @returns {Promise<Array>} List of countries with user counts
   */
  async getCountries() {
    const response = await fetch(`${API_BASE_URL}/api/country`);
    
    if (!response.ok) {
      throw new Error('Failed to fetch countries');
    }

    return await response.json();
  }
};

export const userMatchService = {
  /**
   * Find matching users based on user criteria
   * @param {Object} request - Match request
   * @param {string} request.mainActivity - Main activity/occupation
   * @param {string} request.interests - Comma-separated interests
   * @param {string} request.country - Country (optional)
   * @param {string} request.city - City (optional)
   * @param {number} request.topK - Number of results (default: 3)
   * @returns {Promise<Array>} List of matching users with similarity scores
   */
  async match(request) {
    const response = await fetch(`${API_BASE_URL}/api/usermatch/match`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || 'Match failed');
    }

    return await response.json();
  },

  /**
   * Get list of all countries
   * @returns {Promise<Array>} List of countries with user counts
   */
  async getCountries() {
    const response = await fetch(`${API_BASE_URL}/api/country`);
    
    if (!response.ok) {
      throw new Error('Failed to fetch countries');
    }

    return await response.json();
  }
};

