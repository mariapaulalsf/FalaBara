<template>
  <div class="landing-page">

    <b-navbar toggleable="lg" type="light" class="sabara-navbar fixed-top">
      <b-container>
        <b-navbar-brand href="#" class="font-weight-bold d-flex align-items-center">
          <b-img src="https://upload.wikimedia.org/wikipedia/commons/thumb/1/14/Bras%C3%A3o_de_Sabar%C3%A1.png/180px-Bras%C3%A3o_de_Sabar%C3%A1.png"
                 height="45" class="mr-2" />
          <span class="brand-name">FalaBará</span>
        </b-navbar-brand>

        <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>

        <b-collapse id="nav-collapse" is-nav>
          <b-navbar-nav class="ml-auto align-items-center">
            <b-nav-item href="#recursos" class="mr-3">
              <span class="nav-link-text">Recursos</span>
            </b-nav-item>
            <b-nav-item href="#como-funciona" class="mr-3">
              <span class="nav-link-text">Como Funciona</span>
            </b-nav-item>
            <b-nav-item :to="{ name: 'auth-login' }" class="mr-2">
              <span class="nav-link-text">Entrar</span>
            </b-nav-item>
            <b-button :to="{ name: 'auth-register' }" variant="gold" class="btn-gold font-weight-bold px-4">
              Cadastrar-se
            </b-button>
          </b-navbar-nav>
        </b-collapse>
      </b-container>
    </b-navbar>

    <div class="hero-section">
      <div class="hero-pattern"></div>
      <b-container class="hero-content">
        <b-row class="align-items-center min-vh-100">
          <b-col lg="6" class="text-white py-5">
            <div class="badge-hero mb-4">
              <feather-icon icon="ShieldIcon" size="16" class="mr-2" />
              Plataforma Oficial da Prefeitura de Sabará
            </div>
            <h1 class="hero-title mb-4">
              Sua voz transforma <br>
              <span class="text-gradient">Sabará</span>
            </h1>
            <p class="hero-subtitle mb-5">
              Relate problemas urbanos, vote nas prioridades e acompanhe as melhorias da nossa cidade histórica em tempo real.
            </p>

            <div class="d-flex flex-wrap gap-3 mb-5">
              <b-button :to="{ name: 'auth-register' }" size="lg" class="btn-hero-primary px-5 py-3">
                <feather-icon icon="UserPlusIcon" size="20" class="mr-2" />
                Começar Agora
              </b-button>
              <b-button :to="{ name: 'auth-login' }" size="lg" class="btn-hero-secondary px-5 py-3">
                Já tenho conta
                <feather-icon icon="ArrowRightIcon" size="20" class="ml-2" />
              </b-button>
            </div>

            <div class="hero-badges">
              <div class="badge-item">
                <feather-icon icon="CheckCircleIcon" size="20" class="text-gold" />
                <span>100% Gratuito</span>
              </div>
              <div class="badge-item">
                <feather-icon icon="LockIcon" size="20" class="text-gold" />
                <span>Seguro e Oficial</span>
              </div>
              <div class="badge-item">
                <feather-icon icon="ZapIcon" size="20" class="text-gold" />
                <span>Resposta Rápida</span>
              </div>
            </div>
          </b-col>

          <b-col lg="6" class="d-none d-lg-block">
            <div class="hero-visual">
              <div class="floating-element elem-1">
                <div class="glass-card">
                  <div class="d-flex align-items-center">
                    <div class="status-icon bg-gradient-success">
                      <feather-icon icon="CheckIcon" size="24" class="text-white" />
                    </div>
                    <div class="ml-3">
                      <div class="status-label">Resolvido</div>
                      <div class="status-title">Buraco na Rua Central</div>
                    </div>
                  </div>
                </div>
              </div>

              <div class="floating-element elem-2">
                <div class="glass-card">
                  <div class="d-flex align-items-center">
                    <div class="status-icon bg-gradient-warning">
                      <feather-icon icon="AlertCircleIcon" size="24" class="text-white" />
                    </div>
                    <div class="ml-3">
                      <div class="status-label">Em Progresso</div>
                      <div class="status-title">Iluminação Praça</div>
                    </div>
                  </div>
                </div>
              </div>

              <div class="floating-element elem-3">
                <div class="glass-card">
                  <div class="d-flex align-items-center">
                    <div class="status-icon bg-gradient-blue">
                      <feather-icon icon="ThumbsUpIcon" size="24" class="text-white" />
                    </div>
                    <div class="ml-3">
                      <div class="status-label">Mais Votado</div>
                      <div class="status-title">Limpeza de Terreno</div>
                    </div>
                  </div>
                </div>
              </div>

              <div class="hero-circle circle-1"></div>
              <div class="hero-circle circle-2"></div>
              <div class="hero-circle circle-3"></div>
            </div>
          </b-col>
        </b-row>
      </b-container>
    </div>

    <section id="recursos" class="features-section">
      <b-container>
        <div class="text-center mb-5 pb-4">
          <div class="section-badge mb-3">
            <feather-icon icon="StarIcon" size="16" class="mr-2" />
            RECURSOS
          </div>
          <h2 class="section-title mb-3">Como o FalaBará funciona</h2>
          <p class="section-subtitle">Simples, rápido e eficiente para toda a comunidade</p>
        </div>

        <b-row>
          <b-col md="4" class="mb-4">
            <div class="feature-card">
              <div class="feature-number">01</div>
              <div class="feature-icon-wrapper">
                <div class="feature-icon bg-gradient-blue">
                  <feather-icon icon="MapPinIcon" size="32" class="text-white" />
                </div>
              </div>
              <h3 class="feature-title">Relate Problemas</h3>
              <p class="feature-description">
                Encontrou um problema na cidade? Tire uma foto, marque a localização e envie seu relato em poucos segundos.
              </p>
              <ul class="feature-list">
                <li>
                  <feather-icon icon="MapIcon" size="16" />
                  <span>GPS automático</span>
                </li>
                <li>
                  <feather-icon icon="CameraIcon" size="16" />
                  <span>Fotos e vídeos</span>
                </li>
                <li>
                  <feather-icon icon="TagIcon" size="16" />
                  <span>Categorização</span>
                </li>
              </ul>
            </div>
          </b-col>

          <b-col md="4" class="mb-4">
            <div class="feature-card featured">
              <div class="popular-badge">
                <feather-icon icon="TrendingUpIcon" size="14" class="mr-1" />
                Mais Usado
              </div>
              <div class="feature-number">02</div>
              <div class="feature-icon-wrapper">
                <div class="feature-icon bg-gradient-gold">
                  <feather-icon icon="ThumbsUpIcon" size="32" class="text-white" />
                </div>
              </div>
              <h3 class="feature-title">Vote e Apoie</h3>
              <p class="feature-description">
                Dê visibilidade aos problemas mais urgentes. Quanto mais votos, maior a prioridade de resolução.
              </p>
              <ul class="feature-list">
                <li>
                  <feather-icon icon="TrendingUpIcon" size="16" />
                  <span>Sistema transparente</span>
                </li>
                <li>
                  <feather-icon icon="BarChart2Icon" size="16" />
                  <span>Ranking de prioridades</span>
                </li>
                <li>
                  <feather-icon icon="BellIcon" size="16" />
                  <span>Notificações</span>
                </li>
              </ul>
            </div>
          </b-col>

          <b-col md="4" class="mb-4">
            <div class="feature-card">
              <div class="feature-number">03</div>
              <div class="feature-icon-wrapper">
                <div class="feature-icon bg-gradient-success">
                  <feather-icon icon="CheckCircleIcon" size="32" class="text-white" />
                </div>
              </div>
              <h3 class="feature-title">Acompanhe Soluções</h3>
              <p class="feature-description">
                Receba atualizações em tempo real sobre o andamento e resolução dos problemas reportados.
              </p>
              <ul class="feature-list">
                <li>
                  <feather-icon icon="ActivityIcon" size="16" />
                  <span>Status em tempo real</span>
                </li>
                <li>
                  <feather-icon icon="ClockIcon" size="16" />
                  <span>Histórico completo</span>
                </li>
                <li>
                  <feather-icon icon="MessageSquareIcon" size="16" />
                  <span>Feedback da prefeitura</span>
                </li>
              </ul>
            </div>
          </b-col>
        </b-row>
      </b-container>
    </section>

    <section id="como-funciona" class="steps-section">
      <div class="steps-bg"></div>
      <b-container>
        <div class="text-center mb-5 pb-4">
          <div class="section-badge section-badge-light mb-3">
            <feather-icon icon="GitBranchIcon" size="16" class="mr-2" />
            PASSO A PASSO
          </div>
          <h2 class="section-title text-white mb-3">Comece em 3 passos simples</h2>
          <p class="section-subtitle text-white-50">Criar conta e fazer seu primeiro relato leva menos de 2 minutos</p>
        </div>

        <div class="steps-container">
          <div class="step-card">
            <div class="step-number-badge">
              <span>1</span>
            </div>
            <div class="step-icon">
              <feather-icon icon="UserPlusIcon" size="32" />
            </div>
            <h4 class="step-title">Cadastre-se</h4>
            <p class="step-description">
              Crie sua conta gratuita com e-mail e CPF em menos de 1 minuto.
            </p>
          </div>

          <div class="step-connector">
            <div class="connector-line"></div>
            <div class="connector-dot"></div>
          </div>

          <div class="step-card">
            <div class="step-number-badge">
              <span>2</span>
            </div>
            <div class="step-icon">
              <feather-icon icon="CameraIcon" size="32" />
            </div>
            <h4 class="step-title">Relate</h4>
            <p class="step-description">
              Tire uma foto do problema, escolha a categoria e publique.
            </p>
          </div>

          <div class="step-connector">
            <div class="connector-line"></div>
            <div class="connector-dot"></div>
          </div>

          <div class="step-card">
            <div class="step-number-badge">
              <span>3</span>
            </div>
            <div class="step-icon">
              <feather-icon icon="BellIcon" size="32" />
            </div>
            <h4 class="step-title">Acompanhe</h4>
            <p class="step-description">
              Receba notificações sobre o progresso e veja a solução acontecer.
            </p>
          </div>
        </div>
      </b-container>
    </section>

    <section class="cta-section">
      <b-container>
        <div class="cta-card">
          <b-row class="align-items-center">
            <b-col lg="8" class="text-center text-lg-left mb-4 mb-lg-0">
              <h2 class="cta-title mb-3">Faça parte da transformação de Sabará</h2>
              <p class="cta-subtitle mb-0">
                Junte-se aos cidadãos que estão ajudando a melhorar nossa cidade histórica
              </p>
            </b-col>
            <b-col lg="4" class="text-center text-lg-right">
              <b-button :to="{ name: 'auth-register' }" size="lg" class="btn-cta px-5 py-3">
                Criar minha conta
                <feather-icon icon="ArrowRightIcon" size="20" class="ml-2" />
              </b-button>
            </b-col>
          </b-row>
        </div>
      </b-container>
    </section>

    <footer class="footer-section">
      <b-container>
        <b-row class="mb-5">
          <b-col lg="4" class="mb-4 mb-lg-0">
            <div class="d-flex align-items-center mb-4">
              <b-img src="https://upload.wikimedia.org/wikipedia/commons/thumb/1/14/Bras%C3%A3o_de_Sabar%C3%A1.png/180px-Bras%C3%A3o_de_Sabar%C3%A1.png"
                     height="45" class="mr-3" />
              <h5 class="mb-0 footer-brand">FalaBará</h5>
            </div>
            <p class="footer-description">
              Plataforma oficial de ouvidoria da Prefeitura Municipal de Sabará. Conectando cidadãos e governo.
            </p>
          </b-col>

          <b-col lg="2" md="3" class="mb-4 mb-lg-0">
            <h6 class="footer-heading">Navegação</h6>
            <ul class="footer-links">
              <li><a href="#recursos">Recursos</a></li>
              <li><a href="#como-funciona">Como Funciona</a></li>
              <li><router-link :to="{ name: 'auth-login' }">Entrar</router-link></li>
              <li><router-link :to="{ name: 'auth-register' }">Cadastrar</router-link></li>
            </ul>
          </b-col>

          <b-col lg="3" md="4" class="mb-4 mb-lg-0">
            <h6 class="footer-heading">Suporte</h6>
            <ul class="footer-links">
              <li><a href="#">Central de Ajuda</a></li>
              <li><a href="#">Perguntas Frequentes</a></li>
              <li><a href="#">Política de Privacidade</a></li>
              <li><a href="#">Termos de Uso</a></li>
            </ul>
          </b-col>

          <b-col lg="3" md="5">
            <h6 class="footer-heading">Contato</h6>
            <div class="footer-contact">
              <div class="contact-item">
                <feather-icon icon="MapPinIcon" size="18" />
                <span>Centro Histórico<br>Sabará - MG</span>
              </div>
              <div class="contact-item">
                <feather-icon icon="PhoneIcon" size="18" />
                <span>(31) 3672-7599</span>
              </div>
            </div>
          </b-col>
        </b-row>

        <div class="footer-bottom">
          <p class="mb-0">&copy; 2026 FalaBará - Prefeitura Municipal de Sabará. Todos os direitos reservados.</p>
        </div>
      </b-container>
    </footer>

  </div>
