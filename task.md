# **Stage 1: Basic Game Program**

## **Task Objective**

Create a console-based RPG game where:
1. The player moves within a single room.
2. The player collects items that can be modified using modifiers (**Decorator** pattern).

## **Functional Requirements**

1. **Room:**
   - [x] The game world is a fixed-size **20x40** grid.
   - [x] Each cell of the world can be:
     - [x] empty (` `),
     - [x] a wall (`█`),
     - [x] occupied by the player (`¶`),
     - [x] containing multiple items (any symbol, depending on the items). // multiple?
   - [x] Player's starting position: **(0, 0)**.
   - [x] At this stage, generating a labyrinth or randomly placing items is not required.
     The world can have predefined placements for items and walls.

2. **Player:**
   - [x] Moves in four directions (controlled by `W`, `S`, `A`, `D`).
   - [x] Cannot move beyond the world boundaries.
   - [x] Cannot pass through walls.
   - [x] Has various attributes such as Strength (P), Dexterity (A), Health (H), Luck (L), Aggression (A), and Wisdom (W).
   - [ ] Has two hands, each capable of holding an item.

3. **Items:**
   - [x] The room contains predefined items coded within the program.
   - [ ] There are at least three types of weapons, including one two-handed weapon. Weapons have damage value.
   - [ ] There are at least three types of unusable items.
   - [x] There are two types of currency: coins and gold.
   - [x] Each weapon has a damage value (weapon usage is not implemented at this stage, only equipping is).
   - [x] If the player steps on a tile with an item, they can pick it up by pressing `E`.

4. **Effects:**
   - [x] Items can be modified with additional effects.
     - [x] Example effects:
       - [x] **"Unlucky"**: -5 to the player's luck.
       - [x] **"Powerful"**: +5 weapon damage.
     - [x] Each item can have multiple effects.
   - [x] All effects should be reflected in the item's name (e.g., "Sword (Unlucky) (Protective)").
   - [x] At least one effect should modify weapon damage, and one should affect player attributes.
   - [x] Effects are applied to items at the time of their creation,
     i.e., in the future when generating the game world randomly,
     but at this stage, they can still be predefined in the code.
   - [x] The **Decorator** pattern must be used to implement this functionality (this is **very** important and is the core of the task – failing to use the pattern results in a 80% point deduction).

5. **Game State Display:**
   - [x] The world is drawn in the console.
   - [ ] Next to the world, the following information is displayed:
     - [x] inventory,
     - [ ] currently equipped items,
     - [x] if the player is standing on an item, information about it,
     - [x] the player’s current attribute values,
     - [x] the number of collected coins and gold.

6. **Inventory Management:**
   - [ ] The player can manage their inventory by:
     - [ ] dropping items on the ground,
     - [ ] equipping and unequipping items in both hands (correct handling of two-handed weapons is required).
