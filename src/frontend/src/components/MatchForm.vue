<template>
  <div class="bg-white rounded-lg shadow-lg p-6">
    <h2 class="text-2xl font-bold text-gray-800 mb-6">Найти похожих пользователей</h2>
    
    <form @submit.prevent="handleMatch" class="space-y-4">
      <!-- Main Activity -->
      <div>
        <label for="mainActivity" class="block text-sm font-medium text-gray-700 mb-2">
          Основная деятельность <span class="text-red-500">*</span>
        </label>
        <input
          id="mainActivity"
          v-model="formData.mainActivity"
          type="text"
          class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          placeholder="например, Software Developer, Entrepreneur, Designer"
          required
        />
      </div>

      <!-- Interests -->
      <div>
        <label for="interests" class="block text-sm font-medium text-gray-700 mb-2">
          Интересы <span class="text-red-500">*</span>
        </label>
        <input
          id="interests"
          v-model="formData.interests"
          type="text"
          class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          placeholder="например, AI, Machine Learning, Hiking, Photography"
          required
        />
        <p class="mt-1 text-xs text-gray-500">
          Укажите интересы через запятую
        </p>
      </div>

      <!-- Location Row -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <!-- Country -->
        <div>
          <label for="country" class="block text-sm font-medium text-gray-700 mb-2">
            Страна
          </label>
          <input
            id="country"
            v-model="formData.country"
            type="text"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
            placeholder="например, Germany, USA, Russia"
          />
        </div>

        <!-- City -->
        <div>
          <label for="city" class="block text-sm font-medium text-gray-700 mb-2">
            Город
          </label>
          <input
            id="city"
            v-model="formData.city"
            type="text"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
            placeholder="например, Berlin, New York, Moscow"
          />
        </div>
      </div>

      <!-- TopK (Number of Results) -->
      <div>
        <label for="topK" class="block text-sm font-medium text-gray-700 mb-2">
          Количество результатов: {{ formData.topK }}
        </label>
        <input
          id="topK"
          v-model.number="formData.topK"
          type="range"
          min="1"
          max="10"
          step="1"
          class="w-full"
        />
        <div class="flex justify-between text-xs text-gray-500 mt-1">
          <span>1</span>
          <span>5</span>
          <span>10</span>
        </div>
      </div>

      <!-- Submit Button -->
      <div>
        <button
          type="submit"
          :disabled="loading || !formData.mainActivity || !formData.interests"
          class="w-full bg-primary-600 text-white py-3 px-6 rounded-lg font-semibold hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          <span v-if="loading">Поиск...</span>
          <span v-else>🤝 Match!</span>
        </button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const emit = defineEmits(['match'])

const formData = ref({
  mainActivity: '',
  interests: '',
  country: '',
  city: '',
  topK: 3
})

const loading = ref(false)

const handleMatch = async () => {
  loading.value = true
  try {
    const request = {
      mainActivity: formData.value.mainActivity,
      interests: formData.value.interests,
      topK: formData.value.topK
    }

    // Only add location if provided
    if (formData.value.country) {
      request.country = formData.value.country
    }
    if (formData.value.city) {
      request.city = formData.value.city
    }

    emit('match', request)
  } finally {
    loading.value = false
  }
}
</script>

