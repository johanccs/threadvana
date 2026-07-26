import { chromium } from 'playwright';
const browser = await chromium.launch({ headless: true });
const page = await browser.newPage({ viewport: { width: 1400, height: 950 } });
await page.goto('http://localhost:5099/viz-smoke', { waitUntil: 'networkidle' });
await page.waitForTimeout(1200);
const els = await page.$$('.concept-explainer');
console.log('explainers on smoke page:', els.length);
const names = ['basics','join','pool','sem','async','race','fgbg','cancel','deadlock','lock','channel','tls','gate'];
for (let i = 6; i <= 12 && i < els.length; i++) {
  await els[i].screenshot({ path: `screenshots/new-${names[i]}-a.png` });
}
await page.waitForTimeout(6500);
for (let i = 6; i <= 12 && i < els.length; i++) {
  await els[i].screenshot({ path: `screenshots/new-${names[i]}-b.png` });
}
const lessons = ['c1-l05-foreground-vs-background','c3-l02-lock-one-key','c5-l02-deadlock-detective','c4-l03-channels','c1-l14-cooperative-cancellation'];
for (const id of lessons) {
  await page.goto(`http://localhost:5099/lesson/${id}`, { waitUntil: 'networkidle' });
  await page.waitForTimeout(1800);
  const el = await page.$('.concept-explainer');
  if (el) await el.screenshot({ path: `screenshots/new-lesson-${id}.png` });
  else console.log('NO EXPLAINER on ' + id);
}
await browser.close();
console.log('done');
