<template>
  <div class="min-h-screen bg-gradient-to-br from-primary-50 via-white to-purple-50">
    <!-- Header -->
    <header class="bg-white shadow-sm">
      <div class="max-w-7xl mx-auto px-4 py-6 sm:px-6 lg:px-8">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-3xl font-bold text-gray-900">Vibe App</h1>
            <p class="text-sm text-gray-500 mt-1">Умный поиск людей с RAG</p>
          </div>
          <div class="text-right">
            <p class="text-sm text-gray-600">
              На базе <span class="font-semibold">AI Embeddings</span>
            </p>
          </div>
        </div>
      </div>
    </header>

    <!-- Tabs Navigation -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 mt-6">
      <div class="border-b border-gray-200">
        <nav class="-mb-px flex space-x-8">
          <button
            @click="activeTab = 'rag'"
            :class="[
              'py-4 px-1 border-b-2 font-medium text-sm transition-colors',
              activeTab === 'rag'
                ? 'border-primary-500 text-primary-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            ]"
          >
            🔍 RAG Search
          </button>
          <button
            @click="activeTab = 'match'"
            :class="[
              'py-4 px-1 border-b-2 font-medium text-sm transition-colors',
              activeTab === 'match'
                ? 'border-primary-500 text-primary-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            ]"
          >
            🤝 User Match
          </button>
        </nav>
      </div>
    </div>

    <!-- Main Content -->
    <main class="max-w-7xl mx-auto px-4 py-8 sm:px-6 lg:px-8">
      <!-- RAG Search Tab -->
      <div v-if="activeTab === 'rag'">
        <!-- Search Form -->
        <div class="mb-8">
          <SearchForm @search="handleSearch" />
        </div>

        <!-- Loading State -->
        <div v-if="loading" class="flex justify-center items-center py-12">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
        </div>

        <!-- Error State -->
        <div v-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
          <div class="flex">
            <svg class="h-5 w-5 text-red-400" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"></path>
            </svg>
            <div class="ml-3">
              <h3 class="text-sm font-medium text-red-800">Ошибка поиска</h3>
              <p class="text-sm text-red-700 mt-1">{{ error }}</p>
            </div>
          </div>
        </div>

        <!-- Results -->
        <div v-if="!loading && searchResults">
          <!-- AI Answer (if available) -->
          <div v-if="searchResults.answer" class="bg-gradient-to-r from-purple-50 to-primary-50 border border-primary-200 rounded-lg p-6 mb-8">
            <div class="flex items-start">
              <div class="flex-shrink-0">
                <svg class="h-6 w-6 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"></path>
                </svg>
              </div>
              <div class="ml-4 flex-1">
                <h3 class="text-sm font-semibold text-gray-900 mb-2">Резюме от AI</h3>
                <div class="text-gray-700 leading-relaxed space-y-3">
                  <p v-for="(paragraph, index) in formatAISummary(searchResults.answer)" :key="index">
                    {{ paragraph }}
                  </p>
                </div>
              </div>
            </div>
          </div>

          <!-- Results Header -->
          <div class="flex items-center justify-between mb-6">
            <h2 class="text-2xl font-bold text-gray-900">
              Найдено: {{ searchResults.totalResults }} 
              {{ searchResults.totalResults === 1 ? 'результат' : searchResults.totalResults < 5 ? 'результата' : 'результатов' }}
            </h2>
            <div v-if="searchResults.query" class="text-sm text-gray-500">
              Запрос: "<span class="italic">{{ searchResults.query }}</span>"
            </div>
          </div>

          <!-- No Results -->
          <div v-if="searchResults.totalResults === 0" class="text-center py-12">
            <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
            <h3 class="mt-2 text-sm font-medium text-gray-900">Профили не найдены</h3>
            <p class="mt-1 text-sm text-gray-500">Попробуйте изменить критерии поиска или фильтры</p>
          </div>

          <!-- Profile Cards Grid -->
          <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <ProfileCard
              v-for="profile in searchResults.results"
              :key="profile.id"
              :profile="profile"
            />
          </div>
        </div>

        <!-- Empty State (No Search Yet) -->
        <div v-if="!loading && !searchResults && !error" class="text-center py-12">
          <svg class="mx-auto h-16 w-16 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
          </svg>
          <h3 class="mt-4 text-lg font-medium text-gray-900">Начните поиск</h3>
          <p class="mt-2 text-sm text-gray-500">
            Используйте естественный язык для поиска людей по навыкам, интересам и многому другому
          </p>
          <div class="mt-6 space-y-2 text-xs text-gray-500 max-w-md mx-auto text-left bg-gray-50 rounded-lg p-4">
            <p class="font-semibold text-gray-700">Примеры запросов:</p>
            <ul class="list-disc list-inside space-y-1">
              <li>"Кто здесь знает Rust и любит походы?"</li>
              <li>"Найди мне со-основателя с опытом в маркетинге"</li>
              <li>"Ищу экспертов в AI/ML из Германии"</li>
              <li>"Кто может помочь с привлечением инвестиций?"</li>
            </ul>
          </div>
        </div>
      </div>

      <!-- User Match Tab -->
      <div v-if="activeTab === 'match'">
        <!-- Match Form -->
        <div class="mb-8">
          <MatchForm @match="handleMatch" />
        </div>

        <!-- Loading State -->
        <div v-if="matchLoading" class="flex justify-center items-center py-12">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
        </div>

        <!-- Error State -->
        <div v-if="matchError" class="bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
          <div class="flex">
            <svg class="h-5 w-5 text-red-400" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"></path>
            </svg>
            <div class="ml-3">
              <h3 class="text-sm font-medium text-red-800">Ошибка поиска</h3>
              <p class="text-sm text-red-700 mt-1">{{ matchError }}</p>
            </div>
          </div>
        </div>

        <!-- Match Results -->
        <div v-if="!matchLoading && matchResults && matchResults.length > 0">
          <!-- Results Header -->
          <div class="flex items-center justify-between mb-6">
            <h2 class="text-2xl font-bold text-gray-900">
              Найдено: {{ matchResults.length }} 
              {{ matchResults.length === 1 ? 'совпадение' : matchResults.length < 5 ? 'совпадения' : 'совпадений' }}
            </h2>
          </div>

          <!-- Profile Cards Grid -->
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <MatchProfileCard
              v-for="match in matchResults"
              :key="match.profile.id"
              :match="match"
            />
          </div>
        </div>

        <!-- No Results -->
        <div v-if="!matchLoading && matchResults && matchResults.length === 0" class="text-center py-12">
          <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
          </svg>
          <h3 class="mt-2 text-sm font-medium text-gray-900">Совпадения не найдены</h3>
          <p class="mt-1 text-sm text-gray-500">Попробуйте изменить критерии поиска</p>
        </div>

        <!-- Empty State (No Match Yet) -->
        <div v-if="!matchLoading && !matchResults && !matchError" class="text-center py-12">
          <svg class="mx-auto h-16 w-16 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
          </svg>
          <h3 class="mt-4 text-lg font-medium text-gray-900">Найдите похожих пользователей</h3>
          <p class="mt-2 text-sm text-gray-500">
            Укажите ваши интересы и деятельность, чтобы найти людей с похожими интересами
          </p>
        </div>
      </div>
    </main>

    <!-- Footer -->
    <footer class="mt-16 bg-white border-t border-gray-200">
      <div class="max-w-7xl mx-auto px-4 py-6 sm:px-6 lg:px-8">
        <p class="text-center text-sm text-gray-500">
          Разработано на Vue 3 + Vite + Tailwind CSS • Бэкенд: ASP.NET Core 9 + PostgreSQL + pgvector
        </p>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import SearchForm from './components/SearchForm.vue'
