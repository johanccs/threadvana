import { chromium } from 'playwright';
const browser = await chromium.launch({ headless: true });
const page = await browser.newPage({ viewport: { width: 1400, height: 1000 } });

// 1) All 5 explainers on the smoke page, at two different moments
await page.goto('http://localhost:5099/viz-smoke', { waitUntil: 'networkidle' });
await page.waitForTimeout(1200);
await page.screenshot({ path: 'screenshots/exp-smoke-t1.png', fullPage: true });
await page.waitForTimeout(6500);
await page.screenshot({ path: 'screenshots/exp-smoke-t2.png', fullPage: true });

// 2) Per-explainer close-ups at 3 moments (element screenshots)
const explainers = await page.$('.concept-explainer');
for (let i = 0; i < explainers.length && i < 6; i++) {
  for (let s = 0; s < 3; s++) {
    await explainers[i].screenshot({ path: `screenshots/exp-${i}-step${s}.png` });
    await page.waitForTimeout(3300);
  }
}

// 3) Real lesson pages (integration)
const lessons = ['c1-l04-join-waiting', 'c1-l09-meet-the-thread-pool', 'c3-l03-semaphore-slim-n-spaces', 'c2-l04-under-the-hood'];
for (const id of lessons) {
  await page.goto(`http://localhost:5099/lesson/${id}`, { waitUntil: 'networkidle' });
  await page.waitForTimeout(2500);
  const el = await page.$('.concept-explainer');
  if (el) await el.screenshot({ path: `screenshots/exp-lesson-${id}.png` });
  else console.log('NO EXPLAINER on ' + id);
}
await browser.close();
console.log('done');
