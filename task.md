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
   - [x] Has two hands, each capable of holding an item.

3. **Items:**
   - [x] The room contains predefined items coded within the program.
   - [x] There are at least three types of weapons, including one two-handed weapon. Weapons have damage value.
   - [x] There are at least three types of unusable items.
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
   - [x] Next to the world, the following information is displayed:
     - [x] inventory,
     - [x] currently equipped items,
     - [x] if the player is standing on an item, information about it,
     - [x] the player’s current attribute values,
     - [x] the number of collected coins and gold.

6. **Inventory Management:**
   - [x] The player can manage their inventory by:
     - [x] dropping items on the ground,
     - [x] equipping and unequipping items in both hands (correct handling of two-handed weapons is required).

---

# Stage 2: Creating the dungeons

## Objective of the task.
Create following systems:
- creation of the dungeon,
- presentation of the game state.

## Requirements

### Creation of the dungeon
As part of the task, you need to create a dungeon building system that will allow you to create a dungeon according to the given strategies.

List of available building procedures:
 - [x] empty dungeon - each element is an empty element,
 - [x] filled dungeon - each element is a wall,
 - [ ] adding paths - adds random paths through the dungeon,
 - [x] adding chambers - adds random empty fields in the dungeon,
 - [x] adding a central room - adds a large central room to the dungeon,
 - [x] adding items - distributes random items on non-wall fields,
 - [x] adding weapons - distributes random weapons on non-wall fields,
 - [x] adding modified weapons - distributes random weapons with modifiers on non-wall fields,
 - [ ] adding potions - distributes random potions on non-wall fields,
 - [ ] adding enemies - distributes random enemies on non-wall fields.

Procedures:
 - empty dungeon,
 - filled dungeon,

are starter strategies, this means that they must be called before all other strategies.
The other strategies can be combined with each other in any order, and the result of their application should always be correct.

Adding more strategies consisting of a sequence of procedures and new procedures should be easy.

This step should be implemented using the Builder pattern.

### Presentation of the game state
The task should create a centralized system to present the state of the game.
The game system should present:
 - [ ] dungeon,
 - [ ] information about objects on the player's field,
 - [ ] information about the surrounding opponents,
 - [ ] the status of the player - his equipped items, statistics and equipment,
 - [ ] information about the actions taken by the player.
   
The system is the only object in the program that can write to the console.

The system should be easily expandable to write out other objects, or to present data in another form.

This step should be implemented using the Singleton pattern.

### Displaying game instructions
A system that explains how to play should display instructions based on the elements present in the dungeon. This means that, for example, if there are no items in the maze, there should be no information about picking them up.

This step should be done using the Builder pattern and the system implemented in the previous section with the appropriate building procedures.
