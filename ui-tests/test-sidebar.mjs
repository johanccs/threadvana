import { chromium } from 'playwright';

const browser = await chromium.launch({ headless: true });
const page = await browser.newPage({ viewport: { width: 1400, height: 900 } });
const results = [];
async function check(name, fn) {
  try { await fn(); results.push({ name, pass: true }); }
  catch (e) { results.push({ name, pass: false, error: e.message }); }
}

// Collapse sidebar, reload, verify it STAYS collapsed
await page.goto('http://localhost:5099/lesson/c1-l01-what-is-a-thread');
await page.waitForTimeout(1000);
await page.locator('.btn-sidebar-toggle').click();   // collapse
await page.waitForTimeout(300);

await check('Sidebar hidden after click', async () => {
  if (await page.locator('.sidebar').isVisible().catch(() => false)) throw new Error('visible');
});

await page.reload({ waitUntil: 'load' });
await page.waitForTimeout(1000);

await check('Sidebar STAYS hidden after reload (persistence)', async () => {
  if (await page.locator('.sidebar').isVisible().catch(() => false)) throw new Error('came back');
});

await page.locator('.btn-sidebar-toggle').click();   // expand again
await page.waitForTimeout(300);
await check('Sidebar comes back after toggle', async () => {
  if (!(await page.locator('.sidebar').isVisible().catch(() => false))) throw new Error('still hidden');
});

console.log('\n=== RESULTS ===');
let p = 0, f = 0;
for (const r of results) { console.log(`${r.pass ? 'PASS' : 'FAIL'}  ${r.name}${r.error ? ' — ' + r.error : ''}`); r.pass ? p++ : f++; }
console.log(`\n${p} passed, ${f} failed`);
process.exit(f > 0 ? 1 : 0);
await browser.close();
