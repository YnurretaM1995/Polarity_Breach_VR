# Polarity Breach VR

A first-person shooter for **Meta Quest 2**, built in Unity 6.3 with the Meta XR SDK.

A VR adaptation of *Polarity Breach*, originally a 2.5D shooter with an external camera. This wasn't a port: the combat systems carried over intact, but the entire control, camera and interface layer was redesigned from scratch for VR.

---

## The Core Mechanic

The game is built around a **binary polarity system**: black and white.

- You can only damage enemies of the **opposite** polarity.
- Only projectiles of the **opposite** polarity can damage you.
- Switching polarity has a cooldown, so every switch is a decision.

In first person this created a design problem: the player can't see themselves. Active polarity is communicated through the weapon, which inverts its texture in real time, and through the reticle, which changes along with it.

---

## Controls

| Action 		|	 Input  	|
|---			|---			|
| Movement 		| Left Stick	 	|
| Rotation 		| Head movement 	|
| Fire 			| Right Trigger 	|
| Charged shot 		| Hold Right Trigger	|
| Switch polarity 	| Left Trigger 		|
| Dash 			| Left Grip 		|
| Advance dialogue 	| A Button 		|

Continuous locomotion is **gaze-relative**: the stick moves you toward wherever you're looking. There's no stick turning — the head is the camera.

---

## Systems

**Combat**
- Polarity system with conditional damage (`DamageSystem`, `PolarityComponent`)
- Semi-automatic fire gated by `attackSpeedDelay` cooldown
- Charged shot that charges faster the higher your attack speed
- Directional dash with cooldown
- Object pooling for projectiles and enemies

**Enemies**
- Pursuit AI with line of sight, radius detection, and distinct melee/ranged behaviour (`EnemyPursuitAI`)
- Melee enemies that slow the player through proximity
- Ranged enemies with per-wave configurable spread fire
- Wave spawner with formation patterns and telegraphed spawn warnings

**Boss**
- Four phases with destructible weak points
- Shield that stays invulnerable while enemies remain alive
- Circular bullet patterns and rotating beams of alternating polarity
- Beams built with VFX Graph + Shader Graph, in two variants (additive for white, alpha for black)

**Progression**
- XP system with a scaling level curve
- Level-up upgrade cards: damage, attack speed, or max health
- Run timer with a persistent JSON leaderboard

**Presentation**
- Dialogue system with typewriter effect, portraits, and hold-to-skip
- UI queue that gates gameplay (`UIQueue`)
- Haptic feedback: pulse synced to weapon charge, and an escalating heartbeat in the left controller as health drops
- End-of-run transition with in-headset fade and level teardown

---

### Project Settings

| Setting 		| Value 			|
|---			|---				|
| Platform 		| Meta Quest (Build Profiles) 	|
| Scripting Backend 	| IL2CPP 			|
| Target Architecture 	| ARM64 			|
| Graphics API 		| Vulkan 			|
| Minimum API Level 	| 32 				|
| XR Plugin 		| OpenXR (desktop + Android) 	|
| Render Mode 		| Single Pass Instanced 	|
| Run In Background 	| Enabled 			|


## Current Status

Playable demo from start to finish: menu → three wave-based rooms → boss → victory or defeat.

**Known gaps:**
- Pause menu (needs world-space conversion with laser interaction)
- Debug menu (built with `OnGUI`, which doesn't render in VR)
- Physical dodging by ducking

---
