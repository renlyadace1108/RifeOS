# ✦ RifeOS (Personal Growth Operating System)

> 基于 **.NET 8 / C# 12 / WPF** 开发的轻量级、模块化、插件隔离型桌面个人操作系统框架。

---

## 🏛 核心架构设计

RifeOS 采用 **微内核 (Microkernel) + 动态沙箱插件应用 (Apps)** 的架构体系：
- **RifeOS.SDK**：零 UI 依赖纯净契约层，定义生命周期状态机、沙箱存储协议与事件总线。
- **RifeOS.Host**：基于 `AssemblyLoadContext` 的极低底噪宿主，实现插件按需动态隔离加载与物理深度回收（WorkingSet 修剪）。
- **RifeOS.Apps.Tasks**：独立 EF Core + SQLite 沙箱数据隔离的任务清单应用。
- **RifeOS.Apps.Settings**：解耦独立的系统级配置应用，支持黑灰/白灰主题动态切换与用户凭证管理。

---

## 🛠 开发与运行环境

- **操作系统**：Windows 10 / 11 (x64 / ARM64)
- **开发工具**：Visual Studio 2022 / 2026（需安装 `.NET 桌面开发` 工作负载）
- **运行时目标**：.NET 8.0-windows

---

## 🚀 编译与调试指南

1. 使用 Visual Studio 打开 `RifeOS.slnx`（或 `RifeOS.sln`）。
2. 在解决方案资源管理器中，右键 `RifeOS.Host` $\rightarrow$ 选择 **设为启动项目**。
3. 按 **`F5`** 启动调试，或按 **`Ctrl + F5`** 极速运行。
4. 首次启动将自动引导进入 **OOBE 系统初始化向导**。
