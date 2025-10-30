# CloverPixel Launcher 修改说明

本文档列出了从 PCL Community Edition 到 CloverPixel Launcher 的所有修改。

## 主要功能变更

### 1. 品牌更新
- 启动器名称从 "Plain Craft Launcher Community Edition" 更改为 "CloverPixel Launcher"
- 所有用户界面文本已更新以反映新品牌
- 项目元数据（AssemblyTitle, Product, Copyright）已更新

### 2. 自动安装系统
实现了启动时的自动组件检测和安装：

#### Java 8 自动下载
- 检测系统是否已安装 Java 8
- 如果未安装，自动从 OpenLogic 下载
- URL: https://builds.openlogic.com/downloadJDK/openlogic-openjdk-jre/8u462-b08/openlogic-openjdk-jre-8u462-b08-windows-x64.zip
- 安装位置: `.minecraft\runtime\clover-java-8\`

#### Minecraft 1.8.9 自动下载
- 检测是否已安装 Minecraft 1.8.9
- 使用内置下载系统自动下载和安装

#### Forge 检测
- 检测是否为 1.8.9 安装了 Forge
- 如未安装，提示用户手动安装

### 3. CloverPixel Mod 隐藏加载
实现了透明的 mod 加载机制：

#### 自动复制
- 从项目根目录复制 `cloverpixel-2.0.0.jar` 到游戏 mods 目录
- 在每次启动前检查并确保文件存在

#### 文件隐藏
- 设置文件属性为：隐藏 + 系统 + 只读
- 创建 `.cloverpixel.hidden` 标记文件

#### 界面隐藏
- 在 mod 管理界面过滤掉 CloverPixel mod
- 用户添加的其他 mod 正常显示和管理

## 修改的文件列表

### 核心功能模块
1. **Plain Craft Launcher 2/Modules/ModCloverPixel.vb** (新增)
   - 包含所有 CloverPixel 特定功能
   - Java 8 下载和安装
   - MC 1.8.9 和 Forge 检测
   - 隐藏 mod 管理

### 启动和初始化
2. **Plain Craft Launcher 2/FormMain.xaml.vb**
   - 在启动流程中调用 `InitializeCloverPixel()`
   - 更新品牌文本
   - 更新 EULA 和遥测提示

3. **Plain Craft Launcher 2/FormMain.xaml**
   - 更新窗口标题为 "CloverPixel Launcher"

4. **Plain Craft Launcher 2/Program.vb**
   - 更新欢迎消息

### 启动流程集成
5. **Plain Craft Launcher 2/Modules/Minecraft/ModLaunch.vb**
   - 在 `McLaunchPrerun()` 中添加 CloverPixel mod 准备逻辑

### UI 过滤
6. **Plain Craft Launcher 2/Pages/PageInstance/PageInstanceCompResource.xaml.vb**
   - 在 mod 列表加载时过滤掉 CloverPixel mod

### 项目配置
7. **Plain Craft Launcher 2/Plain Craft Launcher 2.vbproj**
   - 更新 AssemblyTitle: "CloverPixel Launcher"
   - 更新 Product: "CloverPixel Launcher"
   - 更新 Copyright: "Copyright © CloverPixel 2024"

### 文档
8. **README_CloverPixel.md** (新增)
   - CloverPixel Launcher 功能说明
   - 技术文档

9. **CHANGES_CloverPixel.md** (本文件，新增)
   - 详细的修改列表

## 技术实现细节

### 初始化流程
```
启动器启动
  ↓
FormMain.New()
  ↓
FormMain_Loaded()
  ↓
InitializeCloverPixel() [在后台线程]
  ├─ CheckJava8() → DownloadJava8() (如需要)
  ├─ CheckMinecraft189() → DownloadMinecraft189() (如需要)
  └─ CheckForge189() → InstallForge189() (如需要)
```

### 启动流程
```
用户点击启动
  ↓
McLaunchStart()
  ↓
McLaunchPrerun()
  ├─ PrepareCloverPixelMod()
  │   ├─ 复制 cloverpixel-2.0.0.jar
  │   ├─ 设置文件为隐藏属性
  │   └─ 创建标记文件
  ↓
正常启动游戏
  ↓
CloverPixel mod 随游戏加载（对用户透明）
```

### Mod 隐藏机制
1. **文件系统层面**：设置为隐藏+系统+只读文件
2. **UI 层面**：在 `LoadUIFromLoaderOutput()` 中过滤
3. **识别机制**：通过 `IsCloverPixelHiddenMod()` 函数识别

## 兼容性说明

### 保持兼容的功能
- 所有 PCL CE 原有功能
- 用户设置和配置
- 自定义启动参数
- 其他 mod 的管理
- 账户登录
- 下载源切换

### 新增的限制
- 优先使用 Java 8（如果可用）
- 默认推荐 Minecraft 1.8.9
- CloverPixel mod 无法通过 UI 禁用或删除（但可以手动在文件管理器中操作）

## 部署说明

### 开发环境
1. 确保 `cloverpixel-2.0.0.jar` 位于项目根目录
2. 构建项目
3. 运行启动器

### 发布版本
1. 将 `cloverpixel-2.0.0.jar` 与启动器可执行文件放在同一目录
2. 首次运行时会自动下载必要组件
3. 后续启动会自动加载 CloverPixel mod

## 注意事项

1. **Java 8 下载**：需要网络连接，下载大小约 40-50 MB
2. **Forge 安装**：目前需要用户手动在下载页面安装
3. **文件位置**：CloverPixel mod 文件必须可访问（项目根目录或启动器目录）
4. **权限**：设置文件属性可能需要适当的文件系统权限

## 未来可能的改进

1. 自动化 Forge 安装流程
2. 支持更多 Minecraft 版本
3. 提供 CloverPixel mod 的配置界面
4. 添加更新检查机制
5. 本地化支持

## 致谢

本项目基于以下开源项目：
- PCL Community Edition (PCL2-CE)
- Plain Craft Launcher 2 (PCL2) by 龙腾猫跃
