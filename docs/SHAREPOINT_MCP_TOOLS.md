# SharePoint MCP Tools Documentation

This document describes the Microsoft Graph SharePoint MCP (Model Context Protocol) tools available for LLM agents to interrogate SharePoint sites.

## Overview

The SharePoint MCP tools provide LLM agents with the ability to:
- Navigate SharePoint folder structures (similar to `ls` command)
- Search for documents across a SharePoint site
- Retrieve detailed metadata for specific documents
- Access site information

## Prerequisites

### Azure AD App Registration

1. Register an application in Azure AD/Entra ID
2. Grant the following Microsoft Graph API permissions:
   - `Sites.Read.All` - Read items in all site collections
   - `Files.Read.All` - Read files in all site collections
3. Create a client secret for the application
4. Update `appsettings.json` with your credentials:

```json
{
  "MicrosoftGraph": {
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret"
  }
}
```

## Available MCP Tools

### 1. Get Site Information

**Endpoint:** `GET /mcp/sharepoint/sites/{siteId}`

Retrieves basic information about a SharePoint site.

**Parameters:**
- `siteId` - SharePoint site ID or hostname:path format (e.g., `contoso.sharepoint.com:/sites/teamsite`)

**Response:**
```json
{
  "id": "site-guid",
  "name": "Team Site",
  "displayName": "Team Site",
  "webUrl": "https://contoso.sharepoint.com/sites/teamsite",
  "description": "Site description",
  "createdDateTime": "2024-01-01T00:00:00Z",
  "lastModifiedDateTime": "2024-01-15T00:00:00Z"
}
```

### 2. List Folder Contents (ls)

**Endpoint:** `GET /mcp/sharepoint/sites/{siteId}/items?path={path}`

Lists all items (folders and documents) in a specified folder, similar to the `ls` command in Unix.

**Parameters:**
- `siteId` - SharePoint site ID
- `path` (optional) - Folder path (empty for root, or path like "/Documents/Folder1")

**Response:**
```json
[
  {
    "id": "item-id-1",
    "name": "Documents",
    "webUrl": "https://...",
    "parentPath": "",
    "isFolder": true,
    "size": 0,
    "createdDateTime": "2024-01-01T00:00:00Z",
    "lastModifiedDateTime": "2024-01-15T00:00:00Z",
    "createdBy": "John Doe",
    "lastModifiedBy": "Jane Smith"
  },
  {
    "id": "item-id-2",
    "name": "Report.docx",
    "webUrl": "https://...",
    "parentPath": "",
    "isFolder": false,
    "size": 12345,
    "createdDateTime": "2024-01-01T00:00:00Z",
    "lastModifiedDateTime": "2024-01-15T00:00:00Z",
    "createdBy": "John Doe",
    "lastModifiedBy": "Jane Smith"
  }
]
```

**Usage Example:**
- List root: `/mcp/sharepoint/sites/{siteId}/items`
- List subfolder: `/mcp/sharepoint/sites/{siteId}/items?path=/Documents/Projects`

### 3. Search Documents

**Endpoint:** `GET /mcp/sharepoint/sites/{siteId}/search?q={query}`

Searches for documents matching keywords across the entire SharePoint site.

**Parameters:**
- `siteId` - SharePoint site ID
- `q` - Search query (keywords to find in document names or content)

**Response:**
```json
[
  {
    "id": "doc-id",
    "name": "Q4 Budget Report.xlsx",
    "webUrl": "https://...",
    "parentPath": "/Documents/Finance",
    "isFolder": false,
    "size": 45678,
    "createdDateTime": "2024-01-01T00:00:00Z",
    "lastModifiedDateTime": "2024-01-15T00:00:00Z",
    "createdBy": "John Doe",
    "lastModifiedBy": "Jane Smith"
  }
]
```

### 4. Get Document Metadata

**Endpoint:** `GET /mcp/sharepoint/sites/{siteId}/items/{itemId}/metadata`

Retrieves detailed metadata for a specific document, including custom properties.

**Parameters:**
- `siteId` - SharePoint site ID
- `itemId` - Document item ID (obtained from list or search operations)

**Response:**
```json
{
  "id": "doc-id",
  "name": "Report.docx",
  "webUrl": "https://...",
  "parentPath": "/Documents",
  "size": 12345,
  "contentType": "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
  "createdDateTime": "2024-01-01T00:00:00Z",
  "lastModifiedDateTime": "2024-01-15T00:00:00Z",
  "createdBy": "John Doe",
  "lastModifiedBy": "Jane Smith",
  "metadata": {
    "customProperty1": "value1",
    "customProperty2": "value2"
  }
}
```

## LLM Agent Workflow Example

A typical workflow for an LLM agent to find documents on a specific topic:

1. **Get site info** to confirm access:
   ```
   GET /mcp/sharepoint/sites/contoso.sharepoint.com:/sites/teamsite
   ```

2. **List root folders** to understand structure:
   ```
   GET /mcp/sharepoint/sites/{siteId}/items
   ```

3. **Navigate to specific folders** of interest:
   ```
   GET /mcp/sharepoint/sites/{siteId}/items?path=/Documents/Finance
   ```

4. **Search for specific topics** across the site:
   ```
   GET /mcp/sharepoint/sites/{siteId}/search?q=budget 2024
   ```

5. **Get detailed metadata** for relevant documents:
   ```
   GET /mcp/sharepoint/sites/{siteId}/items/{itemId}/metadata
   ```

## Authentication

All endpoints require Azure AD authentication. Include a valid JWT bearer token in the Authorization header:

```
Authorization: Bearer <azure-ad-jwt-token>
```

## Error Responses

- `401 Unauthorized` - Missing or invalid authentication token
- `404 Not Found` - Site or item not found
- `400 Bad Request` - Invalid parameters or query

## Architecture

The implementation follows Clean Architecture principles:

- **Domain Layer**: SharePoint entities (Site, Folder, Document, DriveItem)
- **Infrastructure Layer**: Microsoft Graph service implementation
- **Application Layer**: CQRS queries and MediatR handlers
- **API Layer**: MCP-compatible REST controller

## Testing

Unit tests are provided in `Pixelbadger.Api.Application.Tests/SharePoint/` covering:
- Site info retrieval
- Folder listing
- Document search
- Metadata retrieval