# GitHub Actions Quick Start Guide

## Overview

PCL Community Edition now has a complete GitHub Actions CI/CD pipeline that automatically builds, tests, and packages the application.

## Available Workflows

### 1. CI - Continuous Integration (`ci.yml`)
**Triggers**: Push to main/master/dev/develop, Pull Requests  
**Features**:
- Builds Debug and Release configurations
- Supports x64 and ARM64 architectures
- Runs unit tests
- Uploads build artifacts (30-day retention)

### 2. Quick Build (`quick-build.yml`)
**Triggers**: Any branch push, Any PR  
**Features**:
- Fast x64 Debug build
- Basic testing
- Quick feedback in minutes

### 3. PR Checks (`pr-check.yml`)
**Triggers**: Pull Request events  
**Features**:
- Smart file change detection
- Code statistics
- TODO/FIXME scanner
- Test reporting
- Automatic PR summary

### 4. Nightly Build (`nightly-build.yml`)
**Triggers**: Daily at 02:00 UTC (10:00 Beijing time)  
**Features**:
- Release builds for x64 and ARM64
- Full test suite
- Versioned ZIP packages (7-day retention)

## Quick Actions

### Download Build Artifacts
1. Go to the **Actions** tab
2. Select a workflow run
3. Scroll to **Artifacts** section
4. Download the desired artifact

### Manual Workflow Trigger
1. Navigate to **Actions** tab
2. Select a workflow from the left sidebar
3. Click **Run workflow** button
4. Choose branch and click **Run workflow**

### View Build Status
- Check the README badges
- Visit the Actions tab for detailed logs
- PR page shows check status

## Build Matrix

| Workflow | Config | Architecture | Tests | Artifacts |
|----------|--------|--------------|-------|-----------|
| Quick Build | Debug | x64 | ✅ | ❌ |
| PR Check | Debug | x64 | ✅ | Reports |
| CI | Debug, Release | x64, ARM64 | ✅ | ✅ |
| Nightly | Release | x64, ARM64 | ✅ | ZIP |

## Required Configuration

### GitHub Secrets
Configure these in Settings → Secrets and variables → Actions:

- `CLIENT_ID` - Microsoft client ID
- `CURSEFORGE_API_KEY` - CurseForge API key
- `TELEMETRY_KEY` - Telemetry key
- `NAID_CLIENT_ID` - NAID client ID
- `NAID_CLIENT_SECRET` - NAID client secret
- `LINK_SERVER_ROOTS` - Link server roots
- `LOBBY_DEFAULT_SECRET` - Lobby default secret

## Troubleshooting

### Build Fails
1. Check the build logs for specific errors
2. Verify submodules are correctly configured
3. Ensure all required secrets are set

### Tests Fail
1. Review test reports in artifacts
2. Run tests locally: `dotnet test PCL.Test/PCL.Test.csproj`
3. Check for environment-specific issues

### No Artifacts
1. Confirm the workflow completed successfully
2. Check artifact retention period (30 days for CI, 7 days for nightly)
3. Verify upload steps in workflow logs

## Development Workflow

```
Feature Branch → Quick Build (validation)
      ↓
Pull Request → PR Checks (detailed analysis)
      ↓
Merge to Main → Full CI (comprehensive build)
      ↓
Daily Schedule → Nightly Build (latest stable)
```

## Learn More

- Detailed documentation: `README.md` (Chinese)
- Complete setup guide: `../GITHUB_ACTIONS_SETUP.md`
- GitHub Actions docs: https://docs.github.com/actions

## Support

For issues or suggestions:
1. Check existing documentation
2. Create an issue in the repository
3. Contact project maintainers

---

**Last Updated**: 2025-01-30  
**Maintainer**: PCL Community