</template>

<script>
import { BNavbar, BNavbarBrand, BNavbarToggle, BCollapse, BNavbarNav, BNavItem, BContainer, BButton, BRow, BCol, BImg } from 'bootstrap-vue'

export default {
  name: 'LandingPage',
  components: {
    BNavbar, BNavbarBrand, BNavbarToggle, BCollapse, BNavbarNav, BNavItem, BContainer, BButton, BRow, BCol, BImg
  }
}
</script>

<style scoped>
/* Color Palette */
:root {
  --blue-primary: #2563EB;
  --blue-dark: #1E40AF;
  --blue-darker: #1E3A8A;
  --gold: #F59E0B;
  --gold-dark: #D97706;
  --success: #10B981;
  --warning: #F59E0B;
}

.text-gold { color: var(--gold) !important; }

/* Navbar */
.sabara-navbar {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  box-shadow: 0 2px 20px rgba(0, 0, 0, 0.08);
  padding: 1.2rem 0;
}

.brand-name {
  color: var(--blue-primary);
  font-size: 1.6rem;
  font-weight: 800;
  letter-spacing: -0.5px;
}

.nav-link-text {
  color: #1F2937;
  font-weight: 600;
  transition: all 0.3s;
  font-size: 0.95rem;
}

.nav-link-text:hover {
  color: var(--blue-primary);
}

