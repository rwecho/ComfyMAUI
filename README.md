# ComfyMAUI

ComfyMAUI 是一个基于 .NET MAUI 开发的 ComfyUI 桌面客户端应用程序。它提供了一个友好的图形界面来管理和使用 ComfyUI。

## 功能特点

- 🖥️ 跨平台支持 
  - ✅ Windows
  - 🚧 macOS (开发中)
- 🎨 集成 ComfyUI 工作流管理
- 🚀 内置 Aria2 下载管理
- 🔧 NVIDIA GPU 监控支持
- ⚙️ Python 环境集成
- 📦 Git 版本控制集成
- 🌐 智能网络配置
  - 自动配置国内镜像源
  - 优化模型下载速度
  - 支持配置代理

## 系统要求

### Windows
- Windows 10/11
- .NET 7.0 或更高版本
- Python 3.x
- ComfyUI 环境
- NVIDIA GPU (可选，用于GPU功能)

### macOS
- macOS 12.0 (Monterey) 或更高版本
- .NET 7.0 或更高版本
- Python 3.x
- ComfyUI 环境

## 安装说明

### Windows 安装
1. 从 Release 页面下载最新版本的 Windows 安装包
2. 运行安装程序
3. 按照向导完成安装

### macOS 安装 (开发中)
1. 从 Release 页面下载最新版本的 macOS 安装包
2. 将应用拖入 Applications 文件夹
3. 首次运行时需要在"系统偏好设置"中允许运行

## 配置说明

应用程序首次运行时，需要配置以下内容：

- ComfyUI 路径
- Python 环境
- Aria2 下载设置
- Git 仓库设置（可选）

### 网络配置

#### 镜像源设置
- 支持配置 PyPI 镜像源（默认使用国内镜像）
- 支持配置 Git 镜像源
- 支持配置模型下载镜像源

#### 下载加速
- Aria2 多线程下载
- 断点续传支持
- 自动重试机制

#### 代理设置
- 支持 HTTP/HTTPS 代理
- 支持 Socks5 代理
- 可针对不同服务配置独立代理

### 平台特定配置

#### Windows
- NVIDIA GPU 监控配置
- Windows 特定路径设置

#### macOS
- 权限设置
- macOS 特定路径设置

## 主要功能

- ComfyUI 工作流管理和执行
- 文件下载管理
  - 智能选择最快镜像源
  - 自动优化下载参数
  - 支持批量下载管理
- GPU 资源监控 (仅 Windows)
- 系统设置管理
- 工作流版本控制

## 已知问题

### Windows
- 暂无主要已知问题

### macOS
- GPU 监控功能暂不支持
- 部分功能可能需要适配

## 开发说明

本项目使用 .NET MAUI 框架开发，主要包含以下组件：

- Blazor 页面集成
- 多种后台服务
- 系统托管服务
- 资源管理

## 贡献指南

欢迎提交 Issue 和 Pull Request 来帮助改进项目。特别欢迎针对 macOS 平台的优化和改进建议。

## 许可证

[MIT License](LICENSE)

## 联系方式

如有问题或建议，请通过 Issue 系统与我们联系。 