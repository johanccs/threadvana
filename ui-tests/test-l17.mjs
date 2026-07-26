import { chromium } from 'playwright';
const browser = await chromium.launch({ headless: true });
const page = await browser.newPage({ viewport: { width: 1400, height: 950 } });
await page.goto('http://localhost:5099/lesson/c1-l17-debugging-multithreaded-code', { waitUntil: 'networkidle' });
await page.waitForTimeout(1200);
const el = await page.$('.concept-explainer');
console.log('concept-explainer present:', el !== null);
if (el) {
  await el.screenshot({ path: 'screenshots/l17-race-s0.png' });
  await page.waitForTimeout(3300 * 4); // ~step 4 (rewind)
  await el.screenshot({ path: 'screenshots/l17-race-s4.png' });
  await page.waitForTimeout(3300 * 4); // ~step 8 (lost update)
  await el.screenshot({ path: 'screenshots/l17-race-s8.png' });
}
await browser.close();
console.log('done');
