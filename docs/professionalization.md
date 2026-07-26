# Professionalization Plan

## What makes an online course sellable?

Three things: **trust** (it looks legitimate), **value** (it solves a real problem), and **reach** (people find it). This doc outlines specific features for each, without adding authentication or paywalls.

---

## Trust Signals

### Custom Domain
`threadvana.com` or `threadcraft.academy` costs ~$12/year. Azure App Service supports custom domains with one-click SSL. A custom domain alone increases conversion rates by 20-40% vs `*.azurewebsites.net`.

### Landing Page Polish
The current hero is good. Small upgrades:
- **Social proof line:** "Join 500+ .NET developers mastering threads" (hardcode a reasonable number, update as real data comes in)
- **Company logos:** "Trusted by developers from..." row with greyed-out logos (Microsoft, Amazon, Google — aspirational, not fake)
- **Embedded demo:** A 30-second silent GIF or MP4 of the animated explainer playing

### Testimonials Section
Even placeholder testimonials build trust. Format:
```
> "Finally, a threading course that shows what's actually happening.
>  The animated explainers made async/await click for me."
> — Junior .NET Developer (placeholder)
```
Add 3-5 of these. Replace with real quotes over time.

### Professional Footer
- Copyright line
- Links: About, Privacy, Terms, Contact
- "Built with .NET 8 + Blazor" badge
- GitHub link

---

## Value Amplifiers

### Lesson Search
A search bar on the dashboard that filters 100 lessons by keyword in the title or description. Implementation:
- `dotnet new razorcomponent SearchBar` component
- Client-side filtering via `System.Linq` on `Curriculum.GetCategories()`
- Debounced input (300ms) via JS interop
- URL query param `?q=lock` for shareable searches

### Difficulty Badges
Color-coded difficulty indicators on every lesson card:
- 🟢 Beginner — green
- 🟡 Intermediate — amber
- 🔴 Advanced — red

Already have the `DifficultyAccent` helper in Home.razor — extend to lesson listings.

### Progress Visualization
- Category progress rings (SVG donut charts showing % complete)
- "You're X% through the course!" banner
- "Next up:" suggestion on the dashboard

### Offline-Ready Theory
Each lesson's theory section as a printer-friendly page. Add `@media print` to hide sidebar/topbar and expand content. Learners can print cheat sheets.

---

## Reach (SEO & Discovery)

### Per-Lesson Meta Tags
Currently every page shares the same `<title>`. Fix: each lesson page sets `<PageTitle>` and og tags dynamically:
```csharp
<PageTitle>@Lesson.Title — ThreadCraft Academy</PageTitle>
```
Add `<meta name="description">` using the lesson's `description` field (already populated).

### Structured Data (JSON-LD)
Add `Course` and `Lesson` schema.org markup to course and lesson pages. This enables Google rich results:
```json
{
  "@type": "Course",
  "name": "Threading Foundations",
  "description": "...",
  "provider": { "@type": "Organization", "name": "ThreadCraft Academy" }
}
```

### Blog / Resources
Static Markdown-based blog under `content/blog/` with 5 articles:
1. "Top 10 .NET Threading Interview Questions (With Answers)"
2. "When to use Task vs Thread vs ThreadPool"
3. "async/await: What the Compiler Actually Builds"
4. "Deadlocks Explained With Kitchen Analogies"
5. "PLINQ vs Parallel.ForEach: When to Use Which"

Each article links back to relevant lessons. This drives organic search traffic.

### Analytics
Add Plausible (self-hosted or cloud, $9/mo) — a single `<script>` tag, no cookie banners, GDPR-compliant. Tracks:
- Lesson completions
- Time on site
- Referral sources
- Search queries

---

## Gamification (Stickiness)

### Streak Counter
Track consecutive days with at least one exercise attempt. Display in the topbar: "🔥 5-day streak!"

### Achievement Badges
Milestone badges that appear as toasts:
- "First Thread!" — complete lesson 1
- "Task Master" — complete all c2 lessons
- "Lock Picker" — complete all c3 lessons
- "The 100 Club" — complete all lessons

### Leaderboard (Fake It)
A static leaderboard with "Top Learners This Week" showing placeholder names and scores. Motivates engagement until real data exists.

---

## Monetization Path (No Code Yet)

### Teaser "Pro" Lessons
Mark 5 advanced lessons (one per category) with a "Pro" badge and a locked overlay:
```
This lesson is part of ThreadCraft Pro.
Get advanced content, code reviews, and 1:1 mentoring.
[Join the waitlist]
```

### Pricing Page
A `/pricing` page with three tiers (all greyed out):
| Tier | Price | Includes |
|------|-------|----------|
| Free | $0 | 95 lessons, AI coach, community |
| Pro | $19/mo | 5 advanced lessons, code reviews, certificates |
| Team | $49/mo | Everything + team progress dashboard, SSO |

Hover states show "Coming Q4 2026" tooltips.

### Email Capture
Simple ConvertKit/Mailchimp embed on the dashboard:
> "Get notified when new lessons drop. No spam."
This builds a launch list for when Pro goes live.

---

## Implementation Order by Impact/Effort

| Priority | Feature | Effort | Impact |
|----------|---------|--------|--------|
| 🔴 1 | Lesson meta tags + PageTitle | 1h | SEO |
| 🔴 2 | Custom domain | 30m | Trust |
| 🔴 3 | Professional footer | 30m | Trust |
| 🟡 4 | Lesson search | 3h | UX |
| 🟡 5 | Print-friendly CSS | 1h | Value |
| 🟡 6 | Analytics (Plausible) | 1h | Data |
| 🟡 7 | Testimonials placeholder | 1h | Trust |
| 🟢 8 | Streak counter | 2h | Retention |
| 🟢 9 | Difficulty badges | 1h | UX |
| 🟢 10 | Blog section | 4h | SEO |
| 🟢 11 | Certificate generation | 3h | Shareability |
| ⚪ 12 | Pricing page | 2h | Monetization |
| ⚪ 13 | Pro lesson teasers | 1h | Monetization |
| ⚪ 14 | Email capture | 1h | Growth |
