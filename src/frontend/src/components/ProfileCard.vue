<template>
  <div class="bg-white rounded-lg shadow-md hover:shadow-xl transition-shadow duration-300 overflow-hidden">
    <!-- Header with Score Badge -->
    <div class="bg-gradient-to-r from-primary-50 to-primary-100 p-4 relative">
      <div class="absolute top-4 right-4">
        <span class="bg-primary-600 text-white px-3 py-1 rounded-full text-sm font-semibold">
          {{ (profile.similarityScore * 100).toFixed(0) }}% совпадение
        </span>
      </div>
      
      <h3 class="text-xl font-bold text-gray-800 mb-1">{{ profile.name }}</h3>
      
      <div v-if="profile.city || profile.country" class="text-sm text-gray-600 flex items-center">
        <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"></path>
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path>
        </svg>
        {{ [profile.city, profile.country].filter(Boolean).join(', ') }}
      </div>
    </div>

    <!-- Content -->
    <div class="p-4 space-y-4">
      <!-- Bio -->
      <div v-if="profile.bio">
        <p class="text-gray-700 text-sm">{{ profile.bio }}</p>
      </div>

      <!-- Startup Badge -->
      <div v-if="profile.startupName || profile.startupStage || profile.hasStartup" class="flex items-center space-x-2 bg-purple-50 border border-purple-200 rounded-lg p-2">
        <svg class="w-5 h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"></path>
        </svg>
        <div>
          <p class="text-sm font-semibold text-purple-900">{{ profile.startupName || 'Основатель стартапа' }}</p>
          <p v-if="profile.startupStage" class="text-xs text-purple-700">{{ profile.startupStage }}</p>
        </div>
      </div>

      <!-- Skills -->
      <div v-if="profile.skills && profile.skills.length > 0">
        <p class="text-xs font-semibold text-gray-500 uppercase mb-2">Навыки</p>
        <div class="flex flex-wrap gap-2">
          <span
            v-for="skill in parseSkills(profile.skills)"
            :key="skill"
            class="px-2 py-1 bg-blue-100 text-blue-800 text-xs rounded-full"
          >
            {{ skill }}
          </span>
        </div>
      </div>

      <!-- Looking For -->
      <div v-if="profile.lookingFor && profile.lookingFor.length > 0">
        <p class="text-xs font-semibold text-gray-500 uppercase mb-2">Ищет</p>
        <div class="flex flex-wrap gap-2">
          <span
            v-for="item in parseLookingFor(profile.lookingFor)"
            :key="item"
            class="px-2 py-1 bg-green-100 text-green-800 text-xs rounded-full"
          >
            {{ item }}
          </span>
        </div>
      </div>

      <!-- Can Help -->
      <div v-if="profile.canHelp" class="text-sm">
        <p class="text-xs font-semibold text-gray-500 uppercase mb-1">Может помочь</p>
        <p class="text-gray-700">{{ profile.canHelp }}</p>
      </div>

      <!-- Needs Help -->
      <div v-if="profile.needsHelp" class="text-sm">
        <p class="text-xs font-semibold text-gray-500 uppercase mb-1">Нужна помощь</p>
        <p class="text-gray-700">{{ profile.needsHelp }}</p>
      </div>

      <!-- Interests -->
      <div v-if="profile.interests" class="text-sm">
        <p class="text-xs font-semibold text-gray-500 uppercase mb-1">Интересы</p>
        <p class="text-gray-700">{{ profile.interests }}</p>
      </div>

      <!-- Contact Links -->
      <div class="flex flex-wrap gap-3 pt-4 border-t border-gray-200">
        <a
          v-if="profile.telegram"
          :href="`https://t.me/${profile.telegram.replace('@', '')}`"
          target="_blank"
          rel="noopener noreferrer"
          class="flex items-center space-x-1 text-sm text-primary-600 hover:text-primary-700 font-medium"
        >
          <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 24 24">
            <path d="M12 0C5.373 0 0 5.373 0 12s5.373 12 12 12 12-5.373 12-12S18.627 0 12 0zm5.562 8.161c-.18.717-.962 3.767-1.36 5.002-.169.523-.502.697-.825.715-.7.036-1.233-.463-1.912-.908-1.061-.696-1.66-1.129-2.693-1.81-1.193-.785-.42-1.217.261-1.923.179-.185 3.285-3.015 3.346-3.272.008-.032.014-.15-.056-.213-.07-.063-.173-.041-.247-.024-.107.025-1.793 1.139-5.062 3.345-.479.329-.913.489-1.302.481-.428-.009-1.252-.242-1.865-.442-.751-.245-1.349-.374-1.297-.789.027-.216.325-.437.893-.663 3.498-1.524 5.831-2.529 6.998-3.014 3.332-1.386 4.025-1.627 4.476-1.635.099-.002.321.023.465.14.121.099.155.232.171.326.016.095.037.311.021.479z"/>
          </svg>
          <span>{{ profile.telegram }}</span>
        </a>
        
        <a
          v-if="profile.linkedin"
          :href="profile.linkedin"
          target="_blank"
          rel="noopener noreferrer"
          class="flex items-center space-x-1 text-sm text-primary-600 hover:text-primary-700 font-medium"
        >
          <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 24 24">
            <path d="M19 0h-14c-2.761 0-5 2.239-5 5v14c0 2.761 2.239 5 5 5h14c2.762 0 5-2.239 5-5v-14c0-2.761-2.238-5-5-5zm-11 19h-3v-11h3v11zm-1.5-12.268c-.966 0-1.75-.79-1.75-1.764s.784-1.764 1.75-1.764 1.75.79 1.75 1.764-.783 1.764-1.75 1.764zm13.5 12.268h-3v-5.604c0-3.368-4-3.113-4 0v5.604h-3v-11h3v1.765c1.396-2.586 7-2.777 7 2.476v6.759z"/>
          </svg>
          <span>LinkedIn</span>
        </a>
        
        <a
          v-if="profile.email"
          :href="`mailto:${profile.email}`"
          class="flex items-center space-x-1 text-sm text-primary-600 hover:text-primary-700 font-medium"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path>
          </svg>
          <span>Email</span>
        </a>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  profile: {
    type: Object,
    required: true
  }
})

// Parse skills - handle both array and single string with spaces
const parseSkills = (skills) => {
  if (!skills) return []
  if (Array.isArray(skills)) {
    // If it's an array with a single item that contains spaces/underscores, split it
    if (skills.length === 1 && typeof skills[0] === 'string' && skills[0].includes(' ')) {
      return skills[0].split(/\s+/).filter(s => s.trim())
    }
    return skills
  }
  // If it's a string, split by spaces
  if (typeof skills === 'string') {
    return skills.split(/\s+/).filter(s => s.trim())
  }
  return []
}

// Parse looking for - handle both array and single string
const parseLookingFor = (lookingFor) => {
  if (!lookingFor) return []
  if (Array.isArray(lookingFor)) {
    // If it's an array with a single item that contains multiple items, try to split
    if (lookingFor.length === 1 && typeof lookingFor[0] === 'string' && lookingFor[0].includes(' ')) {
      return lookingFor[0].split(/\s+/).filter(s => s.trim())
    }
    return lookingFor
  }
  if (typeof lookingFor === 'string') {
    return lookingFor.split(/\s+/).filter(s => s.trim())
  }
  return []
}
</script>

