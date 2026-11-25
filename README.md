# StoneToolkit

WPF 与通用基础类库集合。原位于 `ScTrials` 仓库的 `src/StoneToolkit` 目录，现拆分为独立仓库。

## 项目结构

```
StoneToolkit/
├─ src/            类库
└─ samples/        示例程序
```

## 类库

| 项目 | 目标框架 | 说明 |
|---|---|---|
| `StoneToolkit.Common` | netstandard2.0 | 通用基础类型与工具方法 |
| `StoneToolkit.Extension` | net9.0-windows; net472 | 扩展方法集合 |
| `StoneToolkit.OpenAi` | net9.0 | OpenAI 客户端封装 |
| `StoneToolkit.Web` | net9.0 | Web / 服务端基础设施 |
| `StoneToolkit.WpfCommon` | net9.0-windows; net472 | WPF 通用基础设施 |
| `StoneToolkit.WpfControl` | net9.0-windows; net472 | WPF 自定义控件 |
| `StoneToolkit.WpfShape` | net9.0-windows | WPF 图形绘制 |
| `StoneToolkit.WpfUserControl` | net9.0-windows; net472 | WPF 用户控件 |

## 依赖关系

```
Common ──┬─→ Extension
         ├─→ Web
         ├─→ WpfCommon ──┬─→ WpfUserControl
         │               │
         └────────────────┴─→ WpfControl ──→ WpfUserControl

OpenAi    （独立）
WpfShape  （独立）
```

## 示例

`samples/` 下为各功能的独立示例程序，均以 `ProjectReference` 方式引用 `src/` 中的类库。

## 构建

```cmd
dotnet build StoneToolkit.slnx
```

## 被其他仓库引用

其他仓库（如 `ScTrials`）通过 `$(StoneToolkitRoot)` 属性 + `ProjectReference` 引用本仓库源码，
以便在调试时可直接步入类库代码。使用方仓库需保证本仓库位于其同级目录，或设置环境变量
`STONETOOLKIT_ROOT` 指向 `StoneToolkit\src`。