.btn-gold {
  background: linear-gradient(135deg, var(--gold) 0%, var(--gold-dark) 100%);
  border: none;
  color: white;
  font-weight: 700;
  border-radius: 50px;
  transition: all 0.3s;
  box-shadow: 0 4px 15px rgba(245, 158, 11, 0.3);
}

.btn-gold:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(245, 158, 11, 0.4);
}

/* Hero Section */
.hero-section {
  min-height: 100vh;
  background: linear-gradient(135deg, var(--blue-darker) 0%, var(--blue-dark) 50%, var(--blue-primary) 100%);
  position: relative;
  overflow: hidden;
}

.hero-pattern {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-image:
    radial-gradient(circle at 20% 30%, rgba(245, 158, 11, 0.15) 0%, transparent 50%),
    radial-gradient(circle at 80% 70%, rgba(37, 99, 235, 0.2) 0%, transparent 50%),
    repeating-linear-gradient(0deg, transparent, transparent 2px, rgba(255, 255, 255, 0.03) 2px, rgba(255, 255, 255, 0.03) 4px);
}

.hero-content {
  position: relative;
  z-index: 2;
}

.badge-hero {
  display: inline-flex;
  align-items: center;
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  padding: 0.75rem 1.5rem;
  border-radius: 50px;
  color: white;
  font-weight: 600;
  font-size: 0.85rem;
  letter-spacing: 0.3px;
}

