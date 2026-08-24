# ✦ RifeOS（Personal Growth Operating System）

![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-12.0%20%7C%2013.0-239120?style=for-the-badge&logo=csharp&logoColor=white)
![WPF](https://img.shields.io/badge/UI-WPF%20%7C%20Modern%20Dark-0078D4?style=for-the-badge&logo=windows&logoColor=white)
![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%2F%2011-00A4EF?style=for-the-badge&logo=windows11&logoColor=white)
![Architecture](https://img.shields.io/badge/Architecture-Microkernel%20%2B%20ALC%20Sandbox-blueviolet?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

基于 **.NET 8 / C# / WPF** 打造的轻量级桌面工作台。  
RifeOS 采用 **微内核宿主 + 可回收 ALC 插件沙箱** 架构，让每个应用插件在隔离上下文中按需加载、按需释放，实现模块化扩展与低内存驻留并存。

---

## ✨ 项目定位

RifeOS 不是传统意义上的操作系统内核，而是面向个人效率场景的 **“桌面个人成长操作台”**：

- 提供统一桌面壳层（任务栏、开始菜单、主题系统）
- 通过插件机制承载业务应用（Tasks、Settings、后续 Notes 等）
- 在保证体验一致性的前提下，支持应用独立演进与热插拔式扩展

---

## 🌟 核心亮点

### 1) 🏛 微内核宿主（Microkernel Host）
宿主仅保留最小系统能力：

- Shell UI 与窗口管理
- 开始菜单 / 任务栏调度
- 插件生命周期管理
- 主题资源总线（DynamicResource）

业务功能全部下沉到插件层，降低耦合与系统噪音。

### 2) 🧩 可回收 ALC 插件沙箱
每个插件以独立 DLL 运行在 `Collectible AssemblyLoadContext` 中：

- 启动应用时动态加载
- 关闭应用后调用 `Cleanup()`
- 卸载 ALC + 触发 GC，回收托管与工作集内存

实现“用时加载，不用即卸载”的轻量运行模型。

### 3) 🪟 沉浸式桌面交互
- 无边框极简主窗口（隐形拖拽区）
- Win10 风格开始菜单（列表 + 磁贴）
- 动态任务栏 Tab（激活 / 最小化 / 彻底退出）
- 桌面默认纯净启动，无冗余遮挡

### 4) 🎨 全局主题秒级联动
内置 `Dark / OLED Midnight / Light` 三套主题，插件通过 `DynamicResource` 自动响应换肤：

- 背景层次
- 边框与文字
- 磁贴色板
- 子应用视图主题同步

### 5) 📦 契约驱动与数据隔离
`RifeOS.SDK` 提供统一契约（无 UI 强依赖）：

- `IRifeApp`
- `IRifeAppContext`
- `IAppStorage`
- `IAppConfiguration`
- `INotificationService`
- `IThemeContext`

每个插件拥有独立 `Data/{AppId}/` 沙箱目录，保证数据边界清晰。

---

## 🧱 架构概览

```text
RifeOS.Host (Microkernel Shell)
 ├─ UI Shell / Start Menu / Taskbar
 ├─ AppLifecycleManager
 ├─ PluginLoadContext (Collectible ALC)
 └─ Dynamic Theme Bus
        │
        ├── RifeOS.Apps.Tasks
        ├── RifeOS.Apps.Settings
        └── (Future Plugins...)
        
RifeOS.SDK (Contracts)
 ├─ App interfaces
 ├─ Context interfaces
 ├─ Service contracts
 └─ Common models/enums
```

---

## 🔄 插件生命周期

1. 用户从开始菜单启动插件  
2. 宿主创建独立 Collectible ALC  
3. 扫描并实例化 `IRifeApp` 实现  
4. 注入 `IRifeAppContext`（Storage/Config/Theme/Notification）  
5. 渲染 `CreateView()` 返回视图并接入任务栏  
6. 关闭时执行 `Cleanup()`  
7. 卸载 ALC + `GC.Collect()` 完成资源释放

---

## 📁 项目结构

```text
RifeOS/
├── src/
│   ├── RifeOS.SDK/            # 插件契约层（纯接口与模型）
│   ├── RifeOS.Host/           # 微内核宿主（Shell + Lifecycle + Services）
│   ├── RifeOS.Apps.Tasks/     # 任务清单插件
│   └── RifeOS.Apps.Settings/  # 系统设置插件
└── RifeOS.sln
```

---

## 🛠 技术栈

- **Runtime**: .NET 8/9
- **Language**: C# 12/13
- **UI**: WPF
- **Pattern**: MVVM
- **Plugin Isolation**: Collectible `AssemblyLoadContext`
- **Platform**: Windows 10 / 11

---

## 🚀 快速开始

### 1. 克隆项目

```bash
git clone https://github.com/renlyadace1108/RifeOS.git
cd RifeOS
```

### 2. 打开解决方案
使用 Visual Studio 打开 `RifeOS.sln`。

### 3. 设置启动项目
将 `RifeOS.Host` 设为启动项目。

### 4. 运行
按 `F5`（调试）或 `Ctrl + F5`（运行）。

---

## 🔌 插件扩展（开发者）

新增插件建议流程：

1. 在 `src/` 下创建类库（如 `RifeOS.Apps.Notes`）
2. 引用 `RifeOS.SDK`
3. 实现 `IRifeApp`（`Metadata/Initialize/CreateView/Cleanup`）
4. 使用 `IRifeAppContext` 访问存储、配置、通知、主题
5. 在 Host 的应用入口列表中注册菜单项

> 建议：插件 UI 全部使用 `DynamicResource`，避免硬编码颜色，确保主题实时联动。

---

## ⚠️ 开发约束与最佳实践

- **避免类型冲突**：`RifeOS.SDK` 必须作为共享依赖，禁止在插件中重复隔离加载 SDK 副本  
- **保证数据隔离**：统一使用 `IAppStorage` 或 `AppDataDirectory`，避免绝对路径直写  
- **ContextMenu 绑定注意**：WPF 右键菜单脱离主视觉树，命令绑定建议通过 `PlacementTarget.Tag` 穿透

---

## 🧭 未来规划（Roadmap）

- [ ] 插件市场与插件发现机制  
- [ ] 更丰富的生产力插件（Notes / Habits / Focus Timer）  
- [ ] 统一事件总线可视化  
- [ ] 启动性能与内存回收指标面板  
- [ ] 多语言与可访问性增强

---

## 📄 License

MIT License © Renly
