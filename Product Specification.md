# Product Specification: Freakout

## Overview
**Freakout** is a retro-style arcade game inspired by classics like *Breakout* and *Arkanoid*. The player controls a paddle (bat) to bounce a ball and break bricks arranged at the top of the screen. The objective is to clear all bricks without letting the ball fall below the paddle.

---

## Functional Requirements

### 1. Game Mechanics
- **Ball Physics**
  - Ball moves continuously.
  - Bounces off walls, paddle, and bricks.
  - Changes direction based on collision angle.

- **Paddle Control**
  - Player can move the paddle horizontally and vertically using keyboard or mouse input.
  - Paddle collision affects ball trajectory.

- **Brick Interaction**
  - Bricks are destroyed upon collision with the ball.
  - Game ends when all bricks are destroyed or ball falls below paddle.

- **Score System**
  - Points awarded for each brick destroyed.
  - Display current score during gameplay.
  - Bonus added to score for faster play.

- **Lives System (Optional)**
  - Player starts with 1 life.
  - Losing the ball reduces lives to 0 and ends game.

### 2. User Interface
- **Start Screen**
  - Game title and “Start” button.
  - Instructions or controls overview.

- **In-Game HUD**
  - Score display.
  - High score.

- **Game Over Screen**
  - Final score.
  - Option to restart.

---

## Design Specifications

### 1. Visual Design
- **Game Layout**
  - Top: Brick grid.
  - Middle: Ball in motion.
  - Bottom: Paddle controlled by player.

- **Color Scheme**
  - Contrasting colors for bricks, ball, paddle, and background.
  - Optional: Different brick colors for varying durability or point value.

- **Animations**
  - Smooth ball movement.
  - Paddle motion synced with input.

### 2. Resolution & Platform
- Designed for desktop resolution: default window size 1043x424, resizeable.
- Platform: Windows PC, standalone executable.

### 3. Input Methods
- Keyboard:
  - Arrow keys for paddle movement.
  - Spacebar to launch ball.

### 4. Code Architecture
- Modular structure:
  - Game loop
  - Input handler
  - Collision detection
- Language: C#

---

