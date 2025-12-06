<template>
  <div class="bg-white rounded-lg shadow-md hover:shadow-lg transition-all duration-300 overflow-hidden border border-gray-100">
    <!-- AI Summary Header - Light gradient like RAG -->
    <div v-if="match.aiSummary" class="bg-gradient-to-r from-purple-50 to-primary-50 border border-primary-200 p-4">
      <div class="flex items-start space-x-2">
        <div class="flex-shrink-0">
          <svg class="w-5 h-5 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"></path>
          </svg>
        </div>
        <div class="flex-1">
          <p class="text-xs font-semibold text-gray-900 mb-2">🤖 Резюме от AI</p>
          <p class="text-sm text-gray-700 leading-relaxed">{{ match.aiSummary }}</p>
        </div>
        <div class="flex-shrink-0">
          <span class="bg-primary-600 text-white px-2 py-1 rounded-full text-xs font-semibold">
            {{ (match.similarity * 100).toFixed(0) }}%
          </span>
        </div>
      </div>
    </div>

    <!-- Profile Header - Compact -->
    <div class="bg-gray-50 px-4 py-3 border-b border-gray-200">
      <div class="flex items-start justify-between">
        <h3 class="text-lg font-bold text-gray-900">{{ match.profile.name }}</h3>
        <!-- Location on the right -->
        <div v-if="match.profile.city || match.profile.country" class="flex items-center text-gray-500 text-xs ml-3">
          <svg class="w-3 h-3 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"></path>
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path>
          </svg>
          <span class="whitespace-nowrap">{{ [match.profile.city, match.profile.country].filter(Boolean).join(', ') }}</span>
        </div>
      </div>
      
      <!-- Email -->
      <div v-if="match.profile.email" class="flex items-center text-gray-500 text-xs mt-1">
        <svg class="w-3 h-3 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path>
        </svg>
        <span>{{ match.profile.email }}</span>
      </div>
    </div>

    <!-- Main Content - Compact -->
    <div class="p-4 space-y-3">
      <!-- Main Activity -->
      <div v-if="match.profile.mainActivity">
        <p class="text-xs font-semibold text-gray-500 uppercase mb-1">Основная деятельность</p>
        <p class="text-sm text-gray-800 font-medium">{{ match.profile.mainActivity }}</p>
      </div>

      <!-- Startup Badge - After main activity -->
      <div v-if="match.profile.hasStartup" class="bg-gradient-to-r from-orange-50 to-amber-50 border border-orange-200 rounded-lg p-2">
        <p class="text-xs font-semibold text-orange-900">🚀 Основатель стартапа</p>
        <p v-if="match.profile.startupDescription" class="text-xs text-orange-800 mt-1">
          {{ match.profile.startupDescription }}
        </p>
      </div>

      <!-- Bio -->
      <div v-if="match.profile.parsedShortBio">
        <p class="text-xs font-semibold text-gray-500 uppercase mb-1">О себе</p>
        <p class="text-sm text-gray-700 leading-relaxed">{{ match.profile.parsedShortBio }}</p>
      </div>

      <!-- Full Bio (if different from parsed) -->
      <div v-if="match.profile.bio && match.profile.bio !== match.profile.parsedShortBio" class="border-l-2 border-gray-200 pl-2">
        <p class="text-xs font-semibold text-gray-400 uppercase mb-1">Bio</p>
        <p class="text-xs text-gray-600 leading-relaxed">{{ match.profile.bio }}</p>
      </div>

      <!-- Interests -->
      <div v-if="match.profile.interests">
        <p class="text-xs font-semibold text-gray-500 uppercase mb-1">Интересы</p>
        <p class="text-sm text-gray-700 leading-relaxed">{{ match.profile.interests }}</p>
      </div>

      <!-- Divider -->
      <div class="border-t border-gray-200"></div>

      <!-- Starter Message CTA - Compact -->
      <div v-if="match.starterMessage" class="bg-blue-50 border border-blue-200 rounded-lg p-3">
        <p class="text-xs font-semibold text-blue-900 uppercase tracking-wide mb-2">💬 Начните диалог</p>
        <p class="text-sm text-gray-700 leading-relaxed mb-2">{{ match.starterMessage }}</p>
        
        <div class="flex items-center justify-between pt-2 border-t border-blue-200">
          <!-- Contact Links -->
          <div class="flex flex-wrap gap-1.5">
            <a
              v-if="match.profile.telegram"
              :href="`https://t.me/${match.profile.telegram.replace('@', '')}`"
              target="_blank"
              rel="noopener noreferrer"
              class="inline-flex items-center px-2 py-1 bg-white text-primary-700 text-xs font-medium rounded border border-primary-300 hover:bg-primary-50 transition-colors"
            >
              <svg class="w-3 h-3 mr-1" fill="currentColor" viewBox="0 0 24 24">
                <path d="M12 0C5.373 0 0 5.373 0 12s5.373 12 12 12 12-5.373 12-12S18.627 0 12 0zm5.562 8.161c-.18.717-.962 3.767-1.36 5.002-.169.523-.502.697-.825.715-.7.036-1.233-.463-1.912-.908-1.061-.696-1.66-1.129-2.693-1.81-1.193-.785-.42-1.217.261-1.923.179-.185 3.285-3.015 3.346-3.272.008-.032.014-.15-.056-.213-.07-.063-.173-.041-.247-.024-.107.025-1.793 1.139-5.062 3.345-.479.329-.913.489-1.302.481-.428-.009-1.252-.242-1.865-.442-.751-.245-1.349-.374-1.297-.789.027-.216.325-.437.893-.663 3.498-1.524 5.831-2.529 6.998-3.014 3.332-1.386 4.025-1.627 4.476-1.635.099-.002.321.023.465.14.121.099.155.232.171.326.016.095.037.311.021.479z"/>
              </svg>
              Telegram
            </a>
            
            <a
              v-if="match.profile.linkedin"
              :href="match.profile.linkedin"
              target="_blank"
              rel="noopener noreferrer"
              class="inline-flex items-center px-2 py-1 bg-white text-blue-700 text-xs font-medium rounded border border-blue-300 hover:bg-blue-50 transition-colors"
            >
              <svg class="w-3 h-3 mr-1" fill="currentColor" viewBox="0 0 24 24">
                <path d="M19 0h-14c-2.761 0-5 2.239-5 5v14c0 2.761 2.239 5 5 5h14c2.762 0 5-2.239 5-5v-14c0-2.761-2.238-5-5-5zm-11 19h-3v-11h3v11zm-1.5-12.268c-.966 0-1.75-.79-1.75-1.764s.784-1.764 1.75-1.764 1.75.79 1.75 1.764-.783 1.764-1.75 1.764zm13.5 12.268h-3v-5.604c0-3.368-4-3.113-4 0v5.604h-3v-11h3v1.765c1.396-2.586 7-2.777 7 2.476v6.759z"/>
              </svg>
              LinkedIn
            </a>
            
            <a
              v-if="match.profile.email"
              :href="`mailto:${match.profile.email}`"
              class="inline-flex items-center px-2 py-1 bg-white text-gray-700 text-xs font-medium rounded border border-gray-300 hover:bg-gray-50 transition-colors"
            >
              <svg class="w-3 h-3 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path>
              </svg>
              Email
            </a>
          </div>

          <!-- Copy Button - Icon Only -->
          <button 
            @click="copyToClipboard(match.starterMessage)"
            :title="copied ? 'Скопировано!' : 'Скопировать текст'"
            class="flex-shrink-0 p-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors"
          >
            <svg v-if="!copied" class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z"></path>
            </svg>
            <svg v-else class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
            </svg>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

defineProps({
  match: {
    type: Object,
    required: true
  }
})

const copied = ref(false)

const copyToClipboard = async (text) => {
  try {
    await navigator.clipboard.writeText(text)
    copied.value = true
    setTimeout(() => {
      copied.value = false
    }, 2000)
  } catch (err) {
    console.error('Failed to copy:', err)
  }
}
</script>
