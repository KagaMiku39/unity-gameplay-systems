# Architecture

## Composition root

`GameBootstrapper` is the only place that assembles databases, domain state, handlers, trackers and views. Individual UI components receive the capabilities they need instead of searching the scene for unrelated objects.

## Shared messaging

- `CommandBus` maps one command type to one handler and returns a typed result. It is used when the caller must know whether an operation succeeded, partially succeeded or failed.
- `EventBus` allows multiple listeners to observe completed state changes. Inventory, quest and UI modules can react without holding direct references to each other.

The buses do not contain game rules. Validation and mutation remain inside feature handlers and domain objects.

## Inventory

The inventory owns fixed slots and stack rules. Add/remove commands return the amount actually processed, allowing world pickups and shop transactions to keep unprocessed quantities instead of silently losing items.

Item definitions are immutable configuration loaded from JSON; slot state stores only the item identifier and runtime quantity.

## Quest

Quest definitions describe objectives, while runtime objects own progress. Trackers translate domain events, such as an inventory item change, into objective progress. A manager controls active quest lifetime and publishes progress/completion events for views.

## Shop

Buy and sell handlers coordinate shop stock, inventory capacity and wallet balance. Preconditions are checked before mutation, and multi-object changes are applied through a single command result so the UI does not implement transaction rules.

## Presentation and input

Views subscribe to events and render state. `UiPanelStack` gives nested panels deterministic open/close ordering; `GameplayInputLock` aggregates locks so closing one panel cannot incorrectly re-enable movement while another panel is still open.

## Test boundaries

EditMode tests construct domain objects and handlers directly without loading the development scene. Separate test asmdefs reference only the Runtime assemblies under test, keeping dependencies explicit.
