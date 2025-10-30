# GitHub Actions 自动构建配置变更日志

## 变更概述

为 PCL Community Edition 项目添加了完整的 GitHub Actions 自动构建系统。

## 新增文件列表

### GitHub Actions 工作流文件

1. **`.github/workflows/ci.yml`**
   - 完整的持续集成流程
   - 支持多配置构建（Debug/Release）
   - 支持多架构构建（x64/ARM64）
   - 包含单元测试和代码质量检查
   - 运行器：Windows (windows-latest)
   - 触发条件：推送到主分支或 PR

2. **`.github/workflows/quick-build.yml`**
   - 快速构建验证
   - 仅构建 x64 Debug 版本
   - 适合开发阶段的快速反馈
   - 运行器：Windows (windows-latest)
   - 触发条件：任何分支推送或 PR

3. **`.github/workflows/pr-check.yml`**
   - Pull Request 自动检查
   - 智能文件变更检测
   - 代码统计和分析
   - TODO/FIXME 扫描
   - 运行器：Windows 和 Ubuntu
   - 触发条件：PR 创建、更新或重新打开

4. **`.github/workflows/nightly-build.yml`**
   - 每日定时构建
   - 生成 Release 版本的 ZIP 包
   - 运行完整测试套件
   - 运行器：Windows (windows-latest)
   - 触发条件：每天 UTC 02:00（北京时间 10:00）或手动触发

### 文档文件

5. **`.github/workflows/README.md`**
   - 详细的工作流说明文档
   - 包含所有工作流的功能描述
   - 故障排除指南
   - 维护建议

6. **`.github/workflows/使用说明.md`**
   - 中文用户快速指南
   - 包含使用步骤和常见问题
   - 适合中文用户阅读

7. **`.github/workflows/QUICKSTART.md`**
   - 英文快速入门指南
   - 简洁明了的使用说明
   - 适合国际用户

8. **`GITHUB_ACTIONS_SETUP.md`**
   - 完整的设置和配置文档
   - 包含架构图和技术细节
   - 性能优化建议
   - 迁移和维护说明

## 功能特性

### 自动化流程

✅ **持续集成**
- 代码推送后自动构建
- 多配置、多架构并行构建
- 自动运行单元测试
- 代码质量检查

✅ **Pull Request 检查**
- 自动验证 PR 代码
- 生成测试报告
- 代码统计分析
- 显示检查摘要

✅ **每日构建**
- 定时生成最新构建
- 完整测试验证
- 自动打包发布

✅ **快速反馈**
- 几分钟内完成基础验证
- 适合开发迭代

### 构建矩阵

| 配置 | 架构 | 测试 | 产物 |
|------|------|------|------|
| Debug | x64 | ✅ | ✅ |
| Debug | ARM64 | ✅ | ✅ |
| Release | x64 | ✅ | ✅ |
| Release | ARM64 | ✅ | ✅ |

### 产物管理

- **CI 构建产物**：保留 30 天
- **每日构建产物**：保留 7 天
- **测试结果**：TRX 格式报告
- **代码覆盖率**：XPlat Code Coverage

## 技术规格

### 运行环境

- **主构建**：Windows runners (windows-latest)
- **代码分析**：Ubuntu runners (ubuntu-latest)
- **.NET SDK**：自动安装 8.0.x 和 9.0.x

### 依赖项

- Git 子模块：自动递归检出
- NuGet 包：自动恢复
- 项目引用：自动处理

### 环境变量

构建需要以下 GitHub Secrets：
- `CLIENT_ID`
- `CURSEFORGE_API_KEY`
- `TELEMETRY_KEY`
- `NAID_CLIENT_ID`
- `NAID_CLIENT_SECRET`
- `LINK_SERVER_ROOTS`
- `LOBBY_DEFAULT_SECRET`

## 使用说明

### 开发者工作流

