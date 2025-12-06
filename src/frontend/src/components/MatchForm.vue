<template>
  <div class="bg-white rounded-lg shadow-lg p-6">
    <!-- Header -->
    <div class="text-center mb-6">
      <h2 class="text-3xl font-bold text-gray-900">
        The 'Coffee Break' Roulette
        </h2>
    </div>
    
    <!-- Email Section at Top -->
    <div class="bg-gray-50 rounded-lg p-4 mb-6 border border-gray-200">
      <div class="flex gap-2 mb-3">
        <div class="flex-1">
          <input
            v-model="email"
            type="email"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
            placeholder="Email для автозаполнения"
          />
        </div>
        <button
          type="button"
          @click="loadProfileByEmail"
          :disabled="loadingProfile || !email"
          class="px-6 py-2 bg-primary-600 text-white rounded-lg font-semibold hover:bg-primary-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors whitespace-nowrap"
        >
          <span v-if="loadingProfile">⏳</span>
          <span v-else>🔄 Обновить</span>
        </button>
      </div>
      
      <!-- Or Random Button -->
      <div class="text-center">
        <span class="text-xs text-gray-500 mr-2">или</span>
        <button
          type="button"
          @click="generateRandomRequest"
          class="text-sm text-primary-600 hover:text-primary-700 font-medium hover:underline"
        >
          🎲 Случайный запрос
        </button>
      </div>
      
      <!-- Error Message -->
      <div v-if="emailError" class="mt-3 text-sm text-red-600 flex items-center">
        <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"></path>
        </svg>
        {{ emailError }}
      </div>
      
      <!-- Success Message -->
      <div v-if="emailSuccess" class="mt-3 text-sm text-green-600 flex items-center">
        <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
        </svg>
        {{ emailSuccess }}
      </div>
    </div>

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

      <!-- Submit Button - Full Width at Bottom -->
      <button
        type="submit"
        :disabled="loading || !formData.mainActivity || !formData.interests"
        class="w-full bg-primary-600 text-white py-3 px-6 rounded-lg font-semibold hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
      >
        <span v-if="loading">⏳ Поиск...</span>
        <span v-else>🤝 Match!</span>
      </button>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { userProfileService } from '../services/api'

const emit = defineEmits(['match'])

const email = ref('')
const loadingProfile = ref(false)
const emailError = ref('')
const emailSuccess = ref('')

const formData = ref({
  mainActivity: '',
  interests: '',
  country: '',
  city: ''
})

const loading = ref(false)

// Load profile by email
const loadProfileByEmail = async () => {
  if (!email.value) return
  
  loadingProfile.value = true
  emailError.value = ''
  emailSuccess.value = ''
  
  try {
    const profile = await userProfileService.getByEmail(email.value)
    
    // Fill form with profile data
    formData.value.mainActivity = profile.parsedMainActivity || ''
    formData.value.interests = profile.parsedInterests || ''
    formData.value.country = profile.parsedCountry || ''
    formData.value.city = profile.parsedCity || ''
    
    emailSuccess.value = `Данные загружены для ${profile.name}`
    setTimeout(() => {
      emailSuccess.value = ''
    }, 3000)
  } catch (err) {
    emailError.value = err.message || 'Не удалось загрузить профиль'
  } finally {
    loadingProfile.value = false
  }
}

