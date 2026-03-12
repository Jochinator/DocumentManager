README.md

markdown
# DocumentManager

A self-hosted document management system for PDFs with full-text search, tagging and multi-scope support.

## Features

- 📄 Import PDFs via browser upload or filesystem folder
- 🔍 Full-text search across all documents
- 🏷️ Automatic and manual tagging based on document content
- 📁 Multi-scope support (separate document collections)
- 🗂️ Optional filesystem view with symlinks organized by contact, year, tags and title
- 🐳 Docker support with automatic updates from GitHub

## Tech Stack

**Backend:**
- .NET 10 / ASP.NET Core
- Entity Framework Core with SQLite
- C#

**Frontend:**
- Angular 21
- Angular Material
- PrimeNG

## Getting Started

### Docker (recommended)

```yaml
version: '3'
services:
  documentmanager:
    image: documentmanager
    ports:
      - "5000:5000"
    volumes:
      - ./data:/app/documentManagerData
```

## Local Development

### Prerequisites:

    .NET 10 SDK
    Node.js 22
    Angular CLI

### Backend:

```
bash
cd DocumentManagerApi
dotnet run
```

### Frontend:

```
cd DocumentManagerFrontendAngular/DocumentManagerFrontend
npm install
npm start
```

### Configuration

```
appsettings.json:

json
{
  "PersistenceDefinitions": {
    "DataRootFolder": "documentManagerData",
    "DocumentFolder": "documents",
    "ImportFolder": "imports",
    "DeletedFolder": "deleted",
    "FailedFolder": "failed",
    "ViewFolder": "view",
    "GenerateFilesystemView": false
  }
}
```
## Filesystem View


When GenerateFilesystemView is set to true, the application generates a folder structure with symlinks organized by:

```
documentManagerData/
  view/
    {scope}/
      Kontakte/
        {contact}/
          {filename}.pdf → (symlink)
      Jahre/
        {year}/
          {filename}.pdf → (symlink)
      Tags/
        {tag}/
          {filename}.pdf → (symlink)
      Titel/
        {filename}.pdf → (symlink)

    Note: Filesystem view uses symlinks and is not supported on Windows.
```

## Import Folder

Place PDFs in the imports folder to automatically import them. The application will:

    Extract text content
    Automatically assign matching tags
    Move failed imports to the failed folder

## Scopes

Scopes allow you to separate documents into different collections.
Each scope has its own document list and filesystem view.

The active scope can be selected via a dropdown in the navigation.
Only documents belonging to the selected scope are displayed.


## File Naming

Imported documents are automatically named using the following pattern:

```
{YYMM}_{contact (max 10 chars)}{title (max 200 chars total)}.pdf
```

## Tags

Tags can be:

    Automatic - assigned when the tag text is found in the document
    Manual - only assigned manually (IsManualOnly = true)

## License

MIT
### Third Party Licenses

- [PdfPig](https://github.com/UglyToad/PdfPig) - Apache 2.0