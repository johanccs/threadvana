import { chromium } from 'playwright';

const URL = 'http://localhost:5099/lesson/c1-l17-debugging-multithreaded-code';
const browser = await chromium.launch({ headless: true });
const page = await browser.newPage({ viewport: { width: 1400, height: 900 } });

const results = [];

async function check(name, fn) {
  try { await fn(); results.push({ name, pass: true }); }
  catch (e) { results.push({ name, pass: false, error: e.message }); }
}

await page.goto(URL, { waitUntil: 'networkidle' });
await page.waitForTimeout(2000);

await check('page loads', async () => {
  const title = await page.title();
  if (!title.includes('Debugging')) throw new Error(`Unexpected title: ${title}`);
});

await check('lesson header visible', async () => {
  const h1 = await page.$('.lesson-header h1');
  if (!h1) throw new Error('No h1 found');
  const text = await h1.textContent();
  if (!text.includes('Debugging')) throw new Error(`Header: ${text}`);
});

await check('theory text renders', async () => {
  const theory = await page.$('.theory-body');
  if (!theory) throw new Error('No .theory-body');
  const html = await theory.innerHTML();
  if (html.length < 200) throw new Error('Theory body too short');
});

await check('Run demo button exists', async () => {
  const btn = await page.$('.demo-controls button');
  if (!btn) throw new Error('No demo button');
});

// Screenshot before running demo
await page.screenshot({ path: 'c1-l17-before-demo.png', fullPage: true });

await check('run demo', async () => {
  const btn = await page.$('.demo-controls button');
  if (btn) await btn.click();
  await page.waitForTimeout(8000);
});

// Check if viz appeared
await check('viz appeared after demo', async () => {
  const viz = await page.$('.viz-host');
  if (!viz) throw new Error('No .viz-host found');
});

// Check console output appeared
await check('console output appeared', async () => {
  const console = await page.$('.console-output');
  if (!console) throw new Error('No .console-output');
  const text = await console.textContent();
  if (text.trim().length < 10) throw new Error('Console output empty');
});

// Screenshot after demo
await page.screenshot({ path: 'c1-l17-after-demo.png', fullPage: true });

// Check for any obvious layout issues
await check('no overflow errors', async () => {
  const body = await page.$('body');
  const scrollW = await body.evaluate(el => el.scrollWidth);
  if (scrollW > 1400) console.log(`WARN: horizontal overflow: ${scrollW}px`);
});

console.log('\n=== RESULTS ===');
results.forEach(r => console.log(r.pass ? '✅' : '❌', r.name, r.error || ''));

await browser.close();
