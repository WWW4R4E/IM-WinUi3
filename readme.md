# README

## 项目简介

本项目是一个基于 **WinUI 3** 的客户端与 **ASP.NET Core** 服务端的通信应用，使用 **.NET 9** 作为开发框架，并通过 **SignalR** 实现实时通信功能。目前项目已实现以下核心功能：

- 用户登录
- 实时聊天

---

## 技术栈

- **客户端**: WinUI 3 (.NET 9)    
- **服务端**: ASP.NET Core (.NET 9)  
- **通信协议**: SignalR  
- **开发语言**: C#  

---

## 功能概述

### 1. 登录功能
用户可以通过客户端输入用户名和密码进行登录。登录成功后，客户端会与服务端建立 SignalR 连接。

### 2. 聊天功能
登录成功后，用户可以与其他在线用户进行实时聊天。消息通过 SignalR 在客户端和服务端之间实时传递。

---

## 项目结构

```
IM-WinUi3/
├── ChatRoomASP/               # ASP.NET Core 服务端项目
│   ├── bin/                   # 编译输出文件夹
│   ├── Controllers/           # API 控制器
│   ├── Hubs/                  # SignalR Hub 实现
│   ├── Migrations/            # 数据库迁移文件
│   ├── Models/                # 数据模型
│   ├── Properties/            # 项目属性配置
│   ├── Services/              # 服务实现
│   ├── Views/                 # 视图文件
│   ├── appsettings.json       # 应用配置文件
│   └── Program.cs             # 服务端启动配置
├── IM-WinUi/                  # WinUI 3 客户端项目
│   ├── Assets/                # 资源文件夹
│   ├── Helper/                # 辅助类或工具类
│   ├── Models/                # 数据模型
│   ├── Properties/            # 项目属性配置
│   ├── Services/              # 客户端服务（SignalR 通信）
│   ├── ViewModels/            # 视图模型
│   ├── Views/                 # 客户端页面
│   ├── App.xaml               # XAML 主界面定义
│   └── App.xaml.cs            # 应用入口代码文件
└── README.md                  # 项目说明文档
```

---

## 快速开始

### 1. 环境要求
- .NET SDK 9.0 或更高版本
- Visual Studio 2022 或更高版本（支持 WinUI 3 和 ASP.NET Core 开发）

### 2. 克隆项目
```bash
git clone https://github.com/your-repo/IM-WinUi3.git
cd IM-WinUi3
```

### 3. 启动服务端
进入 `Server` 目录并运行以下命令：
```bash
dotnet run
```
服务端默认监听 `http://localhost:5000`。

### 4. 启动客户端
在 Visual Studio 中打开解决方案文件，设置 `Client` 项目为启动项目并运行。

---

## 使用说明

1. 打开客户端应用程序。
2. 输入用户名和密码进行登录。
3. 登录成功后，进入聊天界面，选择目标用户发送消息。

---

## 待办功能

- [ ] 支持群聊功能
- [ ] 添加用户注册功能
- [ ] 消息历史记录查询
- [ ] 文件传输功能

---

