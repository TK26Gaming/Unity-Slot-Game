# Slot Machine

A small 2D slot machine prototype made in Unity as an internship assignment.

The project focuses on clean gameplay logic, smooth reel movement, randomised results, betting/balance management, and a simple retro-style UI.

## Features

- 3 independently spinning reels
- 7 symbol positions per reel
- Smooth reel spinning with staggered stopping
- Random reel results
- Win detection when all 3 reels show the same symbol
- Symbol-based payout multipliers
- Starting balance and betting system
- 10G, 50G, and 100G bet options
- Balance validation before spinning
- Insufficient-balance feedback
- Animated physical lever
- Result display for READY, SPINNING, WIN, NO WIN, and NOT ENOUGH BALANCE
- WebGL build included in `Build/WebGL`

## How to Run

### Requirements

- Unity 6
- Recommended: Unity 6000.3.x or the exact Unity version used to create the project

### Open the Project

1. Clone or download this repository.
2. Open the project using Unity Hub.
3. Open:

```text
Assets/Scenes/Main.unity
```

4. Press **Play**.

### WebGL Build

The WebGL build is located at:

```text
Build/WebGL
```

For local testing, use Unity's **Build And Run** option or host the WebGL files through a local web server. Do not open the WebGL `index.html` directly using `file://`.

## Controls

The main interaction is the slot machine lever.

- Pull the lever to start a spin.
- Select a bet using the buttons on the right side.
- The selected bet is deducted when a spin starts.
- Matching all three symbols awards the corresponding payout.

## Payouts

| Matching Symbol | Payout Multiplier |
|---|---:|
| Cherry | 5x |
| Bell | 10x |
| Bar | 20x |
| Seven | 50x |

Winnings are calculated as:

```text
Current Bet × Payout Multiplier
```

For example, a 10G bet with three Sevens awards 500G.

## Reel Setup

Each reel contains seven symbol positions:

```text
Seven
Cherry
Bell
Bar
Seven
Bell
Bar
```

Each reel independently selects a random target index.

The reels start together but use different spin cycle counts:

```text
Left   → 5 cycles
Center → 7 cycles
Right  → 9 cycles
```

This creates a staggered and more natural stopping effect.

## Project Structure

```text
Assets/
├── Animations/
├── Prefabs/
├── Scenes/
├── Scripts/
├── Sounds/
├── Sprites/
│   ├── Background/
│   ├── Machine/
│   ├── Symbols/
│   └── UI/
├── UI/
└── Reference/

Build/
└── WebGL/
```

## Main Scripts

### `GameManager.cs`

Handles:

- Starting spins
- Generating independent reel results
- Reel stop sequence
- Win detection
- Payout calculation
- Gameplay events

### `Reel.cs`

Handles:

- Reel movement
- Symbol recycling
- Smooth deceleration
- Final symbol alignment

### `BalanceManager.cs`

Handles:

- Starting balance
- Current bet
- Bet deduction
- Winnings
- Bet limits
- Balance validation

### `SlotUI.cs`

Handles:

- Balance display
- Result messages
- Bet button states
- Win/loss/insufficient-balance feedback

### `LeverController.cs`

Handles:

- Lever interaction
- Pull animation
- Normal/pulled lever sprites
- Starting the slot spin

### `SlotSymbol.cs`

Defines the available symbols:

```text
Seven
Cherry
Bell
Bar
```

## Design Decisions & Trade-offs

- The implementation was kept focused on the core slot-machine requirements.
- A fixed seven-position reel layout was used while keeping the result selection independent for each reel.
- The reels stop in sequence rather than simultaneously to make the animation feel more natural.
- Balance, reel, UI, and lever responsibilities are separated into focused scripts.
- No external sound effects were added because no sound assets were provided.
- The optional bonus feature was not added in order to keep the implementation focused on the main requirements.

## Testing

The following were tested in Unity:

- Lever starts a spin
- All three reels spin correctly
- Reels stop in sequence
- Reel results are random
- Matching three symbols awards winnings
- Losing spins display correctly
- Bet selection works
- Balance is deducted when spinning
- Spins are blocked when balance is below the selected bet
- Insufficient-balance feedback works
- Lever pull and return animation works
- WebGL build works when hosted through a local web server

## Author

**Taher Kachwala**
