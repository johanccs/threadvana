import { chromium } from 'playwright';
import { readdirSync, readFileSync, writeFileSync, statSync } from 'fs';
import { join } from 'path';

const root = 'content/lessons';
const lessons = [];
for (const cat of readdirSync(root)) {
  if (!statSync(join(root, cat)).isDirectory()) continue;
  for (const dir of readdirSync(join(root, cat))) {
    if (!statSync(join(root, cat, dir)).isDirectory()) continue;
    const md = readFileSync(join(root, cat, dir, 'lesson.md'), 'utf8');
    const id = md.match(/^id:\s*(.+)$/m)?.[1].trim();
    const expl = md.match(/^explainer:\s*(.+)$/m)?.[1].trim() || null;
    const viz = md.match(/^visualization:\s*(.+)$/m)?.[1].trim() || null;
    if (id) lessons.push({ id, dir, expl, viz });
  }
}
console.log('total lessons:', lessons.length);

const DEF = { 'thread-pool': 'thread-pool', 'semaphore': 'semaphore', 'async-activity': 'async-state-machine' };
const browser = await chromium.launch({ headless: true });
const page = await browser.newPage({ viewport: { width: 1300, height: 900 } });
const missing = [];
for (const l of lessons) {
  await page.goto(`http://localhost:5099/lesson/${l.id}`, { waitUntil: 'domcontentloaded', timeout: 15000 });
  await page.waitForTimeout(500);
  const found = await page.$('.concept-explainer') !== null;
  const expect = l.expl || DEF[l.viz] || null;
  if (!found) missing.push(`${l.id} (explainer=${expect ?? 'NONE'})`);
}
console.log('pages WITHOUT rendered explainer:', missing.length);
for (const m of missing) console.log('  -', m);
writeFileSync('screenshots/_coverage.json', JSON.stringify({ total: lessons.length, missing }, null, 1));
await browser.close();
console.log('done');
