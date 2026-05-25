# TopDown Small RPG Demo

Small top-down RPG demo built in Unity. The project is in active development and focuses on a polished, gameplay vertical slice.

## Stack

- Unity 6, URP
- UI: UiToolkit
- Input: New Input System
- Camera: Cinemachine
- DI: VContainer
- Async: UniTask
- Localization: Unity Localization
- Tweening: PrimeTween
- Tests: Unity Test Framework


## Architecture

The project uses small runtime modules with clear responsibilities, VContainer for composition, and service interfaces for gameplay boundaries. Core logic is kept separate from Unity presentation code where practical.

## Ability System <a id="ability-system"></a>

Abilities are data-driven with ScriptableObject definitions for targeting, effects, and presentation. The runtime runner handles execution flow while presenters keep visual feedback separate from gameplay rules.

## Enemy AI

Enemies use a lightweight finite state machine (FSM) for idle, chase, and attack behavior. Movement is handled through NavMeshAgent, while attacks reuse the shared [ability system](#ability-system).


## Progress

- [x] Basic runtime project structure
- [x] Character stats and gameplay state code
- [x] Ability system foundation
- [x] Audio service and emitter pooling
- [x] Core finite state machine utilities
- [ ] Expand combat encounters
- [ ] Add more enemy and ability content
- [ ] Polish UI and localization coverage
- [ ] Add save/load support
- [ ] Improve tests and WebGL build validation

## Third-party Assets

Art assets are from [Kenney](https://kenney.nl/assets).
