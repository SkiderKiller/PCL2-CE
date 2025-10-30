# GitHub Actions 自动构建配置说明

## 概述

已为 PCL Community Edition 项目配置完整的 GitHub Actions 自动构建系统。该系统提供持续集成、自动化测试、每日构建和 Pull Request 检查等功能。

## 新增的工作流程文件

### 1. `.github/workflows/ci.yml` - 完整的持续集成流程

**主要功能：**
- ✅ 多配置构建（Debug 和 Release）
- ✅ 多架构支持（x64 和 ARM64）
- ✅ 自动运行单元测试
- ✅ 发布应用程序
- ✅ 代码质量检查
- ✅ 构建产物上传（保留 30 天）

**触发条件：**
- 推送到 main/master/dev/develop 分支
- 针对这些分支的 Pull Request
- 手动触发

**运行环境：** Windows 运行器（windows-latest）

### 2. `.github/workflows/quick-build.yml` - 快速构建

**主要功能：**
- ⚡ 快速反馈（仅构建 x64 Debug 版本）
- ✅ 运行基本测试
- 🚀 适用于开发阶段的快速验证

**触发条件：**
- 任何分支的推送
- 任何 Pull Request
- 手动触发

**运行环境：** Windows 运行器（windows-latest）

### 3. `.github/workflows/pr-check.yml` - Pull Request 检查

**主要功能：**
- 🔍 智能检测变更文件
- ✅ 仅在相关文件变更时构建
- 📊 生成测试报告
- 📈 代码统计分析
- 🔎 检查 TODO 和 FIXME 项
- 📝 自动生成 PR 摘要

**触发条件：**
- Pull Request 打开、同步或重新打开

**运行环境：** Windows 和 Ubuntu 运行器

### 4. `.github/workflows/nightly-build.yml` - 每日构建

**主要功能：**
- 🌙 定时构建（每天北京时间 10:00）
- 📦 生成带版本号的发行包
- ✅ 运行完整测试套件
- 💾 保留最近 7 天的构建

**触发条件：**
- 定时触发（每天 UTC 02:00）
- 手动触发

**运行环境：** Windows 运行器（windows-latest）

## 工作流程架构图

```
代码推送/PR
    ↓
┌─────────────────────────────────────┐
│  quick-build.yml (快速验证)          │
│  - 快速构建 x64 Debug                │
│  - 基本测试                          │
└─────────────────────────────────────┘
    ↓
┌─────────────────────────────────────┐
│  pr-check.yml (PR 详细检查)          │
│  - 变更文件检测                      │
│  - 代码统计                          │
│  - TODO/FIXME 检查                   │
└─────────────────────────────────────┘
    ↓
┌─────────────────────────────────────┐
│  ci.yml (完整 CI)                    │
│  - 多配置构建 (Debug/Release)        │
│  - 多架构构建 (x64/ARM64)            │
│  - 完整测试套件                      │
│  - 代码质量检查                      │
│  - 产物上传                          │
└─────────────────────────────────────┘

定时触发 (每日)
    ↓
┌─────────────────────────────────────┐
│  nightly-build.yml (每日构建)        │
│  - Release 版本构建                  │
│  - 完整测试                          │
│  - 生成发行包                        │
└─────────────────────────────────────┘
```

## 构建矩阵

| 工作流 | 配置 | 架构 | 测试 | 产物 |
|--------|------|------|------|------|
| quick-build | Debug | x64 | ✅ | ❌ |
| pr-check | Debug | x64 | ✅ | 报告 |
| ci | Debug, Release | x64, ARM64 | ✅ | ✅ |
| nightly-build | Release | x64, ARM64 | ✅ | ZIP |

## 环境要求

### 必需的 Secrets

在 GitHub 仓库设置中配置以下密钥：

```
CLIENT_ID                 # Microsoft 客户端 ID
CURSEFORGE_API_KEY        # CurseForge API 密钥
TELEMETRY_KEY             # 遥测密钥
NAID_CLIENT_ID            # NAID 客户端 ID
NAID_CLIENT_SECRET        # NAID 客户端密钥
LINK_SERVER_ROOTS         # 链接服务器根地址
LOBBY_DEFAULT_SECRET      # 大厅默认密钥
```

### 运行器要求

- **Windows 构建**：需要 windows-latest 运行器
- **.NET SDK**：自动安装 .NET 8.0 和 9.0
- **子模块**：自动递归检出 PCL.Core

