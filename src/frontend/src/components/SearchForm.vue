<template>
  <div class="bg-white rounded-lg shadow-lg p-6">
    <h2 class="text-2xl font-bold text-gray-800 mb-6">Search for People</h2>
    
    <form @submit.prevent="handleSearch" class="space-y-4">
      <!-- Query Input -->
      <div>
        <label for="query" class="block text-sm font-medium text-gray-700 mb-2">
          What are you looking for?
        </label>
        <textarea
          id="query"
          v-model="formData.query"
          rows="3"
          class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          placeholder="e.g., Who here knows Rust and likes hiking?"
          required
        ></textarea>
        <p class="mt-1 text-xs text-gray-500">
          Try: "Find me someone with AI/ML experience" or "Looking for co-founder with marketing skills"
        </p>
      </div>

      <!-- Filters Row -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <!-- Country Filter -->
        <div>
          <label for="country" class="block text-sm font-medium text-gray-700 mb-2">
            Country
          </label>
          <select
            id="country"
            v-model="formData.filters.country"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          >
            <option value="">All countries</option>
            <option v-for="country in countries" :key="country.name" :value="country.name">
              {{ country.name }} ({{ country.userCount }})
            </option>
          </select>
        </div>

        <!-- HasStartup Filter -->
        <div>
          <label for="hasStartup" class="block text-sm font-medium text-gray-700 mb-2">
            Has Startup
          </label>
          <select
            id="hasStartup"
            v-model="formData.filters.hasStartup"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          >
            <option :value="null">Any</option>
            <option :value="true">Yes</option>
            <option :value="false">No</option>
          </select>
        </div>

        <!-- TopK (Results Count) -->
        <div>
          <label for="topK" class="block text-sm font-medium text-gray-700 mb-2">
            Results
          </label>
          <select
            id="topK"
            v-model="formData.topK"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          >
            <option :value="3">3</option>
            <option :value="5">5</option>
            <option :value="10">10</option>
            <option :value="20">20</option>
          </select>
        </div>
      </div>

      <!-- Advanced Settings (Collapsible) -->
      <div>
        <button
          type="button"
          @click="showAdvanced = !showAdvanced"
          class="text-sm text-primary-600 hover:text-primary-700 font-medium"
        >
          {{ showAdvanced ? '▼' : '▶' }} Advanced Settings
        </button>
        
        <div v-if="showAdvanced" class="mt-3 space-y-4">
          <!-- Min Similarity -->
          <div>
            <label for="minSimilarity" class="block text-sm font-medium text-gray-700 mb-2">
              Min Similarity: {{ formData.minSimilarity.toFixed(1) }}
            </label>
            <input
              id="minSimilarity"
              v-model.number="formData.minSimilarity"
              type="range"
              min="0"
              max="1"
              step="0.1"
              class="w-full"
            />
            <p class="mt-1 text-xs text-gray-500">
              Higher value = more strict matching
            </p>
          </div>

          <!-- Generate Response -->
          <div class="flex items-center">
            <input
              id="generateResponse"
              v-model="formData.generateResponse"
              type="checkbox"
              class="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
            />
            <label for="generateResponse" class="ml-2 block text-sm text-gray-700">
              Generate AI response summary
            </label>
          </div>
        </div>
      </div>

      <!-- Submit Button -->
      <div>
        <button
          type="submit"
          :disabled="loading || !formData.query"
          class="w-full bg-primary-600 text-white py-3 px-6 rounded-lg font-semibold hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          <span v-if="loading">Searching...</span>
          <span v-else>🔍 Search</span>
        </button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { ragSearchService } from '../services/api'

const emit = defineEmits(['search'])

const formData = ref({
  query: '',
  topK: 5,
  generateResponse: true,
  minSimilarity: 0.3,
  filters: {
    country: '',
    hasStartup: null
  }
})

const countries = ref([])
const loading = ref(false)
const showAdvanced = ref(false)

onMounted(async () => {
  try {
    countries.value = await ragSearchService.getCountries()
  } catch (error) {
    console.error('Failed to load countries:', error)
  }
})

const handleSearch = async () => {
  loading.value = true
  try {
    const request = {
      query: formData.value.query,
      topK: formData.value.topK,
      generateResponse: formData.value.generateResponse,
      minSimilarity: formData.value.minSimilarity,
      filters: {}
    }

    // Only add filters if they have values
    if (formData.value.filters.country) {
      request.filters.country = formData.value.filters.country
    }
    if (formData.value.filters.hasStartup !== null) {
      request.filters.hasStartup = formData.value.filters.hasStartup
    }

    // Remove filters if empty
    if (Object.keys(request.filters).length === 0) {
      delete request.filters
    }

    emit('search', request)
  } finally {
    loading.value = false
  }
}
</script>

