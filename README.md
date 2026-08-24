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


📚 RifeOS 开发者规范与 App 插件扩展指南RifeOS 采用 微内核 + 契约驱动 + 动态沙箱 架构。所有应用插件均以独立 DLL 形式存在，通过 .NET 隔离的 AssemblyLoadContext 动态载入运行，并在关闭时即刻触发垃圾回收释放物理内存。一、SDK 契约核心接口总览所有插件仅需引用 RifeOS.SDK 程序集，严禁引用 RifeOS.Host。┌──────────────────────────────────────────────────────────┐
│                        RifeOS.SDK                        │
├──────────────────────────┬───────────────────────────────┤
│ IRifeApp (主入口契约)    │ IRifeAppContext (上下文沙箱)  │
│ ├── Metadata             │ ├── AppId                     │
│ ├── Initialize(context)  │ ├── AppDataDirectory          │
│ ├── CreateView()         │ ├── Storage (IAppStorage)     │
│ └── Cleanup()            │ ├── Config (IAppConfiguration)│
│                          │ ├── Notification              │
│                          │ ├── Theme (IThemeContext)     │
│                          │ ├── EventBus (IEventBus)      │
│                          │ └── CurrentUser (IUserProfile)│
└──────────────────────────┴───────────────────────────────┘
1. 主程序入口：IRifeApp每个 App 必须有且仅有一个实现 IRifeApp 的公开类。C#namespace RifeOS.SDK.App;

public interface IRifeApp
{
    // 应用元数据（名称、版本、作者、分类等）
    AppMetadata Metadata { get; }

    // 初始化钩子：微内核装载时注入沙箱运行上下文
    void Initialize(IRifeAppContext context);

    // 视图构造钩子：返回呈现于工作台主区域的 WPF 根视图 (UserControl)
    object CreateView();

    // 资源释放钩子：在从任务栏关闭/卸载插件前调用，用于注销事件、关闭文件句柄
    void Cleanup();
}
2. 运行上下文：IRifeAppContext微内核通过此上下文向应用沙箱注入所有系统级能力：C#namespace RifeOS.SDK.Context;

public interface IRifeAppContext
{
    string AppId { get; }            // 插件专属标识符（如 "Tasks", "Settings"）
    string AppDataDirectory { get; }     // 当前插件独占的沙箱数据目录（Data/{AppId}/）
    
    IAppStorage Storage { get; }         // 独占文件存储服务（读写文本、文件操作）
    IAppConfiguration Config { get; }    // 强类型配置存取服务（自动持久化为 JSON）
    INotificationService Notification { get; } // 系统级 Toast 通知广播
    IThemeContext Theme { get; }         // 当前系统主题状态与切换监听
    IEventBus EventBus { get; }          // 跨沙箱进程内事件总线
    IUserProfile CurrentUser { get; }    // 当前登录系统用户信息
}
3. 基础服务契约一览接口关键方法 / 属性用途说明IAppStorageReadTextAsync(file)WriteTextAsync(file, content)Exists(file) / Delete(file)读写当前 App 专属目录中的文件，实现绝对隔离。IAppConfigurationGet<T>(key, default)Set<T>(key, value)SaveAsync()基于键值对存取配置，支持复杂对象序列化。INotificationServiceShow(title, content, level)触发主窗口右上角的 Windows 风格 Toast 浮动通知。IThemeContextCurrentThemeevent Action<string> ThemeChanged读取当前主题名称或订阅主题变更事件。IEventBusPublish<T>(msg)Subscribe<T>(handler)跨 App 解耦通信（如任务数变动通知概览台）。二、从零开发一个全新 App 插件（以 RifeOS.Apps.Notes 便签为例）步骤 1：新建类库工程在 src/ 目录下创建 .NET 8 / 9 WPF 类库工程 RifeOS.Apps.Notes。修改 RifeOS.Apps.Notes.csproj：XML<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <UseWPF>true</UseWPF>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\RifeOS.SDK\RifeOS.SDK.csproj" />
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.4.0" />
  </ItemGroup>