.hero-title {
  font-size: 4rem;
  font-weight: 900;
  line-height: 1.1;
  letter-spacing: -2px;
  color: white;
}

.text-gradient {
  background: linear-gradient(135deg, var(--gold) 0%, #FCD34D 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.hero-subtitle {
  font-size: 1.25rem;
  color: rgba(255, 255, 255, 0.85);
  line-height: 1.7;
  max-width: 540px;
}

.gap-3 {
  gap: 1rem;
}

.btn-hero-primary {
  background: linear-gradient(135deg, var(--gold) 0%, var(--gold-dark) 100%);
  border: none;
  color: white;
  font-weight: 700;
  border-radius: 50px;
  transition: all 0.3s;
  box-shadow: 0 10px 30px rgba(245, 158, 11, 0.4);
}

.btn-hero-primary:hover {
  transform: translateY(-3px);
  box-shadow: 0 15px 40px rgba(245, 158, 11, 0.5);
}

.btn-hero-secondary {
  background: transparent;
  border: 2px solid rgba(255, 255, 255, 0.3);
  color: white;
  font-weight: 700;
  border-radius: 50px;
  transition: all 0.3s;
  backdrop-filter: blur(10px);
}

.btn-hero-secondary:hover {
  background: white;
  color: var(--blue-primary);
  border-color: white;
  transform: translateY(-3px);
}

.hero-badges {
  display: flex;
  gap: 2rem;
  flex-wrap: wrap;
}

.badge-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: white;
  font-weight: 600;
  font-size: 0.9rem;
}

