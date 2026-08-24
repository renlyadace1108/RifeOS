✦ RifeOS (Personal Growth Operating System)
![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white) ![C#](https://img.shields.io/badge/C%23-12.0%20%7C%2013.0-239120?style=for-the-badge&logo=csharp&logoColor=white) ![WPF](https://img.shields.io/badge/UI-WPF%20%7C%20Modern%20Dark-0078D4?style=for-the-badge&logo=windows&logoColor=white) ![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%2F%2011-00A4EF?style=for-the-badge&logo=windows11&logoColor=white) ![Architecture](https://img.shields.io/badge/Architecture-Microkernel%20%2B%20ALC%20Sandbox-blueviolet?style=for-the-badge) ![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge) 基于 .NET 8 / C# 12 / WPF 构建的轻量级、模块化、沙箱插件隔离型桌面个人操作系统工作台。
🌟 核心特性与设计亮点
🏛 微内核宿主架构 (Microkernel Core)
宿主保持极低底噪，仅提供窗口管理、插件沙箱调度、系统总线与主题调色板分发。
🧩 可回收 ALC 动态沙箱 (Collectible AssemblyLoadContext)
每个应用插件作为独立的 DLL 存在。应用从开始菜单点击时动态加载，从任务栏关闭后立刻触发深度垃圾回收（GC & WorkingSet 释放），不留常驻内存负担。
🪟 沉浸式 Windows 桌面交互体系
无边框极简视窗：顶部隐形拖拽区域，支持全局顺畅拖动。
纯净桌面开机体验：默认仅呈现极简品牌水印，无冗余首屏遮挡。
Windows 10 风格开始菜单：
左侧全量应用垂直列表；
右侧彩色磁贴网格（Live Tiles）；
支持列表项右键「固定到开始屏幕」、磁贴右键「取消固定」，且在右键菜单交互期间保持展开不失焦。
动态多任务栏：
居中单字母 R 极简开始按钮，鼠标移入自动展开开始菜单；
运行中的应用自动生成任务栏 Tab，支持左键快速激活/最小化回桌面，右键唤出快捷菜单（最小化 / 彻底退出并释放内存）。
🎨 全系统多主题秒级无缝联动
内置 Dark (默认深色)、OLED Midnight (极黑)、Light (浅色) 三套主题。窗口底色、任务栏、开始菜单、磁贴背景、图标颜色及所有运行中的子应用通过 DynamicResource 体系实现全局联动换肤。
📦 契约驱动与数据沙箱隔离
RifeOS.SDK 零 UI 强依赖。各插件独占 Data/{AppId}/ 本地沙箱目录，具备独立存储与强类型 JSON 配置管理。
🏛 系统架构设计
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


插件生命周期时序
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
🚀 编译与调试指南
克隆仓库：
git clone https://github.com/renlyadace1108/RifeOS.git
cd RifeOS


打开解决方案： 使用 Visual Studio 打开根目录下的 RifeOS.sln（或 RifeOS.slnx）。
依赖与引用确认： 确保 RifeOS.Host 项目的项目引用中勾选了 RifeOS.SDK、RifeOS.Apps.Tasks 与 RifeOS.Apps.Settings（确保编译时自动将插件输出至 Host 运行目录）。
启动调试：
在解决方案资源管理器中，右键 RifeOS.Host $\rightarrow$ 选择 “设为启动项目”。
按 F5 启动调试，或按 Ctrl + F5 极速运行。
📚 开发者规范与 App 插件扩展指南
所有应用插件均以独立类库形式构建，运行时完全托管于 Collectible ALC 隔离沙箱中。
1. SDK 核心契约速查
接口
核心方法 / 属性
用途说明
 
IRifeApp
Metadata
Initialize(context)
CreateView()
Cleanup()
插件根入口契约，微内核据此完成装载、上下文注入、视图提取与安全卸载。
IRifeAppContext
AppId
AppDataDirectory
Storage
Config
Notification
Theme
沙箱上下文容器，由微内核在初始化时单向注入。
IAppStorage
ReadTextAsync(file)
WriteTextAsync(file, content)
Exists(file) / Delete(file)
独占式沙箱文件存取服务，自动重定向至 Data/{AppId}/。
IAppConfiguration
Get<T>(key, default)
Set<T>(key, value)
SaveAsync()
强类型配置读写，自动序列化为 JSON 持久化。
INotificationService
Show(title, content, level)
触发宿主右上角 Windows 风格 Toast 浮动通知。
IThemeContext
CurrentTheme
event Action<string> ThemeChanged
当前系统主题查询与变更订阅。


2. 5 步构建全新插件（以 RifeOS.Apps.Notes 便签为例）
步骤 1：新建类库工程
在 src/ 目录下创建 RifeOS.Apps.Notes，修改 .csproj：
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


步骤 2：编写 ViewModel
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RifeOS.SDK.Context;

namespace RifeOS.Apps.Notes.ViewModels;

public partial class NotesViewModel : ObservableObject
{
    private readonly IRifeAppContext _context;
    private const string NoteFileName = "note.txt";

    [ObservableProperty] private string _content = string.Empty;

    public NotesViewModel(IRifeAppContext context)
    {
        _context = context;
        _ = LoadNoteAsync();
    }

    private async Task LoadNoteAsync()
    {
        Content = await _context.Storage.ReadTextAsync(NoteFileName) ?? "欢迎使用 RifeOS 便签！";
    }

    [RelayCommand]
    private async Task SaveNote()
    {
        await _context.Storage.WriteTextAsync(NoteFileName, Content);
        _context.Notification.Show("便签已保存", "内容已持久化至独立沙箱存储。");
    }
}


步骤 3：编写 View（必须使用 DynamicResource 适配主题）
Views/NotesMainView.xaml：
<UserControl Background="{DynamicResource AppBackgroundBrush}" FontFamily="Segoe UI, Microsoft YaHei UI" Foreground="{DynamicResource TextPrimaryBrush}" x:Class="RifeOS.Apps.Notes.Views.NotesMainView" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Border Padding="32">
        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="*"/>
                <RowDefinition Height="Auto"/>
            </Grid.RowDefinitions>

            <TextBlock FontSize="22" FontWeight="Bold" Margin="0,0,0,16" Text="我的便签"/>

            <TextBox AcceptsReturn="True" Background="{DynamicResource SubSurfaceBackgroundBrush}" BorderBrush="{DynamicResource BorderBrush}" BorderThickness="1" FontSize="13" Foreground="{DynamicResource TextPrimaryBrush}" Grid.Row="1" Padding="14" Text="{Binding Content, UpdateSourceTrigger=PropertyChanged}" TextWrapping="Wrap"/>

            <Button Background="#3B82F6" BorderThickness="0" Command="{Binding SaveNoteCommand}" Content="保存便签" Cursor="Hand" FontWeight="SemiBold" Foreground="#FFFFFF" Grid.Row="2" Height="36" HorizontalAlignment="Right" Margin="0,16,0,0" Width="100"/>
        </Grid>
    </Border>
</UserControl>


Views/NotesMainView.xaml.cs：
using System.Windows.Controls;
using RifeOS.Apps.Notes.ViewModels;

namespace RifeOS.Apps.Notes.Views;

public partial class NotesMainView : UserControl
{
    public NotesMainView(NotesViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}


步骤 4：实现 IRifeApp 入口
using RifeOS.Apps.Notes.ViewModels;
using RifeOS.Apps.Notes.Views;
using RifeOS.SDK.App;
using RifeOS.SDK.Context;
using RifeOS.SDK.Enums;
using RifeOS.SDK.Models;

namespace RifeOS.Apps.Notes;

public class NotesApp : IRifeApp
{
    public AppMetadata Metadata { get; } = new(
        "Notes",
        "便签",
        "极简快速记录工具",
        "1.0.0",
        "Renly",
        "StickyNoteIcon",
        AppCategory.Productivity
    );

    private IRifeAppContext? _context;

    public void Initialize(IRifeAppContext context) => _context = context;

    public object CreateView()
    {
        if (_context == null) throw new InvalidOperationException("NotesApp 尚未初始化。");
        return new NotesMainView(new NotesViewModel(_context));
    }

    public void Cleanup() { }
}


步骤 5：在 Host 中注册该插件
在 RifeOS.Host/ViewModels/ShellViewModel.cs 的 AllApps 列表中加入入口：
new AppMenuItemViewModel { AppKey = "Notes", Title = "我的便签 (Notes)", Icon = "📝", Color = "#2563EB" }


🎨 全局主题调色板对照表
为保证全系统主题秒级无缝联动，插件 XAML 中严禁硬编码颜色，统一引用以下动态画刷：
资源键名 (DynamicResource)
Dark (默认深色)
OLED Midnight (极黑)
Light (浅色)
 
AppBackgroundBrush
#0F0F11
#000000
#F3F4F6
SurfaceBackgroundBrush
#161619
#08080A
#FFFFFF
SubSurfaceBackgroundBrush
#1E1E22
#101012
#EBEEF2
BorderBrush
#28282D
#202024
#D7DCE4
TextPrimaryBrush
#FFFFFF
#FFFFFF
#111827
TextSecondaryBrush
#888888
#787878
#64748B
TileTasksBrush
#1D4ED8
#1E3A8A
#2563EB
TileSettingsBrush
#4F46E5
#4338CA
#6366F1
TileDashboardBrush
#047857
#064E3B
#10B981
TileDefaultBrush
#2563EB
#1E3A8A
#3B82F6
WatermarkOpacity
0.12
0.16
0.08


🛠 关键避坑与开发铁律
杜绝 ALC 类型身份冲突：插件必须将 RifeOS.SDK 设为共享依赖，严禁插件隔离区私自重复加载 SDK 副本，确保 typeof(IRifeApp) 接口类型身份全局唯一。
绝对沙箱隔离：插件存取文件严禁使用物理绝对路径（如 C:\...），必须统一通过 _context.Storage 或 _context.AppDataDirectory 进行读写。
右键 ContextMenu 穿透绑定：WPF 的 ContextMenu 独立于主 Visual Tree，在磁贴或任务栏右键菜单中调用 ViewModel 命令时，请统一使用 PlacementTarget.Tag 进行穿透绑定。
📄 开源协议
本项目基于 MIT License 开源，欢迎自由学习、演进与扩展！
