<template>
  <div class="bg-white rounded-xl shadow-lg hover:shadow-2xl transition-all duration-300 overflow-hidden border border-gray-100">
    <!-- AI Summary Header - Most Important -->
    <div v-if="match.aiSummary" class="bg-gradient-to-br from-purple-600 via-purple-500 to-pink-500 text-white p-6">
      <div class="flex items-start space-x-3">
        <div class="flex-shrink-0 mt-1">
          <svg class="w-6 h-6 text-white" fill="currentColor" viewBox="0 0 20 20">
            <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"></path>
          </svg>
        </div>
        <div class="flex-1">
          <p class="text-xs font-bold uppercase tracking-wide mb-2 opacity-90">✨ AI Summary</p>
          <p class="text-base font-medium leading-relaxed">{{ match.aiSummary }}</p>
        </div>
        <div class="flex-shrink-0">
          <span class="bg-white bg-opacity-20 backdrop-blur-sm px-3 py-1.5 rounded-full text-sm font-bold">
            {{ (match.similarity * 100).toFixed(0) }}%
          </span>
        </div>
      </div>
    </div>

    <!-- Profile Header -->
    <div class="bg-gradient-to-r from-gray-50 to-gray-100 px-6 py-4 border-b border-gray-200">
      <h3 class="text-2xl font-bold text-gray-900 mb-1">{{ match.profile.name }}</h3>
      
      <!-- Location -->
      <div v-if="match.profile.city || match.profile.country" class="flex items-center text-gray-600">
        <svg class="w-4 h-4 mr-1.5 text-primary-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"></path>
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path>
        </svg>
        <span class="text-sm font-medium">{{ [match.profile.city, match.profile.country].filter(Boolean).join(', ') }}</span>
      </div>
    </div>

    <!-- Main Content -->
    <div class="p-6 space-y-5">
      <!-- Main Activity -->
      <div v-if="match.profile.mainActivity" class="flex items-start space-x-3">
        <div class="flex-shrink-0 mt-0.5">
          <div class="w-8 h-8 bg-blue-100 rounded-lg flex items-center justify-center">
            <svg class="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path>
            </svg>
          </div>
        </div>
        <div class="flex-1">
          <p class="text-xs font-bold text-gray-500 uppercase tracking-wide mb-1">Основная деятельность</p>
          <p class="text-gray-800 font-medium">{{ match.profile.mainActivity }}</p>
        </div>
      </div>

      <!-- Bio -->
      <div v-if="match.profile.bio" class="flex items-start space-x-3">
        <div class="flex-shrink-0 mt-0.5">
          <div class="w-8 h-8 bg-green-100 rounded-lg flex items-center justify-center">
            <svg class="w-5 h-5 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"></path>
            </svg>
          </div>
        </div>
        <div class="flex-1">
          <p class="text-xs font-bold text-gray-500 uppercase tracking-wide mb-1">О себе</p>
          <p class="text-gray-700 leading-relaxed">{{ match.profile.bio }}</p>
        </div>
      </div>

      <!-- Interests -->
      <div v-if="match.profile.interests" class="flex items-start space-x-3">
        <div class="flex-shrink-0 mt-0.5">
          <div class="w-8 h-8 bg-purple-100 rounded-lg flex items-center justify-center">
            <svg class="w-5 h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z"></path>
            </svg>
          </div>
        </div>
        <div class="flex-1">
          <p class="text-xs font-bold text-gray-500 uppercase tracking-wide mb-1">Интересы</p>
          <p class="text-gray-700 leading-relaxed">{{ match.profile.interests }}</p>
        </div>
      </div>

      <!-- Startup Badge -->
      <div v-if="match.profile.hasStartup" class="flex items-start space-x-3 bg-gradient-to-r from-orange-50 to-amber-50 border-2 border-orange-200 rounded-lg p-4">
        <div class="flex-shrink-0 mt-0.5">
          <div class="w-8 h-8 bg-orange-500 rounded-lg flex items-center justify-center">
            <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"></path>
            </svg>
          </div>
        </div>
        <div class="flex-1">
          <p class="text-sm font-bold text-orange-900 mb-1">🚀 Основатель стартапа</p>
          <p v-if="match.profile.startupDescription" class="text-sm text-orange-800 leading-relaxed">
            {{ match.profile.startupDescription }}
          </p>
        </div>
      </div>

      <!-- Divider -->
      <div class="border-t border-gray-200"></div>

      <!-- Starter Message CTA -->
      <div v-if="match.starterMessage" class="bg-gradient-to-br from-emerald-500 to-teal-500 rounded-xl p-5 text-white">
        <div class="flex items-start space-x-3">
          <div class="flex-shrink-0 mt-0.5">
            <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"></path>
            </svg>
          </div>
          <div class="flex-1">
            <p class="text-xs font-bold uppercase tracking-wide mb-2 opacity-90">💬 Начните диалог</p>
            <p class="text-base leading-relaxed font-medium mb-4">{{ match.starterMessage }}</p>
            <button class="w-full bg-white text-emerald-600 font-bold py-2.5 px-4 rounded-lg hover:bg-emerald-50 transition-colors duration-200 shadow-md hover:shadow-lg">
              Отправить сообщение →
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  match: {
    type: Object,
    required: true
  }
})
</script>