import MatchForm from './components/MatchForm.vue'
import ProfileCard from './components/ProfileCard.vue'
import MatchProfileCard from './components/MatchProfileCard.vue'
import { ragSearchService, userMatchService } from './services/api'

// Active tab state
const activeTab = ref('rag')

// RAG Search state
const loading = ref(false)
const error = ref(null)
const searchResults = ref(null)

// User Match state
const matchLoading = ref(false)
const matchError = ref(null)
const matchResults = ref(null)

const handleSearch = async (request) => {
  loading.value = true
  error.value = null
  searchResults.value = null

  try {
    const results = await ragSearchService.search(request)
    searchResults.value = results
  } catch (err) {
    error.value = err.message || 'Не удалось выполнить поиск'
    console.error('Search error:', err)
  } finally {
    loading.value = false
  }
}

const handleMatch = async (request) => {
  matchLoading.value = true
  matchError.value = null
  matchResults.value = null

  try {
    const results = await userMatchService.match(request)
    matchResults.value = results
  } catch (err) {
    matchError.value = err.message || 'Не удалось найти совпадения'
    console.error('Match error:', err)
  } finally {
    matchLoading.value = false
  }
}

const formatAISummary = (text) => {
  if (!text) return []
  // Split by double newlines or by sentences if no double newlines
  let paragraphs = text.split('\n\n').filter(p => p.trim())
  
  // If no double newlines, try to split long text intelligently
  if (paragraphs.length === 1 && text.length > 200) {
    // Split by sentence endings followed by space and capital letter
    const sentences = text.match(/[^.!?]+[.!?]+/g) || [text]
    paragraphs = []
    let currentParagraph = ''
    
    sentences.forEach((sentence, i) => {
      currentParagraph += sentence
      // Create new paragraph every 2-3 sentences or if current is long enough
      if ((i + 1) % 2 === 0 || currentParagraph.length > 250) {
        paragraphs.push(currentParagraph.trim())
        currentParagraph = ''
      }
    })
    
    if (currentParagraph.trim()) {
      paragraphs.push(currentParagraph.trim())
    }
  }
  
  return paragraphs
}
</script>

