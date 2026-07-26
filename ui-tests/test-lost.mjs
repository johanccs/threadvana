import { chromium } from 'playwright';
const browser = await chromium.launch({ headless: true });
const page = await browser.newPage({ viewport: { width: 1400, height: 950 } });
await page.goto('http://localhost:5099/lesson/c1-l17-debugging-multithreaded-code', { waitUntil: 'networkidle' });
await page.waitForTimeout(1500);
const dots = await page.$$('.concept-explainer .cx-dot');
await dots[8].click(); // step 9: lost update
await page.waitForTimeout(1400); // let token transition finish
await (await page.$('.concept-explainer')).screenshot({ path: 'screenshots/l17-race-lost.png' });
await browser.close();
console.log('done');
