<template>
  <div class="bg-white rounded-lg shadow-md hover:shadow-xl transition-shadow duration-300 overflow-hidden">
    <!-- AI Summary Banner -->
    <div v-if="match.aiSummary" class="bg-gradient-to-r from-purple-500 to-primary-500 text-white p-4">
      <div class="flex items-start space-x-2">
        <svg class="w-5 h-5 flex-shrink-0 mt-0.5" fill="currentColor" viewBox="0 0 20 20">
          <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"></path>
        </svg>
        <p class="text-sm font-medium leading-snug">{{ match.aiSummary }}</p>
      </div>
    </div>

    <!-- Header with Score Badge -->
    <div class="bg-gradient-to-r from-primary-50 to-primary-100 p-4 relative">
      <div class="absolute top-4 right-4">
        <span class="bg-primary-600 text-white px-3 py-1 rounded-full text-sm font-semibold">
          {{ (match.similarity * 100).toFixed(0) }}% совпадение
        </span>
      </div>
      
      <h3 class="text-xl font-bold text-gray-800 mb-1">{{ match.profile.name }}</h3>
      
      <div v-if="match.profile.city || match.profile.country" class="text-sm text-gray-600 flex items-center">
        <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"></path>
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path>
        </svg>
        {{ [match.profile.city, match.profile.country].filter(Boolean).join(', ') }}
      </div>
    </div>

    <!-- Content -->
    <div class="p-4 space-y-4">
      <!-- Main Activity -->
      <div v-if="match.profile.mainActivity">
        <p class="text-xs font-semibold text-gray-500 uppercase mb-1">Основная деятельность</p>
        <p class="text-gray-700 text-sm">{{ match.profile.mainActivity }}</p>
      </div>

      <!-- Bio -->
      <div v-if="match.profile.bio">
        <p class="text-xs font-semibold text-gray-500 uppercase mb-1">О себе</p>
        <p class="text-gray-700 text-sm">{{ match.profile.bio }}</p>
      </div>

      <!-- Interests -->
      <div v-if="match.profile.interests">
        <p class="text-xs font-semibold text-gray-500 uppercase mb-1">Интересы</p>
        <p class="text-gray-700 text-sm">{{ match.profile.interests }}</p>
      </div>

      <!-- Startup Badge -->
      <div v-if="match.profile.hasStartup" class="flex items-center space-x-2 bg-purple-50 border border-purple-200 rounded-lg p-3">
        <svg class="w-5 h-5 text-purple-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"></path>
        </svg>
        <div>
          <p class="text-sm font-semibold text-purple-900">Основатель стартапа</p>
          <p v-if="match.profile.startupDescription" class="text-xs text-purple-700">{{ match.profile.startupDescription }}</p>
        </div>
      </div>

      <!-- Starter Message -->
      <div v-if="match.starterMessage" class="bg-gradient-to-r from-green-50 to-emerald-50 border border-green-200 rounded-lg p-4">
        <div class="flex items-start space-x-2">
          <svg class="w-5 h-5 text-green-600 flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"></path>
          </svg>
          <div class="flex-1">
            <p class="text-xs font-semibold text-green-800 uppercase mb-1">Начните диалог</p>
            <p class="text-sm text-gray-700 italic leading-relaxed">{{ match.starterMessage }}</p>
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

