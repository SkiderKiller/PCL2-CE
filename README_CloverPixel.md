# CloverPixel Launcher

CloverPixel Launcher 是一个专为 Minecraft 1.8.9 定制的启动器，基于 PCL Community Edition 修改而来。

## 功能特性

### 自动安装
启动器会在首次运行时自动检测并安装以下组件：
- **Java 8**: 如果系统未安装 Java 8，将自动从 OpenLogic 下载并安装
- **Minecraft 1.8.9**: 自动下载并安装 Minecraft 1.8.9
- **Forge**: 为 1.8.9 版本安装 Forge mod 加载器

### CloverPixel Mod 自动加载
- 启动器会自动加载 `cloverpixel-2.0.0.jar` mod
- 该 mod 在文件系统中被设置为隐藏文件（只读+系统+隐藏属性）
- 在启动器的 mod 管理界面中不可见
- 用户添加的其他 mod 正常显示和管理

### 兼容性
- 完全兼容 PCL CE 的所有功能
- 支持用户自行添加和管理其他 mod
- 保持原有的启动选项和配置功能

## 技术说明

### CloverPixel Mod 位置
- 源文件：项目根目录下的 `cloverpixel-2.0.0.jar`
- 部署位置：游戏实例的 `mods` 目录
- 文件属性：隐藏、系统、只读

### 自动安装流程
1. 检查 Java 8 是否存在
   - 如果不存在，从 https://builds.openlogic.com 下载
   - 解压到 `.minecraft\runtime\clover-java-8\` 目录
2. 检查 Minecraft 1.8.9 是否安装
   - 使用内置的下载系统自动下载
3. 检查 Forge 是否安装
   - 如果未安装，提示用户在下载页面手动安装

### 游戏启动流程
1. 准备阶段（McLaunchPrerun）
   - 复制 `cloverpixel-2.0.0.jar` 到 mods 目录
   - 设置文件为隐藏属性
2. 正常启动游戏
3. CloverPixel mod 随游戏一起加载

## 构建说明

项目基于 .NET 8 和 WPF，需要以下环境：
- .NET 8 SDK
- Windows 10 或更高版本
- Visual Studio 2022 或 JetBrains Rider

构建命令：
```bash
dotnet build "Plain Craft Launcher 2/Plain Craft Launcher 2.vbproj" --configuration Release
```

## 文件结构
```
Plain Craft Launcher 2/
├── Modules/
│   └── ModCloverPixel.vb      # CloverPixel 功能模块
├── FormMain.xaml.vb            # 启动器主窗口（包含初始化调用）
└── ...

cloverpixel-2.0.0.jar           # CloverPixel Mod 文件（项目根目录）
```

## 授权和致谢

本启动器基于 PCL Community Edition 修改，感谢 PCL CE 团队和原作者龙腾猫跃的贡献。

相关链接：
- PCL CE: https://github.com/PCL-Community/PCL2-CE
- PCL 官方: https://github.com/Hex-Dragon/PCL2