/* Hero Visual */
.hero-visual {
  position: relative;
  height: 500px;
}

.floating-element {
  position: absolute;
  animation: float 6s ease-in-out infinite;
}

.elem-1 {
  top: 10%;
  left: 5%;
  animation-delay: 0s;
}

.elem-2 {
  top: 45%;
  right: 10%;
  animation-delay: 2s;
}

.elem-3 {
  bottom: 15%;
  left: 15%;
  animation-delay: 4s;
}

@keyframes float {
  0%, 100% {
    transform: translateY(0) rotate(0deg);
  }
  50% {
    transform: translateY(-25px) rotate(2deg);
  }
}

.glass-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 20px;
  padding: 1.5rem;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  min-width: 280px;
}

.status-icon {
  width: 56px;
  height: 56px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.bg-gradient-success {
  background: linear-gradient(135deg, #10B981, #059669);
  box-shadow: 0 8px 20px rgba(16, 185, 129, 0.3);
}

.bg-gradient-warning {
  background: linear-gradient(135deg, #F59E0B, #D97706);
  box-shadow: 0 8px 20px rgba(245, 158, 11, 0.3);
}

.bg-gradient-blue {
  background: linear-gradient(135deg, #3B82F6, #2563EB);
  box-shadow: 0 8px 20px rgba(37, 99, 235, 0.3);
}

.status-label {
  color: rgba(255, 255, 255, 0.7);
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: 0.25rem;
}

.status-title {
  color: white;
  font-size: 1rem;
  font-weight: 700;
}

.hero-circle {
  position: absolute;
  border-radius: 50%;
  background: radial-gradient(circle, rgba(245, 158, 11, 0.15), transparent);
  animation: pulse 8s ease-in-out infinite;
}

.circle-1 {
  width: 300px;
  height: 300px;
  top: -50px;
  right: -50px;
}

.circle-2 {
  width: 200px;
  height: 200px;
  bottom: 50px;
  left: -30px;
  animation-delay: 2s;
}

.circle-3 {
  width: 150px;
  height: 150px;
  top: 50%;
  right: 20%;
  animation-delay: 4s;
}

@keyframes pulse {
  0%, 100% {
    transform: scale(1);
    opacity: 0.3;
  }
  50% {
    transform: scale(1.1);
    opacity: 0.5;
  }
}

/* Features Section */
.features-section {
  padding: 120px 0;
  background: linear-gradient(180deg, #F9FAFB 0%, #FFFFFF 100%);
}

.section-badge {
  display: inline-flex;
  align-items: center;
  background: linear-gradient(135deg, rgba(37, 99, 235, 0.1), rgba(245, 158, 11, 0.1));
  border: 1px solid rgba(37, 99, 235, 0.2);
  padding: 0.6rem 1.5rem;
  border-radius: 50px;
  color: var(--blue-primary);
  font-weight: 700;
  font-size: 0.8rem;
  letter-spacing: 1px;
  text-transform: uppercase;
}

.section-badge-light {
  background: rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
}

.section-title {
  font-size: 3rem;
  font-weight: 900;
  color: #111827;
  letter-spacing: -1px;
}

.section-subtitle {
  font-size: 1.15rem;
  color: #6B7280;
  max-width: 600px;
  margin: 0 auto;
}

.feature-card {
  background: white;
  border-radius: 24px;
  padding: 2.5rem 2rem;
  height: 100%;
  position: relative;
  overflow: hidden;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  border: 2px solid transparent;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06);
}

.feature-card::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 5px;
  background: linear-gradient(90deg, var(--blue-primary), var(--gold));
  transform: scaleX(0);
  transition: transform 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.feature-card:hover {
  transform: translateY(-12px);
  box-shadow: 0 25px 50px rgba(0, 0, 0, 0.12);
  border-color: rgba(37, 99, 235, 0.1);
}

.feature-card:hover::before {
  transform: scaleX(1);
}

.feature-card.featured {
  background: linear-gradient(135deg, rgba(37, 99, 235, 0.05), rgba(245, 158, 11, 0.05));
  border: 2px solid var(--gold);
  box-shadow: 0 8px 30px rgba(245, 158, 11, 0.2);
}

.popular-badge {
  position: absolute;
  top: 1.5rem;
  right: 1.5rem;
  background: linear-gradient(135deg, var(--gold), var(--gold-dark));
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 50px;
  font-size: 0.7rem;
  font-weight: 700;
  text-transform: uppercase;
}
</style>
