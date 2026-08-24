# ✦ RifeOS (Personal Growth Operating System)

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-12.0%20%7C%2013.0-239120?style=for-the-badge&logo=csharp&logoColor=white)
![WPF](https://img.shields.io/badge/UI-WPF%20%7C%20Modern%20Dark-0078D4?style=for-the-badge&logo=windows&logoColor=white)
![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%2F%2011-00A4EF?style=for-the-badge&logo=windows11&logoColor=white)
![Architecture](https://img.shields.io/badge/Architecture-Microkernel%20%2B%20ALC%20Sandbox-blueviolet?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**基于 .NET 8 / C# 12 / WPF 构建的轻量级、模块化、沙箱插件隔离型桌面个人操作系统工作台。**

</div>

---

## 🌟 核心特性与设计亮点

* 🏛 **微内核宿主架构 (Microkernel Core)**  
  宿主保持极低底噪，仅提供窗口管理、插件沙箱调度、系统总线与主题调色板分发。
* 🧩 **可回收 ALC 动态沙箱 (Collectible AssemblyLoadContext)**  
  每个应用插件作为独立的 DLL 存在。应用从开始菜单点击时动态加载，从任务栏关闭后立刻触发深度垃圾回收（GC & WorkingSet 释放），不留常驻内存负担。
* 🪟 **沉浸式 Windows 桌面交互体系**
  * **无边框极简视窗**：顶部隐形拖拽区域，支持全局顺畅拖动。
  * **纯净桌面开机体验**：默认仅呈现极简品牌水印，无冗余首屏遮挡。
  * **Windows 10 风格开始菜单**：
    * 左侧全量应用垂直列表；
    * 右侧彩色磁贴网格（Live Tiles）；
    * 支持列表项右键「固定到开始屏幕」、磁贴右键「取消固定」，且在右键菜单交互期间**保持展开不失焦**。
  * **动态多任务栏**：
    * 居中单字母 `R` 极简开始按钮，鼠标移入自动展开开始菜单；
    * 运行中的应用自动生成任务栏 Tab，支持左键快速激活/最小化回桌面，右键唤出快捷菜单（最小化 / 彻底退出并释放内存）。
* 🎨 **全系统多主题秒级无缝联动**  
  内置 `Dark (默认深色)`、`OLED Midnight (极黑)`、`Light (浅色)` 三套主题。窗口底色、任务栏、开始菜单、磁贴背景、图标颜色及所有运行中的子应用通过 `DynamicResource` 体系实现全局联动换肤。
* 📦 **契约驱动与数据沙箱隔离**  
  `RifeOS.SDK` 零 UI 强依赖。各插件独占 `Data/{AppId}/` 本地沙箱目录，具备独立存储与强类型 JSON 配置管理。

---

## 🏛 系统架构设计

┌────────────────────────────────────────────────────────────────────────┐
│                              RifeOS.Host                               │
│  ┌────────────────────┬────────────────────┬────────────────────────┐  │
│  │ Frameless Shell UI │ Taskbar & Win10    │ Dynamic Theme Engine   │  │
│  │ (Windowless Shell) │ Start Menu Manager │ (DynamicResource Bus)  │  │
│  ├────────────────────┴────────────────────┴────────────────────────┤  │
│  │                Kernel: AppLifecycleManager                       │  │
│  │       ┌────────────────────────────────────────────────┐         │  │
│  │       │   PluginLoadContext (isCollectible: true)      │         │  │
│  └───────┴───────┬────────────────────────────────┬───────┴─────────┘  │
│                  │ (Shared SDK Assembly)          │                    │
│                  ▼                                ▼                    │
│      ┌───────────────────────┐        ┌───────────────────────┐        │
│      │   RifeOS.Apps.Tasks   │        │  RifeOS.Apps.Settings │        │
│      │   (Independent App)   │        │   (Independent App)   │        │
│      └───────────┬───────────┘        └───────────┬───────────┘        │
│                  │                                │                    │
│                  └────────────────┬───────────────┘                    │
│                                   ▼                                    │
│  ┌──────────────────────────────────────────────────────────────────┐  │
│  │                           RifeOS.SDK                             │  │
│  │  IRifeApp ── IRifeAppContext ── IAppStorage ── INotification ... │  │
│  └──────────────────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────────────────┘

### 插件生命周期时序

[用户在开始菜单点击 App]
       │
       ▼
1. AppLifecycleManager 创建独立 Collectible ALC 沙箱
       │
       ▼
2. 扫描并实例化 IRifeApp 接口实现（共享 SDK 避免类型身份冲突）
       │
       ▼
3. 注入独占的 RifeAppContext 沙箱上下文 (Storage / Config / EventBus)
       │
       ▼
4. 调用 IRifeApp.CreateView() 呈现于主窗口，任务栏生成对应活动 Tab
       │
       │ (用户在任务栏点击 ✕ 或右键选择关闭应用)
       ▼
5. 触发 IRifeApp.Cleanup() 释放非托管句柄与视图绑定
       │
       ▼
6. 卸载 Collectible ALC 沙箱
       │
       ▼
7. 触发 GC.Collect() 彻底释放物理驻留内存

📁 项目目录结构
RifeOS/
├── src/
│   ├── RifeOS.SDK/                     # 【纯净契约层】零 UI 依赖，定义插件生命周期与服务协议
│   │   ├── App/                        # IRifeApp 插件主契约接口
│   │   ├── Context/                    # IRifeAppContext 上下文接口
│   │   ├── Enums/                      # 系统级枚举 (NotificationLevel, AppCategory 等)
│   │   ├── EventBus/                   # 跨沙箱进程内事件总线接口
│   │   ├── Models/                     # 通用元数据与数据模型 (AppMetadata 等)
│   │   └── Services/                   # 存储、配置、主题、通知与用户模型服务契约
│   │
│   ├── RifeOS.Host/                    # 【微内核宿主】无边框外壳、任务栏、Win10 开始菜单与 ALC 管理器
│   │   ├── Context/                    # 沙箱上下文实体实现
│   │   ├── Kernel/Lifecycle/           # PluginLoadContext 与 AppLifecycleManager
│   │   ├── Services/                   # SDK 六大服务宿主底层落地实现
│   │   ├── ViewModels/                 # ShellViewModel, PinnedTileViewModel 等
│   │   ├── Views/                      # ShellWindow (主窗口) 与 DashboardView (概览台)
│   │   ├── Widgets/                    # SystemMonitorWidget (资源监控) 与 CountdownWidget (目标倒计时)
│   │   └── App.xaml                    # 全局 DynamicResource 调色板初始定义
│   │
│   ├── RifeOS.Apps.Tasks/              # 【任务清单插件】独立数据持久化待办应用
│   │   ├── Models/                     # TaskItem 实体
│   │   ├── ViewModels/                 # TasksViewModel
│   │   ├── Views/                      # TasksMainView (XAML & CS)
│   │   └── TasksApp.cs                 # 插件主入口实现
│   │
│   └── RifeOS.Apps.Settings/           # 【系统设置插件】主题切换与沙箱环境监控
│       ├── ViewModels/                 # SettingsViewModel
│       ├── Views/                      # SettingsMainView (XAML & CS)
│       └── SettingsApp.cs              # 插件主入口实现
│
└── RifeOS.sln                          # 解决方案入口

🛠 开发与运行环境
操作系统：Windows 10 (1809+) / Windows 11 (x64 / ARM64)

开发工具：Visual Studio 2022 / 2026（需勾选 .NET 桌面开发 工作负载）

目标框架：.NET 8.0-windows 或更高版本

语言标准：C# 12 / 13

# 🚀 编译与调试指南

## 1. 克隆仓库

```bash
git clone https://github.com/renlyadace1108/RifeOS.git

cd RifeOS
2. 打开解决方案

使用 Visual Studio 打开项目根目录：

RifeOS.sln

或：

RifeOS.slnx
3. 确认项目引用

确保 RifeOS.Host 项目正确引用：

RifeOS.SDK
RifeOS.Apps.Tasks
RifeOS.Apps.Settings

这样在编译时，插件程序集会自动输出到 Host 运行目录，方便动态加载测试。

4. 启动调试

在解决方案资源管理器中：

右键 RifeOS.Host
        ↓
设为启动项目

运行：

F5：启动调试
Ctrl + F5：直接运行

📚 开发者规范与 App 插件扩展指南

RifeOS 所有应用插件均采用独立类库形式开发。

运行时：

由 Host 动态加载
运行于 Collectible AssemblyLoadContext 隔离环境
通过 RifeOS.SDK 与系统通信

插件之间：

数据隔离
生命周期独立
可动态卸载
1. SDK 核心契约速查
IRifeApp

插件根入口接口。

负责：

应用元数据
生命周期管理
UI 创建
资源释放

核心成员：

成员	作用
Metadata	App 信息描述
Initialize(context)	初始化并接收系统上下文
CreateView()	创建应用界面
OnTerminateAsync()	应用退出清理

Host 通过该接口：

发现插件
创建实例
注入环境
管理卸载
IRifeAppContext

应用运行上下文。

由微内核 Host 在初始化阶段注入。

提供：

属性	作用
AppId	当前应用唯一 ID
AppDataDirectory	应用独立数据目录
Storage	沙箱存储服务
Config	配置管理
Notification	系统通知
Theme	主题上下文
IAppStorage

应用独立文件存储服务。

所有 App 文件必须经过该接口访问。

支持：

ReadTextAsync(file)

WriteTextAsync(file, content)

Exists(file)

Delete(file)

数据自动隔离：

Data/{AppId}/
IAppConfiguration

应用配置服务。

支持：

Get<T>(key, default)

Set<T>(key, value)

SaveAsync()

自动：

JSON 序列化
配置持久化
INotificationService

系统通知服务。

接口：

Show(
    string title,
    string content,
    NotificationLevel level
)

用于触发：

Windows 风格 Toast
系统提醒
IThemeContext

主题上下文。

提供：

CurrentTheme

event Action<string> ThemeChanged

用于：

查询当前主题
监听主题变化

2. 创建新的插件应用

示例：

RifeOS.Apps.Notes

一个简单便签应用。

步骤 1：创建类库项目

目录：

src/RifeOS.Apps.Notes

项目文件：

RifeOS.Apps.Notes.csproj

<Project Sdk="Microsoft.NET.Sdk">

<PropertyGroup>

<TargetFramework>net8.0-windows</TargetFramework>

<Nullable>enable</Nullable>

<UseWPF>true</UseWPF>

<ImplicitUsings>enable</ImplicitUsings>

</PropertyGroup>


<ItemGroup>

<ProjectReference Include="..\RifeOS.SDK\RifeOS.SDK.csproj"/>

<PackageReference Include="CommunityToolkit.Mvvm" Version="8.4.0"/>

</ItemGroup>


</Project>
步骤 2：创建 ViewModel
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RifeOS.SDK.Context;


namespace RifeOS.Apps.Notes.ViewModels;


public partial class NotesViewModel : ObservableObject
{

    private readonly IRifeAppContext _context;


    private const string NoteFileName = "note.txt";


    [ObservableProperty]
    private string _content = string.Empty;



    public NotesViewModel(IRifeAppContext context)
    {
        _context = context;

        _ = LoadNoteAsync();
    }



    private async Task LoadNoteAsync()
    {
        Content =
            await _context.Storage.ReadTextAsync(NoteFileName)
            ?? "欢迎使用 RifeOS 便签！";
    }



    [RelayCommand]
    private async Task SaveNote()
    {
        await _context.Storage.WriteTextAsync(
            NoteFileName,
            Content
        );


        _context.Notification.Show(
            "便签已保存",
            "内容已持久化至独立沙箱存储。"
        );
    }
}
步骤 3：创建 View
NotesMainView.xaml

插件 UI 必须使用：

DynamicResource

适配系统主题。

示例：

<UserControl
Background="{DynamicResource AppBackgroundBrush}"
Foreground="{DynamicResource TextPrimaryBrush}">

</UserControl>

禁止：

Background="#FFFFFF"
NotesMainView.xaml.cs
using System.Windows.Controls;
using RifeOS.Apps.Notes.ViewModels;


namespace RifeOS.Apps.Notes.Views;


public partial class NotesMainView : UserControl
{

    public NotesMainView(
        NotesViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }

}
步骤 4：实现 IRifeApp
public class NotesApp : IRifeApp
{

    public AppMetadata Metadata { get; }
    

    private IRifeAppContext? _context;



    public void Initialize(
        IRifeAppContext context)
    {
        _context = context;
    }



    public object CreateView()
    {
        if (_context == null)
            throw new InvalidOperationException();


        return new NotesMainView(
            new NotesViewModel(_context)
        );
    }



    public Task OnTerminateAsync()
    {
        return Task.CompletedTask;
    }

}
步骤 5：注册插件

在：

RifeOS.Host/ViewModels/ShellViewModel.cs

加入：

new AppMenuItemViewModel
{
    AppKey = "Notes",
    Title = "我的便签 (Notes)",
    Icon = "📝",
    Color = "#2563EB"
}
🎨 全局主题调色板

所有插件必须引用 DynamicResource。

Resource Key	Dark	OLED Midnight	Light
AppBackgroundBrush	#0F0F11	#000000	#F3F4F6
SurfaceBackgroundBrush	#161619	#08080A	#FFFFFF
SubSurfaceBackgroundBrush	#1E1E22	#101012	#EBEEF2
BorderBrush	#28282D	#202024	#D7DCE4
TextPrimaryBrush	#FFFFFF	#FFFFFF	#111827
TextSecondaryBrush	#888888	#787878	#64748B
TileTasksBrush	#1D4ED8	#1E3A8A	#2563EB
TileSettingsBrush	#4F46E5	#4338CA	#6366F1
TileDashboardBrush	#047857	#064E3B	#10B981
TileDefaultBrush	#2563EB	#1E3A8A	#3B82F6
WatermarkOpacity	0.12	0.16	0.08
🛠 关键开发规范
1. 防止 ALC 类型冲突

插件必须：

使用共享的 RifeOS.SDK
禁止复制 SDK DLL 到插件隔离目录

保证：

typeof(IRifeApp)

全系统唯一。

2. 沙箱隔离原则

禁止：

File.ReadAllText(
"C:\\xxx"
)

必须：

_context.Storage

或：

_context.AppDataDirectory
3. ContextMenu 绑定规范

WPF：

ContextMenu

不属于原 Visual Tree。

因此右键菜单绑定 ViewModel 时：

统一使用：

PlacementTarget.Tag

实现命令穿透。

📌 插件设计目标

RifeOS 插件系统最终实现：

即用即载
独立运行
安全隔离
动态卸载
统一主题
统一通知
统一存储

让 RifeOS 具备类似：

Visual Studio Extension
VS Code Extension
Windows App

的模块化扩展能力。


