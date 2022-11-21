# FPS Kit
Arcade First Person Game Kit for Unity 2021.3+. Focuses on player agility, unrealistic physics and implementation of basic features (sprint, crouch, multiple jumps, shoot, aim)

![](https://github.com/peeweek/FPSKit/raw/main/SourceAssets/FPSKit.png)

## Install

From OpenUPM, add the `https://package.openupm.com` scoped registry in your Package Manager category of your Project Settings, and add the `net.peeweek` scope. Then select FPSKit in the package manager's 'My Registry' section.

[![openupm](https://img.shields.io/npm/v/net.peeweek.fpskit?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/net.peeweek.fpskit/)

### Sample Data

Sample data is available as a unity package in the `Packages` folder of the package.


## Requirements

* Unity 2021.3 or newer
* Either HDRP or URP Scriptable Render Pipeline
* Cinemachine
* New Input System

## Features

Here are the current features implemented ✅ with some other features not yet implemented but planned ⬜ (*these features are planned in the current state but could change, be differently implemented, or even removed if irrelevant*)

* Configurable First Person Controller
  * ✅ Move / Look
  * ✅ Head Bobbing
  * ✅ Crouch (Press/Toggle)
  * ✅ Jump (+ Multiple Jumps)
  * ✅ Sprint (Dash)
  * ✅ Aim (Aim down sight)
  * ✅ Slide on steep Slopes
  * ✅ Interaction with objects
* Configurable Input
  * ✅ Legacy Input System
  * ✅ New Input System
  * ✅ Pad Rumble on Shoot
* Camera Rigs
  * ✅ Cinemachine Virtual Cameras
  * ✅ Standard Camera
* Attachments (Weapons, Hands)
  * ⬜ Simple Locomotion
  * ✅ Instant Shooting Weapon (Machine gun)
  * ✅ Rigidbody Launching Weapon (Tennis ball cannon)
* Projectiles
  * ✅ Instant Projectile
  * ✅ Rigid Body Projectile
* Effects
  * ✅ Audio
  * ⬜ Light
  * ✅ VFX
* Pickups
  * ✅ Weapons
  * ⬜ Ammo

