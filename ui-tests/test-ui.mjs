import { chromium } from 'playwright';

const URL = 'http://localhost:5099';
const results = [];
let browser;

async function check(name, fn) {
  try { await fn(); results.push({ name, pass: true }); }
  catch (e) { results.push({ name, pass: false, error: e.message }); }
}

async function main() {
  browser = await chromium.launch({ headless: true });
  const page = await browser.newPage({ viewport: { width: 1400, height: 900 } });

  // ---------- Theory collapse ----------
  await page.goto(`${URL}/lesson/c1-l01-what-is-a-thread`);
  await page.waitForSelector('.lesson-grid', { timeout: 5000 });
  await page.waitForTimeout(800);

  const theoryBox = await page.locator('.theory-column').boundingBox();
  const theoryWidthBefore = theoryBox?.width ?? 0;
  console.log(`Theory width before: ${theoryWidthBefore}px`);

  await check('Theory column starts visible (width > 300px)', () => {
    if (theoryWidthBefore < 300) throw new Error(`width=${theoryWidthBefore}`);
  });

  const collapseBtn = page.locator('.btn-collapse-theory');
  await check('Collapse button exists', async () => {
    if (await collapseBtn.count() === 0) throw new Error('not found');
  });

  await collapseBtn.click();
  await page.waitForTimeout(400);

  const theoryWidthAfter = await page.locator('.theory-column').boundingBox().then(b => b?.width ?? 0);
  console.log(`Theory width after: ${theoryWidthAfter}px`);

  await check('Theory column collapses after click', () => {
    if (theoryWidthAfter >= theoryWidthBefore) throw new Error(`no collapse: before=${theoryWidthBefore} after=${theoryWidthAfter}`);
  });

  await check('Theory content hidden when collapsed', async () => {
    const breadcrumb = await page.locator('.breadcrumb').isVisible().catch(() => false);
    if (breadcrumb) throw new Error('breadcrumb still visible');
  });

  await collapseBtn.click();
  await page.waitForTimeout(400);
  const theoryWidthRestored = await page.locator('.theory-column').boundingBox().then(b => b?.width ?? 0);
  await check('Theory column re-expands after second click', () => {
    if (theoryWidthRestored < theoryWidthBefore * 0.8) throw new Error(`restored=${theoryWidthRestored} vs original=${theoryWidthBefore}`);
  });

  // ---------- Sidebar toggle ----------
  await check('Sidebar toggle button exists', async () => {
    if (await page.locator('.btn-sidebar-toggle').count() === 0) throw new Error('not found');
  });

  const sidebarVisibleBefore = await page.locator('.sidebar').isVisible().catch(() => false);
  console.log(`Sidebar visible before: ${sidebarVisibleBefore}`);

  await page.locator('.btn-sidebar-toggle').click();
  await page.waitForTimeout(300);

  const sidebarVisibleAfter = await page.locator('.sidebar').isVisible().catch(() => false);
  console.log(`Sidebar visible after: ${sidebarVisibleAfter}`);

  await check('Sidebar hides after toggle', () => {
    if (sidebarVisibleAfter) throw new Error('still visible after toggle');
  });

  await page.locator('.btn-sidebar-toggle').click();
  await page.waitForTimeout(300);

  const sidebarRestored = await page.locator('.sidebar').isVisible().catch(() => false);
  await check('Sidebar restores after second toggle', () => {
    if (!sidebarRestored) throw new Error('sidebar did not come back');
  });

  // ---------- Resize handle ----------
  await page.goto(`${URL}/lesson/c1-l01-what-is-a-thread`);
  await page.waitForSelector('.lesson-grid-handle', { timeout: 5000 });
  await page.waitForTimeout(800);

  const handle = page.locator('.lesson-grid-handle');
  await check('Resize handle element exists', async () => {
    if (await handle.count() === 0) throw new Error('not found');
  });

  const theoryBeforeDrag = await page.locator('.theory-column').boundingBox().then(b => b?.width ?? 0);
  console.log(`Theory width before drag: ${theoryBeforeDrag}px`);

  const handleBox = await handle.boundingBox();
  if (!handleBox) throw new Error('handle has no bounding box');

  await page.mouse.move(handleBox.x + handleBox.width / 2, handleBox.y + handleBox.height / 2);
  await page.mouse.down();
  await page.mouse.move(handleBox.x + handleBox.width / 2 + 150, handleBox.y + handleBox.height / 2, { steps: 10 });
  await page.mouse.up();
  await page.waitForTimeout(400);

  const theoryAfterDrag = await page.locator('.theory-column').boundingBox().then(b => b?.width ?? 0);
  console.log(`Theory width after drag: ${theoryAfterDrag}px`);

  await check('Theory width changes after drag', () => {
    const delta = Math.abs(theoryAfterDrag - theoryBeforeDrag);
    if (delta < 20) throw new Error(`delta=${delta}px (expected >20px)`);
  });

  // ---------- Exercise area ----------
  await page.goto(`${URL}/lesson/c1-l01-what-is-a-thread`);
  await page.waitForTimeout(800);

  await check('Run demo button exists', async () => {
    if (await page.locator('.btn-primary', { hasText: 'Run demo' }).count() === 0) throw new Error('not found');
  });

  await check('Exercise editor is present', async () => {
    const editor = await page.locator('.code-editor-host, .code-editor-fallback').count();
    if (editor === 0) throw new Error('no editor found');
  });

  await check('Check my code button exists', async () => {
    if (await page.locator('.btn-primary', { hasText: 'Check my code' }).count() === 0) throw new Error('not found');
  });

  // ---------- Results ----------
  console.log('\n=== RESULTS ===');
  let passed = 0, failed = 0;
  for (const r of results) {
    console.log(`${r.pass ? 'PASS' : 'FAIL'}  ${r.name}${r.error ? ' — ' + r.error : ''}`);
    r.pass ? passed++ : failed++;
  }
  console.log(`\n${passed} passed, ${failed} failed`);
  process.exit(failed > 0 ? 1 : 0);
}

main().catch(e => { console.error('FATAL:', e); process.exit(1); })
  .finally(() => browser?.close());
