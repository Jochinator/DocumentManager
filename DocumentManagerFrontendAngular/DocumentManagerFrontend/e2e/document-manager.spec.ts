import { test, expect } from '@playwright/test';
import path from 'path';

test.describe.serial('Document Manager', () => {
  let beforeUrl: string;
  test.beforeEach(async ({ page, browserName }) => {
    await page.goto(`/${browserName}/list`);
  });

  test('Should show empty List for given scope initially', async ({ page, browserName }) => {
    await expect(page).toHaveURL(`/${browserName}/list`);

    const rows = page.locator('p-table tbody tr');
    await expect(rows).toHaveCount(0);
  });

  test('Should redirect to imported document, after importing a single document', async ({ page, browserName }) => {
    await page.goto(`/${browserName}/single-file-import`);

    const fileInput = page.locator('input[type="file"]');
    await fileInput.setInputFiles(path.join(__dirname, 'test-document-1.pdf'));

    await expect(page).toHaveURL(
      new RegExp(`/${browserName}/document/.*`),
      { timeout: 30000 }
    );
    beforeUrl = page.url();
  });

  test('Should save document with title and contact', async ({ page, browserName }) => {
    await page.goto(beforeUrl);

    await page.locator('app-input[label="Titel"] input').fill('Test Dokument');
    await page.locator('app-input[label="Kontakt"] input').fill('Max Mustermann');

    await page.getByRole('button', { name: 'Speichern' }).click();

    await expect(page).toHaveURL(`/${browserName}/list`, { timeout: 30000 });

    const rows = page.locator('p-table tbody tr');
    await expect(rows).toHaveCount(1);

    const headers = page.locator('p-table thead tr th');
    const kontaktIndex = await headers.evaluateAll(
      (ths) => ths.findIndex((th) => th.textContent?.includes('Kontakt'))
    );
    const titelIndex = await headers.evaluateAll(
      (ths) => ths.findIndex((th) => th.textContent?.includes('Titel'))
    );


    const row = rows.first();
    await expect(row.locator('td').nth(kontaktIndex)).toHaveText('Max Mustermann');
    await expect(row.locator('td').nth(titelIndex)).toHaveText('Test Dokument');
  });

});

