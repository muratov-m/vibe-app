<template>
  <div class="max-w-3xl mx-auto">
    <!-- Search Form Card -->
    <div class="bg-white rounded-lg shadow-md p-6 mb-8">
      <h2 class="text-2xl font-bold text-gray-900 mb-4">🔎 Найти профиль по Email</h2>
      <p class="text-sm text-gray-600 mb-6">
        Введите email адрес для поиска профиля пользователя
      </p>
      
      <form @submit.prevent="handleSearch" class="space-y-4">
        <div>
          <label for="email" class="block text-sm font-medium text-gray-700 mb-2">
            Email адрес
          </label>
          <input
            id="email"
            v-model="email"
            type="email"
            required
            placeholder="example@email.com"
            class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent transition-all"
          />
        </div>
        
        <button
          type="submit"
          :disabled="loading || !email"
          class="w-full bg-primary-600 text-white py-3 px-6 rounded-lg font-semibold hover:bg-primary-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors"
        >
          <span v-if="!loading">Найти профиль</span>
          <span v-else class="flex items-center justify-center">
            <svg class="animate-spin h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            Поиск...
          </span>
        </button>
      </form>
    </div>

    <!-- Loading State -->
    <div v-if="loading && !profile" class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
    </div>

    <!-- Error State -->
    <div v-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
      <div class="flex">
        <svg class="h-5 w-5 text-red-400" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"></path>
        </svg>
        <div class="ml-3">
          <h3 class="text-sm font-medium text-red-800">Ошибка</h3>
          <p class="text-sm text-red-700 mt-1">{{ error }}</p>
        </div>
      </div>
    </div>

    <!-- Profile Display -->
    <div v-if="profile && !loading" class="bg-white rounded-xl shadow-lg overflow-hidden">
      <!-- Profile Header -->
      <div class="bg-gradient-to-r from-primary-500 to-purple-600 px-6 py-8">
        <h2 class="text-3xl font-bold text-white mb-2">{{ profile.name }}</h2>
        <div class="flex items-center text-primary-50 mb-2">
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path>
          </svg>
          <span class="font-medium">{{ profile.email }}</span>
        </div>
        <div v-if="profile.parsedCity || profile.parsedCountry" class="flex items-center text-primary-50">
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"></path>
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path>
          </svg>
          <span>{{ [profile.parsedCity, profile.parsedCountry].filter(Boolean).join(', ') }}</span>
        </div>
      </div>

      <!-- Profile Content -->
      <div class="p-6 space-y-6">
        <!-- Main Activity -->
        <div v-if="profile.parsedMainActivity">
          <h3 class="text-xs font-bold text-gray-500 uppercase tracking-wide mb-2">Основная деятельность</h3>
          <p class="text-gray-800 font-medium">{{ profile.parsedMainActivity }}</p>
        </div>

        <!-- Bio -->
        <div v-if="profile.parsedShortBio">
          <h3 class="text-xs font-bold text-gray-500 uppercase tracking-wide mb-2">О себе</h3>
          <p class="text-gray-700 leading-relaxed">{{ profile.parsedShortBio }}</p>
        </div>

        <!-- Interests -->
        <div v-if="profile.parsedInterests">
          <h3 class="text-xs font-bold text-gray-500 uppercase tracking-wide mb-2">Интересы</h3>
          <p class="text-gray-700 leading-relaxed">{{ profile.parsedInterests }}</p>
        </div>

        <!-- Startup Info -->
        <div v-if="profile.hasStartup" class="bg-gradient-to-r from-orange-50 to-amber-50 border-2 border-orange-200 rounded-lg p-4">
          <div class="flex items-start space-x-3">
            <div class="flex-shrink-0">
              <svg class="w-6 h-6 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"></path>
              </svg>
            </div>
            <div class="flex-1">
              <p class="text-sm font-bold text-orange-900 mb-1">🚀 Основатель стартапа</p>
              <p v-if="profile.startupName" class="text-sm text-orange-800 font-semibold">{{ profile.startupName }}</p>
              <p v-if="profile.startupStage" class="text-xs text-orange-700 mt-1">{{ profile.startupStage }}</p>
              <p v-if="profile.startupDescription" class="text-sm text-orange-800 mt-2">{{ profile.startupDescription }}</p>
            </div>
          </div>
        </div>

        <!-- Skills -->
        <div v-if="profile.skills && profile.skills.length > 0">
          <h3 class="text-xs font-bold text-gray-500 uppercase tracking-wide mb-3">Навыки</h3>
          <div class="flex flex-wrap gap-2">
            <span
              v-for="skill in profile.skills"
              :key="skill.id"
              class="px-3 py-1.5 bg-blue-100 text-blue-800 text-sm rounded-full font-medium"
            >
              {{ skill.skill }}
            </span>
          </div>
        </div>

        <!-- Looking For -->
        <div v-if="profile.lookingFor && profile.lookingFor.length > 0">
          <h3 class="text-xs font-bold text-gray-500 uppercase tracking-wide mb-3">Ищет</h3>
          <div class="flex flex-wrap gap-2">
            <span
              v-for="item in profile.lookingFor"
              :key="item.id"
              class="px-3 py-1.5 bg-green-100 text-green-800 text-sm rounded-full font-medium"
            >
              {{ item.lookingFor }}
            </span>
          </div>
        </div>

        <!-- Can Help -->
        <div v-if="profile.canHelp">
          <h3 class="text-xs font-bold text-gray-500 uppercase tracking-wide mb-2">Может помочь</h3>
          <p class="text-gray-700">{{ profile.canHelp }}</p>
        </div>

        <!-- Needs Help -->
        <div v-if="profile.needsHelp">
          <h3 class="text-xs font-bold text-gray-500 uppercase tracking-wide mb-2">Нужна помощь</h3>
          <p class="text-gray-700">{{ profile.needsHelp }}</p>
        </div>

        <!-- AI Usage -->
        <div v-if="profile.aiUsage">
          <h3 class="text-xs font-bold text-gray-500 uppercase tracking-wide mb-2">🤖 Использование AI</h3>
          <p class="text-gray-700">{{ profile.aiUsage }}</p>
        </div>

        <!-- Contact Links -->
        <div class="border-t border-gray-200 pt-6 mt-6">
          <h3 class="text-xs font-bold text-gray-500 uppercase tracking-wide mb-3">Контакты</h3>
          <div class="flex flex-wrap gap-3">
            <a
              v-if="profile.telegram"
              :href="`https://t.me/${profile.telegram.replace('@', '')}`"
              target="_blank"
              rel="noopener noreferrer"
              class="flex items-center space-x-2 px-4 py-2 bg-primary-100 text-primary-700 rounded-lg hover:bg-primary-200 transition-colors font-medium"
            >
              <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 24 24">
                <path d="M12 0C5.373 0 0 5.373 0 12s5.373 12 12 12 12-5.373 12-12S18.627 0 12 0zm5.562 8.161c-.18.717-.962 3.767-1.36 5.002-.169.523-.502.697-.825.715-.7.036-1.233-.463-1.912-.908-1.061-.696-1.66-1.129-2.693-1.81-1.193-.785-.42-1.217.261-1.923.179-.185 3.285-3.015 3.346-3.272.008-.032.014-.15-.056-.213-.07-.063-.173-.041-.247-.024-.107.025-1.793 1.139-5.062 3.345-.479.329-.913.489-1.302.481-.428-.009-1.252-.242-1.865-.442-.751-.245-1.349-.374-1.297-.789.027-.216.325-.437.893-.663 3.498-1.524 5.831-2.529 6.998-3.014 3.332-1.386 4.025-1.627 4.476-1.635.099-.002.321.023.465.14.121.099.155.232.171.326.016.095.037.311.021.479z"/>
              </svg>
              <span>{{ profile.telegram }}</span>
            </a>
            
            <a
              v-if="profile.linkedin"
              :href="profile.linkedin"
              target="_blank"
              rel="noopener noreferrer"
              class="flex items-center space-x-2 px-4 py-2 bg-blue-100 text-blue-700 rounded-lg hover:bg-blue-200 transition-colors font-medium"
            >
              <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 24 24">
                <path d="M19 0h-14c-2.761 0-5 2.239-5 5v14c0 2.761 2.239 5 5 5h14c2.762 0 5-2.239 5-5v-14c0-2.761-2.238-5-5-5zm-11 19h-3v-11h3v11zm-1.5-12.268c-.966 0-1.75-.79-1.75-1.764s.784-1.764 1.75-1.764 1.75.79 1.75 1.764-.783 1.764-1.75 1.764zm13.5 12.268h-3v-5.604c0-3.368-4-3.113-4 0v5.604h-3v-11h3v1.765c1.396-2.586 7-2.777 7 2.476v6.759z"/>
              </svg>
              <span>LinkedIn</span>
            </a>
            
            <a
              v-if="profile.email"
              :href="`mailto:${profile.email}`"
              class="flex items-center space-x-2 px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors font-medium"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path>
              </svg>
              <span>Email</span>
            </a>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const emit = defineEmits(['search'])

const email = ref('')
const loading = ref(false)
const error = ref(null)
const profile = ref(null)

const handleSearch = () => {
  loading.value = true
  error.value = null
  
  emit('search', { email: email.value, loading, error, profile })
}
</script>

