// Render SVG files to PNG with transparency. Usage: node render.js manifest.json
const fs = require('fs'); const { chromium } = require('playwright');
(async () => {
  const jobs = JSON.parse(fs.readFileSync(process.argv[2], 'utf8'));
  const browser = await chromium.launch();
  const ctxs = {};
  for (const j of jobs) {
    const s = j.scale || 2;
    if (!ctxs[s]) ctxs[s] = await (await browser.newContext({ deviceScaleFactor: s })).newPage();
    const page = ctxs[s];
    await page.setViewportSize({ width: j.w, height: j.h });
    const svg = fs.readFileSync(j.svg, 'utf8');
    await page.setContent(`<html><body style="margin:0;background:transparent">${svg}</body></html>`);
    await page.screenshot({ path: j.png, omitBackground: true, clip: { x: 0, y: 0, width: j.w, height: j.h } });
  }
  await browser.close();
})();
