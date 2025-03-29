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
 - [x] adding paths - adds random paths through the dungeon,
 - [x] adding chambers - adds random empty fields in the dungeon,
 - [x] adding a central room - adds a large central room to the dungeon,
 - [x] adding items - distributes random items on non-wall fields,
 - [x] adding weapons - distributes random weapons on non-wall fields,
 - [x] adding modified weapons - distributes random weapons with modifiers on non-wall fields,
 - [x] adding potions - distributes random potions on non-wall fields,
 - [x] adding enemies - distributes random enemies on non-wall fields.

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
 - [x] dungeon,
 - [x] information about objects on the player's field,
 - [x] information about the surrounding opponents,
 - [x] the status of the player - his equipped items, statistics and equipment,
 - [x] information about the actions taken by the player.
   
The system is the only object in the program that can write to the console.

The system should be easily expandable to write out other objects, or to present data in another form.

This step should be implemented using the Singleton pattern.

### Displaying game instructions
A system that explains how to play should display instructions based on the elements present in the dungeon. This means that, for example, if there are no items in the maze, there should be no information about picking them up.

This step should be done using the Builder pattern and the system implemented in the previous section with the appropriate building procedures.

# Stage 3: Keyboard control, potions

## Task Objective

Implementation of:

- keyboard control (extension/refactor of Stage 1 functionality),
- application of temporal effects (duration measured in turns) via drinkng potions.

New code fragments can (and mostly should), if possible, use and extend functionality from previous stages (e.g. writing out to the console).

## Requirements

### Keyboard control

The user should be able to control the game using the keyboard. Possible actions to perform are:

- [x] moving in four directions (keys: `W`, `S`, `A`, `D`),
- [x] picking up an item (`E`),
- [x] dropping a selected item from the inventory or a hand,
- [x] dropping all items,
- [x] drinking a selected potion from the inventory,
- [x] placing an item in a hand or hiding it in the inventory,
- [x] exiting the game.

Implement the individual actions and their processing using the *Chain Of Responsibility* pattern. Adding new actions, if necessary, should be easy to implement (additional actions may be required in the future stages).

- [x] It will be a good practice to dynamically change the list of supported keys according to what is possible in the given dungeon configuration. There is no point in handling e.g. lifting of items unless they were initially generated.

All available actions should be displayed in the game view. When an additional action is required (e.g. selecting an item to drop), the game should ask for additional information in a concise and understandable way.

- [x] If the user presses a key not associated with any action, a message should be displayed.
Invalid input handling should be also implemented within the *Chain Of Responsibility* pattern (hint: guard action).

### Effects

Each potion has a certain effect which, when player drinks the potion, modifies a certain attribute of the player for a certain period of time.

**NOTE**: The effect after drinking a potion is a different and acts independently from the 'effect' created by taking the item in a hand!

Examples:

- [x] a "power potion" can increase a player's strength by `2` for `5` consecutive turns.
- [ ] a "luck potion" can last for `n` turns; in the `i`-th turn of effect causes the luck attribute to be multiplied by `n-i+1`.

- [ ] An information should be displayed for each effect acting on the player. After a given number of turns, the effect expires and should not be displayed.

Implement the operation of effects (in particular its temporality) using the *Observer* pattern. The effect should update its state after each turn and, if necessary, report its expiration.

- [ ] At least two potions should be created with a temporal effect and one with an 'eternal' effect. The ability to remove effects before the effect ends (e.g. drinking an antidote) is highly acceptable.
