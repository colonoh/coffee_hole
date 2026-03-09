# COFFEE HOLE

COFFEE HOLE is a proof of concept game where you run a logistics company and have to manage inventory to make sure the right thing gets to the right place. 

## Basic game play elements
- First-person view controls
- Physics-based objects spawn and need to be put somewhere else to score points 
- Players pick up and place objects with their (invisible?) hands
- Once that objects ends up falling inside another tube, the player will score one point

## Difficulty / Pacing
- In the beginning, 1 object (e.g. a coffee cup) will fall down a tube onto the floor when you press a button
- Eventually different objects with different shapes, etc., will spawn (perhaps from a different spot) and need to go somewhere
- The player increases difficulty by increasing the spawn rate of the objects (doubling the rate every increase)
- Objects and can pile up and overflow, and each object has a lifetime
- Perhaps more difficult objects will have shorter lifetimes
- Having objects timeout will incur a loss of score

## Multiplayer
- Target is up to ~4 players
- All players share the same space and the same score
- Players can interfere with each other but this is meant as a fun coop game with friends, not with the public
- There's no inherit benefit to multiplayer besides having fun with friends

## Economy / Automation
- To help you, tools like conveyor belts, fans, ramps, chutes, tubes, etc, can be bought (with score) and placed (by the player) to automate this process
- The win condition is undefined at the moment 
- Tools can be freely repositioned 

## Filtering
- At some point, filtering of objects will be required 
- Possible filtering criteria includes: size-based gates (small objects fall through grates), weight-based (heavy objects don't get pushed
by fans), shape-based (only round things roll down ramps), or color-based (player manually sorts)

## Theme
- The initial theme can be a large warehouse with a placeholder texture 
- Coffee hole is a reference to a whimsical phrase from the game Abiotic Factor "I love coffee in my coffee hole"

## Technical details
- Networking model: Online, no LAN, I suppose host-authoritative over dedicated server
- Target platform: PC only
- Audio/feedback: I would love satisfying feedback — thuds, dings when scoring, quiet conveyor hums
- Unity networking: Need help deciding Netcode: GameObjects (Unity's official) vs. Mirror vs. Fishnet vs. Photon are very
different trade-offs.
- Physics sync: Also need help understanding about syncing physics objects across 4 players.

## MVP v0: Grab and Drop

The smallest possible prototype to validate that first-person physics interaction feels good.

### In scope
- A simple room (floor + 4 walls)
- First-person controls (WASD + mouse look)
- 5-8 physics primitives (cubes, spheres, cylinders) with Rigidbodies
- Grab/release system: left click to pick up an object, left click again to release
- Held objects follow the camera and still collide with the environment
- Released objects keep their velocity (so you can throw things)
- Minimal crosshair UI (dot at screen center)

### Deferred
- Scoring, tubes, spawn buttons
- Multiplayer / networking
- Conveyor belts, fans, automation tools
- Sound effects and audio feedback
- Art, materials, textures beyond default URP lit
- Object filtering mechanics