1. **日常开发**
   ```bash
   git checkout -b feature/my-feature
   # 进行开发
   git push origin feature/my-feature
   # → 触发 quick-build.yml（快速验证）
   ```

2. **创建 Pull Request**
   ```bash
   # 在 GitHub 上创建 PR
   # → 触发 pr-check.yml（详细检查）
   ```

3. **合并到主分支**
   ```bash
   # 合并 PR
   # → 触发 ci.yml（完整构建）
   ```

4. **获取每日构建**
   - 每天自动运行
   - 在 Actions 页面下载产物

### 手动触发

所有工作流都支持手动触发：
1. 进入 GitHub Actions 页面
2. 选择工作流
3. 点击 "Run workflow"
4. 选择分支并运行

## 兼容性

### 与现有工作流的关系

新工作流**不会替代**现有的工作流，而是作为补充和增强：

- `build-test.yml`：继续用于 dev 分支的 CI 构建
- `reusable-build.yml`：作为可重用组件继续被调用
- `release-*.yml`：发布流程保持不变
- `mirrorchyan_*.yml`：镜像功能保持不变

### 向后兼容

- 所有现有的构建流程保持不变
- 新工作流使用相同的构建命令和配置
- 不影响现有的发布流程

## 性能优化

### 并行构建

- 使用矩阵策略并行构建多个配置
- `fail-fast: false` 确保所有配置都能完成

### 智能触发

- PR 检查仅在相关文件变更时运行
- 路径过滤减少不必要的构建

### 缓存策略

- NuGet 包自动缓存
- 加速依赖恢复过程

## 故障排除

### 常见问题

**Q: 构建失败显示 "Missing secrets"**  
A: 在仓库设置中配置所有必需的 Secrets。

**Q: 测试失败**  
A: 查看测试报告，在本地运行 `dotnet test` 进行调试。

**Q: ARM64 构建失败**  
A: 确认项目配置支持 ARM64，检查是否有平台特定的依赖。

**Q: 找不到产物**  
A: 检查构建是否成功，注意产物保留期限。

### 调试技巧

1. 查看完整的构建日志
2. 使用 `workflow_dispatch` 手动触发测试
3. 在本地使用相同的命令进行验证
4. 检查环境变量和 Secrets 配置

## 维护指南

### 定期检查

- ✅ 每周查看 Actions 运行状态
- ✅ 及时修复失败的构建
- ✅ 更新过期的依赖和 Actions

### 版本更新

更新 .NET 版本时：
1. 搜索所有工作流中的 `dotnet-version`
2. 同步更新到新版本
3. 测试构建确保兼容

### Secrets 管理

- 定期轮换敏感密钥
- 使用最小权限原则
- 记录密钥用途和有效期

## 未来计划

### 可能的改进

- [ ] 添加代码覆盖率报告
- [ ] 集成静态代码分析工具
- [ ] 添加性能基准测试
- [ ] 实现自动版本号管理
- [ ] 添加发布说明自动生成

### 扩展功能

- [ ] Docker 容器化构建
- [ ] 多语言构建产物
- [ ] 自动化部署到测试环境
- [ ] 集成更多的代码质量工具

## 贡献

欢迎贡献改进！提交 PR 时请：
1. 说明修改原因和目的
2. 测试工作流的有效性
3. 更新相关文档
4. 遵循现有的代码风格

## 参考资料

- [GitHub Actions 官方文档](https://docs.github.com/actions)
- [.NET CLI 参考](https://docs.microsoft.com/dotnet/core/tools/)
- [GitHub Actions 最佳实践](https://docs.github.com/actions/learn-github-actions/best-practices-for-github-actions)

## 版本信息

- **创建日期**：2025-01-30
- **版本**：1.0.0
- **作者**：PCL Community
- **许可**：与项目主许可一致

## 致谢

感谢所有为 PCL Community Edition 项目做出贡献的开发者！

---

**注意**：本次更改仅添加新文件，不修改任何现有代码或配置。所有新工作流都经过测试，可以安全使用。
