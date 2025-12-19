# Whispra

Whispra is a cross-platform social application (mobile + desktop) that blends Instagram-style posting (text, photos, videos, comments, reactions) with Facebook-like communities (public/private groups with their own feed and group chat), plus real-time private messaging.

The project is designed as a production-ready learning journey: from architecture and domain modeling to implementation, testing, and deployment.

## Core Features

- User accounts and profiles
- Public and private posts (text/photo/video)
- Comments and reactions
- Communities (public/private groups)
  - Community feed (group posts)
  - Community group chat (real-time)
- Private messaging (DMs) with real-time delivery
- Media upload via object storage (images/videos)
- Moderation basics (reporting, blocking, rate limiting) _(planned)_

## Tech Stack

### Frontend

- Mobile: React Native (Expo) + TypeScript
- Desktop: React + Vite + TypeScript + Tauri
- Web (optional): Next.js + TypeScript

### Backend

- ASP.NET Core (C#) Web API
- SignalR for real-time messaging and presence
- MongoDB for primary data storage
- Redis for caching/presence/backplane (recommended for scale)
- S3-compatible object storage for media

## Architecture Overview

Whispra uses a **modular monolith** backend architecture: a single deployable service split into clear modules that can evolve independently.

Backend modules:

- Identity (auth, sessions, tokens)
- Social Graph (follow, block, privacy)
- Communities (groups, membership, roles, invites)
- Feed & Posts (posts, comments, reactions, visibility rules)
- Messaging (DM + group conversations, message history, receipts)
- Notifications (in-app + push)
- Moderation (reports, audit logs, anti-spam)

### REST + Real-time

- REST APIs handle stateful operations (CRUD, feeds, history, settings)
- SignalR handles real-time events (new messages, typing, presence, delivery/read receipts)

## Repository Structure (Monorepo)

```csharp
whispra/
  apps/
    mobile/             # Expo React Native
    desktop/            # Tauri + React
    web/                # Next.js (optional)
  packages/
    ui/                 # design system shared
    shared/             # types, validation schemas, utilities
  backend/
    Whispra.Api/                # ASP.NET Core API (controllers + SignalR hubs)
    Whispra.Application/        # use-cases (business logic)
    Whispra.Domain/             # entities + rules (no DB here)
    Whispra.Infrastructure/     # MongoDB, Redis, Storage, external services
  docker-compose.yml
  README.md
```

## Development Roadmap (order from scratch)

1. **Repository setup**

   - Create the monorepo structure
   - Add basic CI (lint + build + tests)
   - Add `docker-compose` for local infrastructure (**MongoDB**, **Redis**)

2. **Identity**

   - User registration and login
   - JWT authentication
   - Refresh tokens (with rotation)

3. **Communities**

   - Create communities (public/private)
   - Join/leave communities
   - Roles and permissions (owner/moderator/member)
   - Invites (links/codes, optional approval)

4. **Feed & Posts**

   - Create/read/update/delete posts
   - Comments
   - Reactions
   - Visibility rules (public, followers, community-only, etc.)

5. **Media Upload**

   - Object storage integration (S3-compatible)
   - Media metadata in MongoDB
   - Basic processing/validation (size/type limits)

6. **Messaging**

   - REST API for message history and conversation lists
   - Real-time messaging via **SignalR**
   - Typing indicators, presence (optional early), delivery/read receipts

7. **Notifications**

   - In-app notifications
   - Push notifications (later, once core flows are stable)

8. **Moderation & Safety**

   - Reporting
   - Blocking
   - Rate limiting / anti-spam
   - Audit logs

9. **Testing & Security Hardening**
   - Unit tests + integration tests
   - End-to-end tests
   - Security review and improvements (permissions, validation, logging hygiene)

## Roadmap (feature checklist)

- `[ ]` **Authentication** (JWT + refresh token rotation)
- `[ ]` **Communities** (public/private, roles, invites)
- `[ ]` **Feed and Posts** (comments, reactions, visibility rules)
- `[ ]` **Media Uploads** (object storage + metadata)
- `[ ]` **Messaging** (REST history + SignalR real-time)
- `[ ]` **Notifications** (in-app, then push)
- `[ ]` **Moderation** (reporting, blocking, rate limiting, audit logs)
- `[ ]` **Testing** (unit + integration + e2e) and **CI/CD** pipeline

## Security & Privacy Notes

Whispra aims for privacy-conscious defaults and strong permission checks.  
Planned improvements include rate limiting, audit logs, and more granular visibility controls.  
End-to-end encryption (E2EE) is intentionally postponed until the core UX and key management design are validated.

## Contributing

This project is currently in active development. Contributions, suggestions, and issue reports are welcome.

## Browser & Runtime Compatibility

Whispra is designed to behave consistently across environments by explicitly targeting stable web standards and validating UI behavior through automated cross-engine testing.

### Desktop (Tauri) Runtime Notes

The desktop app does **not** run in the user’s installed browser. Instead, it runs inside the operating system’s **WebView**:

- **Windows:** WebView2 (Edge/Chromium runtime)
- **macOS:** WebKit
- **Linux:** WebKitGTK

Because of this, user browser differences are mostly irrelevant for the desktop build. To keep the UI reliable, the project avoids experimental browser APIs and targets modern, well-supported web features.

### Web Browser Support Policy (if/when the Web client is added)

If a Web client is introduced, Whispra will explicitly support:

- The **latest two versions** of **Chrome/Edge**, **Firefox**, and **Safari**
- Mobile browsers equivalent to **Safari iOS** and **Chrome Android** (where applicable)

### Compatibility Choices

- **Explicit target browsers:** we define a `browserslist` policy so the build output is generated with clear compatibility targets.
- **Conservative API usage:** we prefer stable, widely supported web APIs and avoid experimental features unless a fallback exists.
- **Cross-engine automated testing:** we use **Playwright** to run end-to-end tests on **Chromium**, **Firefox**, and **WebKit** in CI to catch engine-specific issues early.
- **Polyfills only when necessary:** if a required feature is missing on a supported browser, we add targeted polyfills rather than relying on browser-specific behavior.
- **Continuous validation:** compatibility checks are part of the CI pipeline so regressions are caught during pull requests, not after release.

## Getting Started (Local Development)

### Prerequisites

- Node.js (LTS)
- .NET SDK (latest stable)
- Docker Desktop (or Docker Engine)
- MongoDB + Redis (via docker-compose)

### 1) Start infrastructure

```bash
docker compose up -d
```

### 2) Run the backend

```bash
cd backend/Whispra.Api
dotnet restore
dotnet run
```

### 3) Run the mobile app (Expo)

```bash
cd apps/mobile
npm install
npx expo start
```

### 4) Run the desktop app (Tauri)

```bash
cd apps/desktop
npm install
npm run tauri dev
```