## 构建产物

### CI 构建产物

```
PCL2-CE-Debug-x64/
PCL2-CE-Debug-ARM64/
PCL2-CE-Release-x64/
PCL2-CE-Release-ARM64/
```

### 每日构建产物

```
PCL2-CE-Nightly-YYYY.MM.DD-commit-x64.zip
PCL2-CE-Nightly-YYYY.MM.DD-commit-ARM64.zip
```

### 测试结果

```
test-results-Debug-x64/
test-results-Release-x64/
nightly-test-results/
```

## 使用指南

### 日常开发

1. **功能分支开发**：推送代码时会触发 `quick-build.yml` 快速验证
2. **创建 PR**：自动运行 `pr-check.yml` 进行详细检查
3. **合并到主分支**：触发完整的 `ci.yml` 构建流程

### 下载构建产物

1. 访问 GitHub Actions 页面
2. 选择对应的工作流运行
3. 在"Artifacts"部分下载产物

### 手动触发构建

所有工作流都支持手动触发：

1. 进入 Actions 页面
2. 选择要运行的工作流
3. 点击 "Run workflow" 按钮

### 查看测试报告

测试结果以两种形式提供：

1. **TRX 文件**：在产物中下载
2. **工作流摘要**：在 GitHub Actions 运行页面查看

## 故障排除

### 构建失败

**问题：** 构建失败并显示依赖错误

**解决方案：**
1. 检查子模块是否正确配置
2. 确认 `Plain Craft Launcher 2.slnx` 文件正确
3. 验证项目引用路径

### 测试失败

**问题：** 单元测试失败

**解决方案：**
1. 在本地运行测试：`dotnet test PCL.Test/PCL.Test.csproj`
2. 查看详细的测试报告
3. 检查测试环境配置

### Secrets 配置

**问题：** 缺少必需的 Secrets

**解决方案：**
1. 进入仓库设置 → Secrets and variables → Actions
2. 添加所有必需的密钥
3. 重新运行失败的工作流

### ARM64 构建问题

**问题：** ARM64 架构构建失败

**解决方案：**
1. 确认项目配置支持 ARM64 平台
2. 检查是否有 x64 特定的依赖
3. 考虑使用交叉编译

## 性能优化

### 缓存策略

工作流已配置依赖缓存：
- NuGet 包自动缓存
- 构建输出不缓存（避免过期问题）

### 并行构建

- CI 工作流使用矩阵策略并行构建多个配置
- 失败不影响其他配置的构建（fail-fast: false）

### 条件执行

- PR 检查只在相关文件变更时运行
- 测试仅在 x64 平台运行（避免重复）

## 维护建议

### 定期检查

- ✅ 每周查看每日构建状态
- ✅ 及时修复失败的测试
- ✅ 更新过期的依赖

### 版本更新

当更新 .NET 版本时，需要修改：
- `.github/workflows/ci.yml`
- `.github/workflows/quick-build.yml`
- `.github/workflows/pr-check.yml`
- `.github/workflows/nightly-build.yml`

搜索 `dotnet-version` 并更新版本号。

### Secrets 轮换

定期更新敏感的 Secrets：
1. 生成新的密钥
2. 在 GitHub 中更新
3. 验证构建仍然正常

## 迁移说明

### 从原有工作流迁移

原有的工作流文件保持不变：
- `build-test.yml` - 继续用于 dev 分支
- `reusable-build.yml` - 被其他工作流调用
- `release-*.yml` - 发布流程不变

新工作流是补充和增强，不会冲突。

### 禁用某个工作流

如果需要临时禁用某个工作流：

1. 编辑工作流文件
2. 在 `on:` 下面添加：
   ```yaml
   on:
     workflow_dispatch:  # 仅保留手动触发
   ```

或者直接删除/重命名文件。

## 相关文档

- [GitHub Actions 文档](https://docs.github.com/actions)
- [.NET CLI 文档](https://docs.microsoft.com/dotnet/core/tools/)
- [MSTest 文档](https://docs.microsoft.com/visualstudio/test/using-microsoft-visualstudio-testtools-unittesting-members-in-unit-tests)

## 支持与反馈

如有问题或建议，请：
1. 查看 `.github/workflows/README.md` 详细文档
2. 提交 Issue 到项目仓库
3. 联系项目维护者

---

**创建日期**：2025-01-30  
**版本**：1.0  
**维护者**：PCL Community
