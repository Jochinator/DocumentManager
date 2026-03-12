import { test, expect } from '@playwright/test';
import path from 'path';

test.describe.serial('Document Details', () => {

  test.beforeEach(async ({ page, browserName }) => {
    await page.goto(`/${browserName}/single-file-import`);
    const fileInput = page.locator('input[type="file"]');
    await fileInput.setInputFiles(path.join(__dirname, 'test-document-1.pdf'));
    await expect(page).toHaveURL(
      new RegExp(`/${browserName}/document/.*`),
      { timeout: 30000 }
    );
  });

  test('Should not be able to save when tag input is not empty', async ({ page }) => {
    const tagInput = page.locator('app-tag-selection input[placeholder="New Tag"]');
    await tagInput.fill('TestTag');

    await expect(page.getByRole('button', { name: 'Speichern' })).toBeDisabled();
  });

  test('Should be able to save after confirming tag with Enter', async ({ page }) => {
    const tagInput = page.locator('app-tag-selection input[placeholder="New Tag"]');
    await tagInput.fill('TestTag');

    await expect(page.getByRole('button', { name: 'Speichern' })).toBeDisabled();

    await tagInput.press('Enter');

    await expect(page.getByRole('button', { name: 'Speichern' })).toBeEnabled();
  });

  test('Should save document with title, date in german format, contact and tag', async ({ page, browserName }) => {
    // Titel eingeben
    await page.getByLabel('Titel').fill('Test Dokument');

    // Kontakt eingeben
    await page.getByLabel('Kontakt').fill('Max Mustermann');

    // Datum im deutschen Format eingeben
    await page.getByLabel('Datum').fill('15.03.2024');

    // Tag eingeben und bestätigen
    const tagInput = page.locator('app-tag-selection input[placeholder="New Tag"]');
    await tagInput.fill('TestTag');
    await tagInput.press('Enter');

    //Todo das ist nötig, weil das autocomplete der Tagauswahl sonst im Weg ist
    await page.locator('app-input[label="Titel"] input').click();
    // Speichern
    await page.getByRole('button', { name: 'Speichern' }).click();

    // Sollte zur Liste redirecten
    await expect(page).toHaveURL(`/${browserName}/list`, { timeout: 30000 });

    const headers = page.locator('p-table thead tr th');
    const kontaktIndex = await headers.evaluateAll(
      (ths) => ths.findIndex((th) => th.textContent?.includes('Kontakt'))
    );
    const titelIndex = await headers.evaluateAll(
      (ths) => ths.findIndex((th) => th.textContent?.includes('Titel'))
    );
    const datumIndex = await headers.evaluateAll(
      (ths) => ths.findIndex((th) => th.textContent?.includes('Datum'))
    );

    const row = page.locator('p-table tbody tr', { hasText: 'Test Dokument' });
    await expect(row.locator('td').nth(kontaktIndex)).toHaveText('Max Mustermann');
    await expect(row.locator('td').nth(titelIndex)).toHaveText('Test Dokument');
    await expect(row.locator('td').nth(datumIndex)).toHaveText('15.03.2024');
  });
});