</Project>
步骤 2：建立标准的 MVVM 目录结构PlaintextRifeOS.Apps.Notes/
├── Models/
│   └── NoteItem.cs
├── ViewModels/
│   └── NotesViewModel.cs
├── Views/
│   ├── NotesMainView.xaml
│   └── NotesMainView.xaml.cs
└── NotesApp.cs (IRifeApp 入口)
步骤 3：编写 ViewModel（使用注入的上下文存储数据）C#using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RifeOS.SDK.Context;

namespace RifeOS.Apps.Notes.ViewModels;

public partial class NotesViewModel : ObservableObject
{
    private readonly IRifeAppContext _context;
    [ObservableProperty] private string _content = string.Empty;

    public NotesViewModel(IRifeAppContext context)
    {
        _context = context;
        _ = LoadNoteAsync();
    }

    private async Task LoadNoteAsync()
    {
        Content = await _context.Storage.ReadTextAsync("note.txt") ?? "欢迎使用 RifeOS 便签！";
    }

    [RelayCommand]
    private async Task Save()
    {
        await _context.Storage.WriteTextAsync("note.txt", Content);
        _context.Notification.Show("便签已保存", "内容已写入沙箱持久化存储");
    }
}
步骤 4：编写 View（必须使用 DynamicResource 适配主题）NotesMainView.xaml：XML<UserControl x:Class="RifeOS.Apps.Notes.Views.NotesMainView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             Background="{DynamicResource AppBackgroundBrush}"
             Foreground="{DynamicResource TextPrimaryBrush}">
    <Border Padding="32">
        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="*"/>
                <RowDefinition Height="Auto"/>
            </Grid.RowDefinitions>

            <TextBlock Text="我的便签" FontSize="22" FontWeight="Bold" Margin="0,0,0,16"/>

            <TextBox Grid.Row="1" Text="{Binding Content, UpdateSourceTrigger=PropertyChanged}"
                     AcceptsReturn="True" TextWrapping="Wrap"
                     Background="{DynamicResource SubSurfaceBackgroundBrush}"
                     Foreground="{DynamicResource TextPrimaryBrush}"
                     BorderBrush="{DynamicResource BorderBrush}" Padding="12"/>

            <Button Grid.Row="2" Content="保存" Command="{Binding SaveCommand}"
                    Height="36" Width="100" HorizontalAlignment="Right" Margin="0,16,0,0"
                    Background="#3B82F6" Foreground="#FFFFFF" BorderThickness="0" Cursor="Hand"/>
        </Grid>
    </Border>
</UserControl>
NotesMainView.xaml.cs：C#using System.Windows.Controls;
using RifeOS.Apps.Notes.ViewModels;

namespace RifeOS.Apps.Notes.Views;

public partial class NotesMainView : UserControl
{
    public NotesMainView(NotesViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
步骤 5：实现 IRifeApp 入口C#using RifeOS.Apps.Notes.ViewModels;
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
        "RifeOS",
        "StickyNote",
        AppCategory.Productivity
    );

    private IRifeAppContext? _context;

    public void Initialize(IRifeAppContext context) => _context = context;

    public object CreateView()
    {
        if (_context == null) throw new InvalidOperationException("未初始化");
        return new NotesMainView(new NotesViewModel(_context));
    }

    public void Cleanup() { }
}
三、关键避坑与开发铁律色彩规范：插件 XAML 中严禁硬编码控件的 Background="#141416" 或 Foreground="#FFFFFF"，必须统一使用 {DynamicResource AppBackgroundBrush}、{DynamicResource SurfaceBackgroundBrush}、{DynamicResource TextPrimaryBrush} 等标准资源键，以保证全系统主题联动。绝对沙箱隔离：插件读写文件严禁使用硬编码物理绝对路径（如 C:\...），必须统一通过 _context.Storage 或 _context.AppDataDirectory 访问自身独占的沙箱空间。依赖规范：插件工程只引用 RifeOS.SDK；宿主工程通过 PluginLoadContext 在运行时动态加载 RifeOS.Apps.*.dll，实现了微内核与各插件的彻底物理隔离。
