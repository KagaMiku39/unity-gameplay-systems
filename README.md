# Harvest Farm Gameplay Systems

一套面向农场经营游戏的 Unity 模块化玩法系统，围绕背包、任务、商店、世界交互与 UI 状态管理构建。仓库保存的是本人独立实现的玩法代码与最小演示场景，用于和团队农场项目对接；不包含队友代码、团队场景以及来源不明的商业美术、音频和视频资源。

## 为什么做

农场游戏中的拾取、背包、任务与商店会频繁共享物品和状态。如果每个 UI 直接修改其他模块，功能增加后很容易出现耦合、重复逻辑和难以测试的问题。本项目把这些玩法拆成清晰的 Domain、Application、Infrastructure 与 Presentation 边界，再通过消息总线和组合根连接。

## 怎么做

- 背包支持固定容量、堆叠上限、部分添加、删除和格子交换。
- 物品、任务与商店库存使用 3 份 JSON 配置，由独立 Database 加载和查询。
- `CommandBus` 处理有明确结果的操作，`EventBus` 广播状态变化，UI 不直接修改领域对象。
- 任务模块包含 Definition、Runtime、Factory、Tracker 与 Manager，并能由背包事件推进收集任务。
- 商店交易同时校验库存、背包容量和钱包余额；失败或部分成功时返回明确结果并保护状态一致性。
- 世界交互包含触发拾取、射线交互、提示显示和剩余数量回写。
- `UiPanelStack` 与 `GameplayInputLock` 统一管理面板互斥、Esc 返回、移动和世界交互锁定。
- 通过 asmdef 划分 9 个运行时/测试程序集，降低模块间的隐式引用。

## 结果

- `Assets/_Game` 包含 92 个 C# 文件，覆盖 Bootstrap、Inventory、Quest、Shop 与 Shared 基础设施。
- 包含 28 个 NUnit EditMode 测试用例，覆盖背包边界、任务推进、交易补偿、世界拾取、UI 栈和输入锁。
- 公开仓库仅约 4 MiB，不携带团队工程中约数百 MiB 的第三方美术和工具包。
- 核心玩法可以在最小开发场景中独立演示，也可以通过组合根接入更完整的团队游戏。

## 技术栈

- Unity `2022.3.62f3c1`
- C# / UGUI / TextMeshPro / Newtonsoft.Json
- CommandBus / EventBus / Composition Root
- Domain / Application / Infrastructure / Presentation
- asmdef / NUnit EditMode Tests

## 调用关系

```text
JSON -> Database -> GameBootstrapper
                       |
                       +-> CommandBus -> Handler -> Domain state
                       |
                       +-> EventBus -> View / QuestTracker / Logger

ShopView ------ Buy/Sell Command ------> Inventory + Wallet
WorldPickup --- AddItem Command -------> Inventory
Inventory Event -----------------------> InventoryView + QuestTracker
Quest Event ---------------------------> QuestView + QuestRuntimeManager
E / Esc -------------------------------> UiPanelStack -> GameplayInputLock
F -------------------------------------> RayInteractor -> Pickup / Shop
```

更详细的模块边界见 [`docs/architecture.md`](docs/architecture.md)。

## 运行

1. 使用 Unity Hub 通过 `2022.3.62f3c1` 打开仓库。
2. 打开 `Assets/_Game/Scenes/Development/InventoryStudy.unity`。
3. 进入 Play Mode。

| 按键 | 功能 |
|---|---|
| WASD / 方向轴 | 移动角色 |
| E | 打开或关闭背包 |
| Esc | 关闭最上层 UI |
| F | 与射线命中的拾取物或商店交互 |
| 1-5 | 切换快捷栏选中格子 |

## 测试

在 Unity 中打开 `Window > General > Test Runner`，选择 `EditMode` 后运行全部测试。测试代码与被测 Runtime 分属不同 asmdef，不会进入普通运行时程序集。

## 所有权边界

本仓库的玩法系统以个人独立实现形式维护。完整团队农场项目中的其他成员源码、地图、美术、模型、音频、视频和第三方编辑器插件不在此仓库内。TextMesh Pro Essential Resources 的许可与署名见 [`THIRD_PARTY_NOTICES.md`](THIRD_PARTY_NOTICES.md)。
