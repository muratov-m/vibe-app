<template>
  <div class="bg-white rounded-lg shadow-lg p-6">
    <h2 class="text-2xl font-bold text-gray-800 mb-6">Поиск людей</h2>
    
    <form @submit.prevent="handleSearch" class="space-y-4">
      <!-- Query Input -->
      <div>
        <label for="query" class="block text-sm font-medium text-gray-700 mb-2">
          Что вы ищете?
        </label>
        <textarea
          id="query"
          v-model="formData.query"
          rows="3"
          class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          placeholder="например, Кто здесь знает Rust и любит походы?"
          required
        ></textarea>
        <p class="mt-1 text-xs text-gray-500">
          Попробуйте: "Найди мне кого-то с опытом в AI/ML" или "Ищу со-основателя с навыками маркетинга"
        </p>
      </div>

      <!-- Filters Row -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <!-- Country Filter -->
        <div>
          <label for="country" class="block text-sm font-medium text-gray-700 mb-2">
            Страна
          </label>
          <select
            id="country"
            v-model="formData.filters.country"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          >
            <option value="">Все страны</option>
            <option v-for="country in countries" :key="country.name" :value="country.name">
              {{ country.name }} ({{ country.userCount }})
            </option>
          </select>
        </div>

        <!-- HasStartup Filter -->
        <div>
          <label for="hasStartup" class="block text-sm font-medium text-gray-700 mb-2">
            Есть стартап
          </label>
          <select
            id="hasStartup"
            v-model="formData.filters.hasStartup"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          >
            <option :value="null">Неважно</option>
            <option :value="true">Да</option>
            <option :value="false">Нет</option>
          </select>
        </div>

        <!-- TopK (Results Count) -->
        <div>
          <label for="topK" class="block text-sm font-medium text-gray-700 mb-2">
            Результатов
          </label>
          <select
            id="topK"
            v-model="formData.topK"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          >
            <option :value="3">3</option>
            <option :value="6">6</option>
            <option :value="15">15</option>
          </select>
        </div>
      </div>

      <!-- Generate Response Toggle -->
      <div class="flex items-center bg-purple-50 border border-purple-200 rounded-lg p-4">
        <input
          id="generateResponse"
          v-model="formData.generateResponse"
          type="checkbox"
          class="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
        />
        <label for="generateResponse" class="ml-3 block text-sm font-medium text-gray-700">
          🤖 Создать резюме от AI (структурированный список людей)
        </label>
      </div>

      <!-- Advanced Settings (Collapsible) - Only filters now -->
      <div v-if="false">
        <button
          type="button"
          @click="showAdvanced = !showAdvanced"
          class="text-sm text-primary-600 hover:text-primary-700 font-medium"
        >
          {{ showAdvanced ? '▼' : '▶' }} Расширенные настройки
        </button>
        
        <div v-if="showAdvanced" class="mt-3 space-y-4">
          <!-- No advanced settings for now -->
        </div>
      </div>

      <!-- Submit Button -->
      <div>
        <button
          type="submit"
          :disabled="loading || !formData.query"
          class="w-full bg-primary-600 text-white py-3 px-6 rounded-lg font-semibold hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          <span v-if="loading">Поиск...</span>
          <span v-else>🔍 Искать</span>
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
  topK: 6,
  generateResponse: true,
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

