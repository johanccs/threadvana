import { chromium } from 'playwright';
const browser = await chromium.launch({ headless: true });
const page = await browser.newPage({ viewport: { width: 1400, height: 900 } });
await page.goto('http://localhost:5099/lesson/c1-l17-debugging-multithreaded-code', { waitUntil: 'networkidle' });
await page.waitForTimeout(2000);
await page.screenshot({ path: 'c1-l17-before.png', fullPage: false });
const btn = await page.$('.demo-controls button');
if (btn) { await btn.click(); await page.waitForTimeout(6000); }
await page.screenshot({ path: 'c1-l17-after.png', fullPage: false });
// also capture just the viz area for close inspection
const viz = await page.$('.viz-host');
if (viz) await viz.screenshot({ path: 'c1-l17-viz-closeup.png' });
const console = await page.$('.console-output');
if (console) await console.screenshot({ path: 'c1-l17-console.png' });
await browser.close();
console.log('done');