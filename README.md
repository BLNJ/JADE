![JADE Logo](https://user-images.githubusercontent.com/3627916/162974413-7de6a7b1-2540-4c2d-b275-ea4382ec0f8c.png)<br />
<br /><br />
This is just another DMG (GameBoy) emulator.<br />
I started working on this out of interest in how something like this would be programmed after watching [this video (The Ultimate Game Boy Talk (33c3))](https://www.youtube.com/watch?v=HyzD8pNlpwI "this video"), which spiralled out of control fast over time.<br />
The whole project is currently in its third iteration, which I have been working on from time to time in the past 4 years.<br />
This is not meant to solve any problem or re-invent the wheel, I just like working on this.<br />

## Goal
My personal goal is to create a dynamic System, which makes it possible to implement the core into different Frontends (current goal is WPF and ASP.NET).<br />
In regards of features I want to at least implement the CPU and PPU, while being able to run testing ROM´s or even a simple commercial ROM (like Tetris).<br />
Overall I really want to have a full fledged emulator. Not just one which could run commercial ROM´s, but one that makes it possible to take a deep look into the internals of the Emulation, including a Debugger.<br />

## Current State
Like previously mentioned the project is currently in a major refactor.<br />
The previous version had the CPU partially implemented and was able to execute all instructions, but ran into problems all the time. <br />
Resolving those always included manually looking at the ROM in a Hex-Editor and stepping through those instruction in VisualStudio. <br />
A design weakness that I discovered was that as soon as a Instruction started executing said Instruction had power over the whole System, being able to cause all kind of weird behaviour.<br />
With this refactor the whole System gets way more dynamic and functions seperated, including a completely new System for implemented Instruction. <br />
Although Instruction were pretty dynamic beforehand, now they have no power over the whole System altogether, only being able to request values and submit changes to the CPU which also has the sideeffect of being able to precisely see what each Instruction does and being able to see what Instructions did in the past.<br />
This new System for the Instructions also makes it possible that one implementation is capable to handle multiple Instructions, which should make maintenance way easier in the future.<br />

## Features
(This is very limited for now but will be expanded on in the future, with all the necessary details)

### CPU
| Status | Name | Comment |
| ------------ | ------------ | ------------ |
| :construction: | Instructions | Implementing 256*2 Instructions takes time |
| :heavy_exclamation_mark: | Interrupts | Partially implemented, untested |

### GPU
| Status | Name | Comment |
| ------------ | ------------ | ------------ |
| :x: | Everything |   |

### MMU
| Status | Name | Comment |
| ------------ | ------------ | ------------ |
| :white_check_mark: | Regions |  |
| :white_check_mark: | MappedStreams | Makes it possible to interrupts reading/writing to a Stream and limiting it to a specific region
| :white_check_mark: | Loading / Unloading Regions |  |
| :heavy_exclamation_mark: | Banking | MBC0 only for now

### Sound
| Status | Name | Comment |
| ------------ | ------------ | ------------ |
| :x: | Everything |   |

## Ressources
https://gbdev.io/pandocs/
