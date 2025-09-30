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
2. Grant the following Microsoft Graph API **delegated** permissions (for on-behalf-of flow):
   - `Sites.Read.All` - Read items in all site collections
   - `Files.Read.All` - Read files in all site collections
3. Grant **admin consent** for these permissions
4. Create a client secret for the application
5. Update `appsettings.json` with your credentials:

```json
{
  "MicrosoftGraph": {
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret"
  }
}
```

**Important:** The API uses OAuth 2.0 On-Behalf-Of (OBO) flow, meaning:
- Graph API calls execute with the **authenticated user's permissions**
- Users must have appropriate SharePoint access rights
- Audit logs show the actual user, not the service principal
- The service will fall back to app-only authentication if no user token is provided

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

Lists all items (folders and documents) in a specified folder, recursively displaying the entire folder tree in a hierarchical text format similar to the Unix `tree` command.

**Parameters:**
- `siteId` - SharePoint site ID
- `path` (optional) - Folder path (empty for root, or path like "/Documents/Folder1")

**Response Format:** `text/plain`

The response is a hierarchical tree structure with the following format:
- **Folders**: `Name/ [d:folder_id] X items MM-DD HH:MM`
- **Files**: `Name [f:file_id] size ext MM-DD HH:MM`
- **Indentation**: 2 spaces per level
- **File sizes**: Human-readable format (B, K, M, G)
- **Dates**: MM-DD HH:MM format showing last modified time

**Response Example:**
```
/ [d:root] 3 items
  Marketing/ [d:folder_001] 2 items 09-28 16:45
    Brand Guidelines 2025.pdf [f:doc_001] 4.6M pdf 03-12 10:20
    Campaigns/ [d:folder_002] 2 items 09-25 14:30
      Q3_Strategy.docx [f:doc_002] 281K docx 07-08 09:15
      email_templates.html [f:doc_003] 34K html 08-15 13:45
  Engineering/ [d:folder_003] 1 items 09-29 18:20
    architecture_diagram.png [f:doc_004] 764K png 09-15 14:30
  Finance/ [d:folder_004] 0 items 09-27 10:30
```

**Usage Example:**
- List root tree: `/mcp/sharepoint/sites/{siteId}/items`
- List subfolder tree: `/mcp/sharepoint/sites/{siteId}/items?path=/Documents/Projects`

**Notes:**
- The tree is built recursively, showing all nested folders and files
- Unique IDs (`folder_XXX`, `doc_XXX`) are assigned sequentially for reference
- File extensions are extracted from file names
- Empty folders show "0 items"

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

### On-Behalf-Of (OBO) Flow

The API implements OAuth 2.0 On-Behalf-Of authentication:

1. **User authenticates** to the API with Azure AD JWT token
2. **API extracts** the user's bearer token from the Authorization header
3. **Token flows** through the application layers:
   - Controller → MediatR Query → Handler → Infrastructure Service
4. **Service exchanges** user token for Graph API token using OBO flow
5. **Graph API calls** execute with user's delegated permissions

**Benefits:**
- User-level SharePoint permissions are respected
- Audit trails show actual user activity
- Enhanced security through least-privilege access
- No need for elevated app-only permissions

## Error Responses

- `401 Unauthorized` - Missing or invalid authentication token
- `404 Not Found` - Site or item not found
- `400 Bad Request` - Invalid parameters or query

## Architecture

The implementation follows Clean Architecture principles with proper separation of concerns:

- **Domain Layer**: SharePoint entities (Site, Folder, Document, DriveItem)
- **Infrastructure Layer**: Microsoft Graph service implementation with OBO credential management
- **Application Layer**: CQRS queries (with UserAccessToken property) and MediatR handlers
- **API Layer**: MCP-compatible REST controller with token extraction

**Token Flow (maintaining Clean Architecture):**
1. Controller extracts bearer token from HTTP Authorization header
2. Token passed to MediatR query as property
3. Handler forwards token to infrastructure service method
4. Service creates GraphServiceClient with OnBehalfOfCredential per-request
5. No HTTP dependencies leak into lower layers

## Testing

Unit tests are provided in `Pixelbadger.Api.Application.Tests/SharePoint/` covering:
- Site info retrieval
- Folder listing
- Document search
- Metadata retrieval