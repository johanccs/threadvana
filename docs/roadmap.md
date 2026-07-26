# Roadmap

## Phase 1 — Content Quality ✅
- [x] Fix all double-UTF-8-encoded characters across 100+ lesson files
- [x] Add comprehensive descriptions to every lesson front matter
- [x] Flesh out abbreviated PLINQ, Channels, and CAS lessons with full writing-style skeleton
- [ ] Verify every exercise has hints (1 test failing)

## Phase 2 — Professionalization (no auth/paywall)

### Landing Page
- [ ] Hero animation polish — smooth entrance transitions
- [ ] Social proof section — "X concepts explained, Y demos runnable"
- [ ] Embedded video walkthrough (2 min screen recording of a lesson)
- [ ] "As seen on" placeholder logos (GitHub, Azure, .NET Foundation)

### Navigation & Discovery
- [ ] Lesson search bar on dashboard — filter by keyword across all 100 lessons
- [ ] Difficulty filter tabs (Beginner / Intermediate / Advanced)
- [ ] "Jump to next uncompleted" button in topbar
- [ ] Breadcrumb navigation on lesson pages

### Gamification
- [ ] Streak counter — days since first lesson
- [ ] Completion percentage badges per category
- [ ] "First try" / "N attempts" stats per exercise
- [ ] Total time spent learning counter

### Visual Polish
- [ ] Dark/light mode toggle (persisted to sessionStorage)
- [ ] Print-friendly lesson pages (`@media print` CSS)
- [ ] Loading skeletons instead of blank screens
- [ ] Smooth page transitions

### Certificates
- [ ] Generate SVG certificate when all 100 lessons passed
- [ ] Shareable link with certificate preview (Open Graph tags)
- [ ] Download as PNG option

### SEO & Analytics
- [ ] Per-lesson meta tags (title, description, og:image)
- [ ] JSON-LD structured data for course/category pages
- [ ] Plausible or Umami analytics (privacy-friendly, no cookie banner)
- [ ] XML sitemap auto-refresh on content changes

### Content Marketing
- [ ] Blog section — 5-10 articles on multithreading topics
- [ ] "Top 10 .NET threading interview questions" long-form post
- [ ] RSS feed for blog
- [ ] Email capture — "Get notified when new lessons drop"

## Phase 3 — Monetization Ready (no auth yet)

### Teaser Features
- [ ] "Pro" badge on 5 locked advanced lessons with coming-soon messaging
- [ ] Pricing page placeholder — Team / Enterprise / Individual tiers
- [ ] Feature comparison table on pricing page

### Social Proof
- [ ] Testimonials page with placeholder quotes
- [ ] Twitter/LinkedIn share cards for completed lessons
- [ ] "X learners joined this week" counter (static for now)

### Resources
- [ ] Affiliate links to recommended books (CLR via C#, Concurrency in C# Cookbook)
- [ ] External tools/resources page
- [ ] "Hire me for training" contact section

## Phase 4 — Production Readiness

### Infrastructure
- [ ] Custom domain (threadvana.com or threadcraft.academy)
- [ ] HTTPS/TLS certificate (auto via Azure)
- [ ] CDN for static assets
- [ ] CI/CD pipeline (GitHub Actions → Azure)
- [ ] Automated content validation in CI

### Monitoring
- [ ] Application Insights or OpenTelemetry
- [ ] Uptime monitoring (Azure Availability Tests)
- [ ] Error tracking (Sentry or similar)

### Performance
- [ ] Response caching for static lesson theory
- [ ] Bundle/minify CSS and JS
- [ ] Brotli compression enabled
- [ ] Lazy load below-the-fold visualizations
