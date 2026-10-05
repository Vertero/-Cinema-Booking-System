# 项目技术说明

## 1. 说明范围

本文档记录当前仓库中能够直接从源码和工程文件确认的实现信息。

该项目是 2020 年完成的 Windows Forms 电影院订票系统。当前仓库包含 UI 工程源码和若干二进制依赖，但不包含原始 SQL Server 数据库定义，也不包含所有被引用项目的源码。

## 2. 功能映射

### 用户端

| 组件 | 主要职责 |
| --- | --- |
| `Form1` | 用户登录与程序入口 |
| `Form2` | 用户订票菜单 |
| `Form3` | 用户注册 |
| `Form4` | 影片信息与排片选择 |
| `BuyTickets` | 座位生成、选择、占用检查与订单写入 |
| `Form5` | 订单记录及相关操作 |
| `Form6` | 会员 / VIP 操作 |

### 管理员端

| 组件 | 主要职责 |
| --- | --- |
| `Form7` | 管理员登录 |
| `Form8` | 管理员导航 |
| `Form9` | 影厅管理 |
| `Form10` | 影片管理 |
| `Form11` | 排片管理 |
| `Form12` | 订单管理 |
| `Form13` | 用户管理 |
| `Form14` | 统计分析导航 |
| `Form15` | 统计相关视图 |
| `Form17` | 统计相关视图 |

`Form16` 和 `Form18` 保留在源码目录中。由于当前仓库中的上下文不足以可靠确认其完整运行职责，这里不进一步指定。

## 3. 工程依赖

当前主工程为 `hh.csproj`，目标环境为：

- .NET Framework 4.0 Client Profile
- x86
- Windows Forms

工程依赖包括：

- `Model.dll`
- `PublicLib.dll`
- `IrisSkin4.dll`
- `MP10.ssk`
- 通过 `System.Data.SqlClient` 访问 SQL Server

原始 `hh.csproj` 将 `Model` 和 `PublicLib` 作为仓库之外的兄弟源码项目引用。当前仓库中没有这两个项目的源码。原构建目录中能够找到的 DLL 已整理到 `lib/`。

## 4. 数据库依赖

配置中的数据库名称为 `CinemaSystem`。

当前源码中可以看到对 `Hall`、`Movie`、`Timing`、`Order` 等表的访问，也可以看到对 `CheckCustomerLogin` 等存储过程的调用。

当前仓库不包含：

- SQL 数据表定义
- 存储过程定义
- 初始化数据
- SQL Server 数据库备份

因此，数据库中的约束、索引、触发器以及存储过程具体实现无法根据当前仓库验证。

## 5. 数据访问方式

多个 WinForms 类直接使用：

- `SqlConnection`
- `SqlCommand`
- `SqlDataAdapter`
- `DataSet`
- `SqlDataReader`

进行数据库访问。

部分命令通过带参数的存储过程执行；另有部分 SQL 语句通过字符串拼接生成。

数据库连接创建、打开、关闭和异常处理逻辑分别存在于多个 Form 中。

## 6. 购票流程

`BuyTickets.cs` 中可以确认以下流程：

1. 读取影厅行数和列数；
2. 动态生成座位控件；
3. 从 `Order` 表读取已售座位；
4. 在界面中记录用户选择的座位；
5. 写入前再次检查座位状态；
6. 写入订单记录。

应用代码中的“座位检查”和“订单写入”是两个分开的数据库操作。

数据库层是否另外设置了防止重复订座的约束，目前无法确认，因为数据库 schema 不在仓库中。

## 7. 配置

原仓库的 `app.config` 中包含 SQL Server 连接字符串。

当前分支使用 Windows Integrated Authentication 示例：

```text
Data Source=.;Initial Catalog=CinemaSystem;Integrated Security=True
```

除非重写 Git 历史，否则旧配置仍会存在于历史 commit 中。

## 8. 测试

当前仓库中没有自动化测试项目。

依赖缺失数据库的运行行为无法仅通过当前仓库复现。