// Predefined sample profiles for random generation
const sampleProfiles = [
  // === Разработчики: Frontend ===
  {
    mainActivity: 'Frontend разработчик',
    interests: 'React, Vue.js, TypeScript, UI/UX, Web Performance',
    country: 'Россия',
    city: 'Москва'
  },
  {
    mainActivity: 'Frontend Developer',
    interests: 'Angular, RxJS, Web Components, Микрофронтенды, Дизайн-системы',
    country: 'Россия',
    city: 'Санкт-Петербург'
  },
  {
    mainActivity: 'Frontend разработчик',
    interests: 'Next.js, React, Tailwind CSS, Анимации, Accessibility',
    country: 'Грузия',
    city: 'Тбилиси'
  },

  // === Разработчики: Backend ===
  {
    mainActivity: 'Backend разработчик',
    interests: 'Node.js, PostgreSQL, Microservices, GraphQL, Docker',
    country: 'Россия',
    city: 'Новосибирск'
  },
  {
    mainActivity: 'Backend Developer',
    interests: 'Python, Django, FastAPI, Redis, Celery, API Design',
    country: 'Россия',
    city: 'Казань'
  },
  {
    mainActivity: 'Backend разработчик',
    interests: 'Go, Kubernetes, High Load, Распределенные системы, gRPC',
    country: 'Россия',
    city: 'Екатеринбург'
  },
  {
    mainActivity: 'Backend разработчик',
    interests: 'C#, .NET Core, Azure, Microservices, Event Sourcing',
    country: 'Беларусь',
    city: 'Минск'
  },
  {
    mainActivity: 'Java Backend Developer',
    interests: 'Spring Boot, Kafka, Elasticsearch, Highload, Архитектура',
    country: 'Армения',
    city: 'Ереван'
  },

  // === Разработчики: Full Stack ===
  {
    mainActivity: 'Full Stack разработчик',
    interests: 'React, Node.js, MongoDB, AWS, DevOps, Стартапы',
    country: 'Россия',
    city: 'Москва'
  },
  {
    mainActivity: 'Full Stack Developer',
    interests: 'Vue.js, Python, PostgreSQL, Docker, CI/CD, Opensource',
    country: 'Кипр',
    city: 'Лимассол'
  },
  {
    mainActivity: 'Fullstack разработчик',
    interests: 'TypeScript, NestJS, React, GraphQL, Тестирование, TDD',
    country: '',
    city: ''
  },

  // === Разработчики: Mobile ===
  {
    mainActivity: 'iOS разработчик',
    interests: 'Swift, SwiftUI, Combine, App Architecture, Mobile UX',
    country: 'Россия',
    city: 'Санкт-Петербург'
  },
  {
    mainActivity: 'Android разработчик',
    interests: 'Kotlin, Jetpack Compose, Clean Architecture, Performance',
    country: 'Россия',
    city: 'Москва'
  },
  {
    mainActivity: 'Mobile разработчик',
    interests: 'Flutter, Dart, Cross-platform, Firebase, Mobile Design',
    country: 'Казахстан',
    city: 'Алматы'
  },
  {
    mainActivity: 'React Native разработчик',
    interests: 'React Native, TypeScript, Redux, Mobile CI/CD, Expo',
    country: 'Узбекистан',
    city: 'Ташкент'
  },

  // === Разработчики: Специализированные ===
  {
    mainActivity: 'DevOps Engineer',
    interests: 'Kubernetes, Terraform, GitLab CI, Monitoring, Prometheus',
    country: 'Германия',
    city: 'Берлин'
  },
  {
    mainActivity: 'Blockchain разработчик',
    interests: 'Solidity, Ethereum, Web3, DeFi, Smart Contracts, NFT',
    country: '',
    city: ''
  },
  {
    mainActivity: 'ML Engineer',
    interests: 'Python, TensorFlow, PyTorch, NLP, Computer Vision, MLOps',
    country: 'Россия',
    city: 'Москва'
  },
  {
    mainActivity: 'Game Developer',
    interests: 'Unity, C#, Game Design, 3D Graphics, Мультиплеер',
    country: 'Польша',
    city: 'Варшава'
  },
  {
    mainActivity: 'Data Engineer',
    interests: 'Apache Spark, Airflow, Data Warehousing, ETL, Big Data',
    country: 'Нидерланды',
    city: 'Амстердам'
  },
  {
    mainActivity: 'QA Engineer',
    interests: 'Автотестирование, Selenium, pytest, CI/CD, Performance testing',
    country: 'Россия',
    city: 'Нижний Новгород'
  },

  // === Инвесторы ===
  {
    mainActivity: 'Angel Investor',
    interests: 'Стартапы на ранней стадии, EdTech, FinTech, Менторство',
    country: 'Россия',
    city: 'Москва'
  },
  {
    mainActivity: 'Venture Capitalist',
    interests: 'Pre-seed инвестиции, SaaS, B2B, Product-market fit, Scaling',
    country: 'США',
    city: 'Сан-Франциско'
  },
  {
    mainActivity: 'Бизнес-ангел',
    interests: 'Технологические стартапы, AI/ML проекты, Нетворкинг, Exit strategy',
    country: 'ОАЭ',
    city: 'Дубай'
  },
  {
    mainActivity: 'Private Equity инвестор',
    interests: 'Growth-stage компании, M&A, Корпоративные финансы, Due diligence',
    country: 'Великобритания',
    city: 'Лондон'
  },
  {
    mainActivity: 'Crypto инвестор',
    interests: 'Web3, DeFi протоколы, Токеномика, Blockchain инфраструктура',
    country: '',
    city: ''
  },
  {
    mainActivity: 'Инвестор в недвижимость',
    interests: 'PropTech, Коммерческая недвижимость, REITs, Девелопмент',
    country: 'Россия',
    city: 'Сочи'
  },

  // === Предприниматели и Менеджеры ===
  {
    mainActivity: 'Основатель стартапа',
    interests: 'Product Management, Growth Hacking, Fundraising, Lean Startup',
    country: 'Россия',
    city: 'Москва'
  },
  {
    mainActivity: 'Tech Entrepreneur',
    interests: 'SaaS, B2B продажи, Product-led growth, Масштабирование команд',
    country: 'Сингапур',
    city: 'Сингапур'
  },
  {
    mainActivity: 'Product Manager',
    interests: 'Product Discovery, User Research, A/B тестирование, Метрики',
    country: 'Россия',
    city: 'Санкт-Петербург'
  },
  {
    mainActivity: 'CTO',
    interests: 'Техническая архитектура, Team Leadership, Tech Stack, R&D',
    country: 'Эстония',
    city: 'Таллин'
  },

  // === Дизайнеры ===
  {
    mainActivity: 'Product Designer',
    interests: 'UI/UX, Figma, Дизайн-системы, Прототипирование, User Testing',
    country: 'Россия',
    city: 'Москва'
  },
  {
    mainActivity: 'UX Researcher',
    interests: 'User Research, Качественные исследования, CJM, Analytics',
    country: 'Канада',
    city: 'Торонто'
  },

  // === Маркетинг и Продажи ===
  {
    mainActivity: 'Growth Marketer',
    interests: 'Performance Marketing, SEO, Контент-маркетинг, Аналитика',
    country: 'Россия',
    city: 'Москва'
  },
  {
    mainActivity: 'B2B Sales Manager',
    interests: 'Enterprise Sales, SaaS продажи, CRM, Переговоры, Networking',
    country: 'Германия',
    city: 'Мюнхен'
  },

  // === Data & AI ===
  {
    mainActivity: 'Data Scientist',
    interests: 'Machine Learning, Python, Статистика, A/B тесты, Прогнозирование',
    country: 'Швейцария',
    city: 'Цюрих'
  },
  {
    mainActivity: 'AI Researcher',
    interests: 'Deep Learning, Transformer модели, NLP, Research Papers, PyTorch',
    country: '',
    city: ''
  }
]

const generateRandomRequest = () => {
  const randomProfile = sampleProfiles[Math.floor(Math.random() * sampleProfiles.length)]
  formData.value = { ...randomProfile }
}

const handleMatch = async () => {
  loading.value = true
  try {
    const request = {
      mainActivity: formData.value.mainActivity,
      interests: formData.value.interests,
      topK: 3 // Always return top 3 most relevant matches
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

