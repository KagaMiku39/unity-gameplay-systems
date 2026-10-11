# Harvest Farm Gameplay Systems

一个 Unity 农场玩法示例项目，包含背包、任务、商店、物品拾取和 UI 面板管理。仓库保留玩法代码和一个最小开发场景，方便单独运行、阅读和接入其他项目。

## 运行

使用 Unity Hub，以 Unity `2022.3.62f3c1` 打开项目，然后打开：

`Assets/_Game/Scenes/Development/InventoryStudy.unity`

进入 Play Mode 后，可以移动角色、拾取物品、查看背包和进行商店交易。

| 按键 | 功能 |
| --- | --- |
| WASD / 方向轴 | 移动角色 |
| E | 打开或关闭背包 |
| Esc | 关闭最上层 UI |
| F | 与射线命中的拾取物或商店交互 |
| 1–5 | 切换快捷栏选中格子 |

项目使用 UGUI、TextMeshPro 和 Newtonsoft.Json，依赖配置位于 `Packages/manifest.json`。

## 目前包含的功能

- **背包**：固定格子数量，支持堆叠、添加、删除和格子交换。添加物品时返回未装下的数量。
- **任务**：根据背包中当前持有的物品数量更新进度，满足条件后完成任务，并按配置启动下一个任务。
- **商店**：校验商品是否在售、钱包余额和背包容量。购买时若只装下部分物品，会退回未成交部分的金币；出售时扣除物品并增加金币。
- **世界交互**：支持触发拾取和射线交互。背包装不下的物品会保留在场景中。
- **UI 与输入**：用面板栈管理打开和返回顺序，用输入锁控制面板打开时的移动和世界交互。

物品、任务和商店商品目录分别由 JSON 配置。商店目前没有有限库存数量或补货逻辑。

## 代码位置

主要代码在 `Assets/_Game`：

| 目录 | 内容 |
| --- | --- |
| `Bootstrap/Runtime` | 初始化数据库、玩法对象、命令处理器和界面 |
| `Bootstrap/Editor` | 开发场景和 UI 的配置工具 |
| `Features/Inventory` | 背包、快捷栏和世界物品拾取 |
| `Features/Quest` | 任务定义、运行状态、进度跟踪和任务链 |
| `Features/Shop` | 钱包、买卖处理和商店界面 |
| `Shared` | 命令、事件、面板栈、输入锁等共用代码 |
| `Scenes/Development` | 最小演示场景 |

每个玩法模块中的 `Runtime` 按职责分成几部分：

- `Domain` 保存玩法数据和规则。
- `Application` 处理操作、查询和事件。
- `Infrastructure` 读取配置。
- `Presentation` 连接 Unity 场景、输入和界面。

`GameBootstrapper` 负责把这些对象连接起来。需要返回结果的操作通过 `CommandBus` 调用；状态变化通过 `EventBus` 通知界面和任务跟踪器。

如果从背包开始阅读，可以按下面的顺序看：

1. `InventorySlot.cs` 和 `ItemDefinition.cs`：格子状态和物品配置。
2. `Inventory.cs`：添加、删除和交换的规则。
3. `AddItemCommandHandler.cs`：查找配置、调用背包并发送变化事件。
4. `InventoryView.cs`：接收事件并刷新界面。
5. `GameBootstrapper.cs`：这些对象如何被创建和连接。

模块之间的关系另见 [docs/architecture.md](docs/architecture.md)。

## 测试

在 Unity 中打开 `Window > General > Test Runner`，选择 `EditMode`，运行全部测试。

测试位于各模块的 `Tests/EditMode` 目录，包含背包堆叠与容量边界、物品删除、任务推进、商店买卖、拾取剩余数量、面板栈和输入锁等用例。测试使用 NUnit，并通过独立的 asmdef 与运行时代码分开。

## 项目范围与许可

仓库包含玩法系统和最小演示场景，完整农场游戏的地图、其他成员代码和第三方商业素材未包含在内。

项目代码采用 [MIT License](LICENSE)。随项目保留的 TextMesh Pro Essential Resources 及相关字体、素材许可见 [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md)。
